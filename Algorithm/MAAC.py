import argparse
import torch
import os
import numpy as np
from gym.spaces import Box, Discrete
from pathlib import Path
from torch.autograd import Variable
from tensorboardX import SummaryWriter
from utils.make_env import make_env
from utils.buffer import ReplayBuffer
from utils.env_wrappers import SubprocVecEnv, DummyVecEnv
from algorithms.attention_sac import AttentionSAC

from UnityEnv import UnityEnv

import mlagents
from mlagents_envs.environment import UnityEnvironment


def make_parallel_env(env_id, n_rollout_threads, seed):
    def get_env_fn(rank):
        def init_env():
            env = UnityEnv("../Build/" + env_id)
            np.random.seed(seed + rank * 1000)
            return env
        return init_env
    if n_rollout_threads == 1:
        return DummyVecEnv([get_env_fn(0)])
    else:
        return SubprocVecEnv([get_env_fn(i) for i in range(n_rollout_threads)])
        

def run(config):
    model_dir = Path('./models') / config["env_id"] / config["model_name"]
    if not model_dir.exists():
        run_num = 1
    else:
        exst_run_nums = [int(str(folder.name).split('run')[1]) for folder in
                         model_dir.iterdir() if
                         str(folder.name).startswith('run')]
        if len(exst_run_nums) == 0:
            run_num = 1
        else:
            run_num = max(exst_run_nums) + 1

    curr_run = 'run%i' % run_num
    run_dir = model_dir / curr_run
    log_dir = run_dir / 'logs'
    os.makedirs(log_dir)
    logger = SummaryWriter(str(log_dir))

    torch.manual_seed(run_num)
    np.random.seed(run_num)
    env = make_parallel_env(config["env_id"], config["n_rollout_threads"], run_num)
    # env = UnityEnv("../Build/Logistics")
    model = AttentionSAC.init_from_env(env, tau=config["tau"], pi_lr=config["pi_lr"], q_lr=config["q_lr"], gamma=config["gamma"], pol_hidden_dim=config["pol_hidden_dim"], critic_hidden_dim=config["critic_hidden_dim"], attend_heads=config["attend_heads"], reward_scale=config["reward_scale"])

    replay_buffer = ReplayBuffer(config["buffer_length"], model.nagents,
                                 [86 for _ in range(5)],
                                 [env.action_space for _ in range(5)])
    t = 0
    for ep_i in range(0, config["n_episodes"], config["n_rollout_threads"]):
        # print("Episodes %i-%i of %i" % (ep_i + 1, ep_i + 1 + config["n_rollout_threads"], config["n_episodes"]))
        
        obs = env.reset()
        model.prep_rollouts(device='cpu')

        episode_rewards = [0] * env.nagent

        for step in range(config["episode_length"]):
            # rearrange observations to be per agent, and convert to torch Variable
            torch_obs = [Variable(torch.Tensor(np.vstack(obs[:, i])), requires_grad=False) for i in range(model.nagents)]

            # get actions as torch Variables
            torch_agent_actions = model.step(torch_obs, explore=True)
            # convert actions to numpy arrays
            agent_actions = [ac.data.numpy() for ac in torch_agent_actions]
            # rearrange actions to be per environment
            actions = np.array([[ac[i] for ac in agent_actions] for i in range(config["n_rollout_threads"])])

            next_obs, rewards, done, info = env.step(actions)
            replay_buffer.push(obs, agent_actions, rewards, next_obs, done)

            for i in range(5):
                episode_rewards[i] += rewards[0][i]
            
            print("step : %4d/%4d | rewards : %.4f %.4f %.4f %.4f %.4f " % (step, config["episode_length"], *episode_rewards), end='\r')

            obs = next_obs
            t += config["n_rollout_threads"]

            if (len(replay_buffer) >= config["batch_size"] and
                (t % config["steps_per_update"]) < config["n_rollout_threads"]):
                if config["use_gpu"]:
                    model.prep_training(device='gpu')
                else:
                    model.prep_training(device='cpu')
                for u_i in range(config["num_updates"]):
                    sample = replay_buffer.sample(config["batch_size"],
                                                  to_gpu=config["use_gpu"])
                    model.update_critic(sample, logger=logger)
                    model.update_policies(sample, logger=logger)
                    model.update_all_targets()
                model.prep_rollouts(device='cpu')

        print("episode : %6d/%6d | rewards : %.4f %.4f %.4f %.4f %.4f " % (ep_i, config["n_episodes"], *episode_rewards), end='\n\n')

        ep_rews = replay_buffer.get_average_rewards(
            config["episode_length"] * config["n_rollout_threads"])
        for a_i, a_ep_rew in enumerate(ep_rews):
            logger.add_scalar('agent%i/mean_episode_rewards' % a_i,
                              a_ep_rew * config["episode_length"], ep_i)

        if ep_i % config["save_interval"] < config["n_rollout_threads"]:
            model.prep_rollouts(device='cpu')
            os.makedirs(run_dir / 'incremental', exist_ok=True)
            model.save(run_dir / 'incremental' / ('model_ep%i.pt' % (ep_i + 1)))
            model.save(run_dir / 'model.pt')

    model.save(run_dir / 'model.pt')
    env.close()
    logger.export_scalars_to_json(str(log_dir / 'summary.json'))
    logger.close()


if __name__ == '__main__':
    config = dict()

    config["env_id"] = "Logistics"
    config["model_name"] = "MAAC"
    config["n_rollout_threads"] = 1
    config["buffer_length"] = int(1e6)
    config["n_episodes"] = 5_000_000
    config["episode_length"] = 500
    config["steps_per_update"] = 100
    config["num_updates"] = 4
    config["batch_size"] = 1024
    config["save_interval"] = 1000
    config["pol_hidden_dim"] = 128
    config["critic_hidden_dim"] = 128
    config["attend_heads"] = 4
    config["pi_lr"] = 0.001
    config["q_lr"] = 0.001
    config["tau"] = 0.001
    config["gamma"] = 0.99
    config["reward_scale"] = 100.0
    config["use_gpu"] = False

    run(config)
