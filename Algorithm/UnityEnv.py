import mlagents
import numpy as np
from mlagents_envs.environment import UnityEnvironment

"""
# Hoeun Lee
# 
# Unity Env Gym Wrapper
# 
"""

class UnityEnv(object):
    def __init__(self, name):
        self.env = UnityEnvironment(file_name=name)
        self.env.reset()
        self.behavior_name = list(self.env.get_behavior_names())[0]
        self.spec = self.env.get_behavior_spec(self.behavior_name)

        self.env.reset()
        decision_steps, _ = self.env.get_steps(self.behavior_name)

        self.observation_space = self.reshape_obs(decision_steps).shape
        # self.action_space = self.spec.action_shape
        self.action_space = 7

        self.nagent = len(list(decision_steps))

        self.agents = list(decision_steps)


    def reshape_obs(self, decision_steps):
        # obs[0] is raycast obs, obs[1] is vector obs
        return np.column_stack((decision_steps.obs[0], decision_steps.obs[1]))
    

    def reset(self):
        self.env.reset()
        decision_steps, _ = self.env.get_steps(self.behavior_name)
        obs = self.reshape_obs(decision_steps)

        return obs

    
    def step(self, actions):
        self.env.set_actions(self.behavior_name, actions)
        self.env.step()
        next_decision_step, _ = self.env.get_steps(self.behavior_name)

        next_obs = self.reshape_obs(next_decision_step)
        reward = next_decision_step.reward
        done = [False] * self.nagent
        info = {}

        return next_obs, reward, done, info


    def close(self):
        self.env.close()
