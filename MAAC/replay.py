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

from UnityGymWrapper5 import GymEnv

def make_parallel_env(n_rollout_threads, seed):
    def get_env_fn(rank):
        def init_env():
            env =GymEnv(name="../Build_Linux/Logistics", 
                        max_smallbox=10, 
                        max_bigbox=10, 
                        timescale=1, 
                        quality_level=5, 
                        target_frame_rate=30, 
                        capture_frame_rate=30)
                        
            np.random.seed(seed + rank * 1000)
            return env
        return init_env
    if n_rollout_threads == 1:
        return DummyVecEnv([get_env_fn(0)])
    else:
        return SubprocVecEnv([get_env_fn(i) for i in range(n_rollout_threads)])

def run(config):
    model_path = '../run2/model.pt'
    model = AttentionSAC.init_from_save(model_path)
    run_num = 1
    env = make_parallel_env(config.n_rollout_threads, run_num)
    
    for ep_i in range(0, config.n_episodes, config.n_rollout_threads):
        print("Episodes %i-%i of %i" % (ep_i + 1,
                                        ep_i + 1 + config.n_rollout_threads,
                                        config.n_episodes))
        obs = env.reset()
        model.prep_rollouts(device='cpu')

        for et_i in range(config.episode_length):
            # rearrange observations to be per agent, and convert to torch Variable
            torch_obs = [Variable(torch.Tensor(np.vstack(obs[:, i])),
                                  requires_grad=False)
                         for i in range(model.nagents)]
            # get actions as torch Variables
            torch_agent_actions = model.step(torch_obs, explore=True)
            # convert actions to numpy arrays
            agent_actions = [ac.data.numpy() for ac in torch_agent_actions]
            # rearrange actions to be per environment
            actions = np.array([[ac[i] for ac in agent_actions] for i in range(config.n_rollout_threads)])
            next_obs, rewards, dones, infos = env.step(actions)

            obs = next_obs
            
    env.close()

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("--n_rollout_threads", default=1, type=int)
    parser.add_argument("--buffer_length", default=int(1e6), type=int)
    parser.add_argument("--n_episodes", default=100, type=int)
    parser.add_argument("--episode_length", default=3000, type=int)
    parser.add_argument("--pol_hidden_dim", default=128, type=int)
    parser.add_argument("--critic_hidden_dim", default=128, type=int)
    parser.add_argument("--attend_heads", default=4, type=int)
    parser.add_argument("--pi_lr", default=0.001, type=float)
    parser.add_argument("--q_lr", default=0.001, type=float)
    parser.add_argument("--tau", default=0.001, type=float)
    parser.add_argument("--gamma", default=0.99, type=float)
    parser.add_argument("--reward_scale", default=100., type=float)
    parser.add_argument("--use_gpu", action='store_true')

    config = parser.parse_args()

    config.use_gpu = False

    run(config)