import torch
import numpy as np
from torch.autograd import Variable
from utils.env_wrappers import DummyVecEnv
from algorithms.attention_sac import AttentionSAC

from UnityGymWrapper5 import GymEnv


def make_parallel_env():
    def get_env_fn():
        def init_env():
            env = GymEnv(name="../Build_Linux/Logistics",
                         mapsize=13,
                         numbuilding=3,
                         max_smallbox=10,
                         max_bigbox=10,
                         width=1280,
                         height=720,
                         timescale=20,
                         quality_level=5,
                         target_frame_rate=30,
                         capture_frame_rate=30)
            return env
        return init_env
    return DummyVecEnv([get_env_fn()])


def run(config):
    model_path = '../run2/model.pt'  # write model path
    model = AttentionSAC.init_from_save(model_path)
    env = make_parallel_env()

    for ep_i in range(0, config["n_episodes"]):

        obs = env.reset()
        model.prep_rollouts(device='cpu')

        episode_rewards = [0] * env.nagent

        for step in range(config["episode_length"]):
            # rearrange observations to be per agent, and convert to torch Variable
            torch_obs = [Variable(torch.Tensor(np.vstack(obs[:, i])),
                                  requires_grad=False)
                         for i in range(model.nagents)]

            # get actions as torch Variables
            torch_agent_actions = model.step(torch_obs, explore=True)

            # convert actions to numpy arrays
            agent_actions = [ac.data.numpy() for ac in torch_agent_actions]

            # rearrange actions to be per environment
            actions = np.array([[ac[i] for ac in agent_actions]
                               for i in range(1)])

            next_obs, rewards, dones, infos = env.step(actions)

            for i in range(env.nagent):
                episode_rewards[i] += rewards[0][i]

            print("step : %4d/%4d | rewards : %.4f %.4f %.4f %.4f %.4f " %
                  (step, config["episode_length"], *episode_rewards), end='\r')

            obs = next_obs

        print("episode : %8d/%8d | rewards : %.4f %.4f %.4f %.4f %.4f " %
              (ep_i + 1, config["n_episodes"], *episode_rewards), end='\n')

    env.close()


if __name__ == '__main__':
    config = dict()

    config["n_episodes"] = 1000
    config["episode_length"] = 3000

    run(config)
