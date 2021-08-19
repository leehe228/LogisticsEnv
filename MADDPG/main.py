import argparse
import torch
import time
import os
import numpy as np
from gym.spaces import Box, Discrete
from pathlib import Path
from torch.autograd import Variable
from tensorboardX import SummaryWriter
from utils.make_env import make_env
from utils.buffer import ReplayBuffer
from utils.env_wrappers import SubprocVecEnv, DummyVecEnv
from algorithms.maddpg import MADDPG

from UnityGymWrapper4 import GymEnv
from mlagents_envs.environment import UnityEnvironment

USE_CUDA = False  # torch.cuda.is_available()

def make_parallel_env(env_id, n_rollout_threads, seed):
    def get_env_fn(rank):
        def init_env():
            env = GymEnv(name="../Build/Logistics")
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

    torch.manual_seed(config["seed"])
    np.random.seed(config["seed"])
    if not USE_CUDA:
        torch.set_num_threads(config["n_training_threads"])
    env = make_parallel_env(config["env_id"], config["n_rollout_threads"], config["seed"])
    maddpg = MADDPG.init_from_env(env, agent_alg=config["agent_alg"],
                                  adversary_alg=config["adversary_alg"],
                                  tau=config["tau"],
                                  lr=config["lr"],
                                  hidden_dim=config["hidden_dim"])
    replay_buffer = ReplayBuffer(config["buffer_length"], maddpg.nagents,
                                 [obsp[0] for obsp in env.observation_space],
                                 [acsp for acsp in env.action_space])
    t = 0
    for ep_i in range(0, config["n_episodes"], config["n_rollout_threads"]):
        
        obs = env.reset()
        # obs.shape = (n_rollout_threads, nagent)(nobs), nobs differs per agent so not tensor
        maddpg.prep_rollouts(device='cpu')

        explr_pct_remaining = max(0, config["n_exploration_eps"] - ep_i) / config["n_exploration_eps"]
        maddpg.scale_noise(config["final_noise_scale"] + (config["init_noise_scale"] - config["final_noise_scale"]) * explr_pct_remaining)
        maddpg.reset_noise()

        episode_rewards = [0] * env.nagent

        for step in range(config["episode_length"]):
            # rearrange observations to be per agent, and convert to torch Variable
            torch_obs = [Variable(torch.Tensor(np.vstack(obs[:, i])),
                                  requires_grad=False)
                         for i in range(maddpg.nagents)]
            # get actions as torch Variables
            torch_agent_actions = maddpg.step(torch_obs, explore=True)
            # convert actions to numpy arrays
            agent_actions = [ac.data.numpy() for ac in torch_agent_actions]
            # rearrange actions to be per environment
            actions = np.array([[ac[i] for ac in agent_actions] for i in range(config["n_rollout_threads"])])

            next_obs, rewards, dones, infos = env.step(actions)
            replay_buffer.push(obs, agent_actions, rewards, next_obs, dones)
            
            for i in range(env.nagent):
                episode_rewards[i] += rewards[0][i]

            if step % 100 == 0:
                print("step : %4d/%4d | rewards : %.4f %.4f %.4f %.4f %.4f " % (step, config["episode_length"], *episode_rewards), end='\r')
            
            obs = next_obs
            t += config["n_rollout_threads"]

            if (len(replay_buffer) >= config["batch_size"] and
                (t % config["steps_per_update"]) < config["n_rollout_threads"]):
                if USE_CUDA:
                    maddpg.prep_training(device='gpu')
                else:
                    maddpg.prep_training(device='cpu')
                for u_i in range(config["n_rollout_threads"]):
                    for a_i in range(maddpg.nagents):
                        sample = replay_buffer.sample(config["batch_size"],
                                                      to_gpu=USE_CUDA)
                        maddpg.update(sample, a_i, logger=logger)
                    maddpg.update_all_targets()
                maddpg.prep_rollouts(device='cpu')

        print("episode : %8d/%8d | rewards : %.4f %.4f %.4f %.4f %.4f " % (ep_i, config["n_episodes"], *episode_rewards), end='\n\n')

        ep_rews = replay_buffer.get_average_rewards(
            config["episode_length"] * config["n_rollout_threads"])
        for a_i, a_ep_rew in enumerate(ep_rews):
            logger.add_scalar('agent%i/mean_episode_rewards' % a_i, a_ep_rew, ep_i)

        if ep_i % config["save_interval"] < config["n_rollout_threads"]:
            os.makedirs(run_dir / 'incremental', exist_ok=True)
            maddpg.save(run_dir / 'incremental' / ('model_ep%i.pt' % (ep_i + 1)))
            maddpg.save(run_dir / 'model.pt')

    maddpg.save(run_dir / 'model.pt')
    env.close()
    logger.export_scalars_to_json(str(log_dir / 'summary.json'))
    logger.close()


if __name__ == '__main__':
    config = dict()

    config["env_id"] = "Logistics"
    config["model_name"] = "MADDPG"
    config["seed"] = 1
    config["n_rollout_threads"] = 1
    config["n_training_threads"] = 1
    config["buffer_length"] = int(1e6)
    config["n_episodes"] = 30000
    config["episode_length"] = 1000
    config["steps_per_update"] = 100
    config["batch_size"] = 1024
    config["n_exploration_eps"] = 25000
    config["init_noise_scale"] = 0.3
    config["final_noise_scale"] = 0.0
    config["save_interval"] = 5000
    config["hidden_dim"] = 128
    config["lr"] = 0.01
    config["tau"] = 0.01
    config["agent_alg"] = "MADDPG" # MADDPG or DDPG
    config["adversary_alg"] = "MADDPG" # MADDPG or DDPG

    run(config)
