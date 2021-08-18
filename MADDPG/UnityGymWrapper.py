import mlagents
import numpy as np
from mlagents_envs.environment import UnityEnvironment

"""
# Hoeun Lee - leehe228@konkuk.ac.kr
# 
# Unity Env Gym Wrapper for multi-agents (working on...)
# 
"""

class GymEnv(object):

    # GymEnv Object <- (environment)
    def __init__(self, name):
        self.env = UnityEnvironment(file_name=name)
        self.env.reset()
        
        # behavior names
        self.behavior_names = list(self.env.get_behavior_names())
        self.nbehavior = len(self.behavior_names)

        print('-' * 10)
        print("behavior names : ")
        print(self.behavior_names)
        print("number of behaviors :", self.nbehavior)

        # specs
        self.specs = []

        for _i in range(self.nbehavior):
            self.specs.append(self.env.get_behavior_spec(self.behavior_names[_i]))

        print('-' * 10)
        print("behavior specs")
        print(self.specs)

        self.decision_steps = []
        self.terminal_steps = []

        for _i in range(self.nbehavior):
            d_s, t_s = self.env.get_steps(self.behavior_names[_i])
            self.decision_steps.append(d_s)
            self.terminal_steps.append(t_s)

        self.n_each_agent = []
        
        for _i in range(self.nbehavior):
            self.n_each_agent.append(len(self.decision_steps[_i]))

        print('-' * 10)
        print("n of agents for each behav :")
        print(self.n_each_agent)

        self.nagent = sum(self.n_each_agent)

        print('-' * 10)
        print("nagent :", self.nagent)

        self.observation_space = []
        self.action_space = []
        self.action_space_n = []

        for _i in range(self.nbehavior):
            for _j in range(self.n_each_agent[_i]):
                # self.observation_space.append(self.specs[_i].observation_shapes[0])
                self.observation_space.append(self.reshape_obs(self.decision_steps[_i])[0].shape)
                self.action_space.append(len(self.specs[_i].action_shape))
                self.action_space_n.append(len(self.specs[_i].action_shape))

        print('-' * 10)
        print("observation space")
        print(self.observation_space)

        print('-' * 10)
        print("action space")
        print(self.action_space)


    # reshape observations into 1dim array (use to conc raycast info)
    def reshape_obs(self, d_s):
        return np.column_stack(tuple(d_s.obs))
    

    # obs <- reset(None)
    def reset(self):
        self.env.reset()
        self.decision_steps = []
        self.terminal_steps = []

        for _i in range(self.nbehavior):
            d_s, t_s = self.env.get_steps(self.behavior_names[_i])
            self.decision_steps.append(d_s)
            self.terminal_steps.append(t_s)

        obs = []

        for _i in range(self.nbehavior):
            for o in self.reshape_obs(self.decision_steps[_i]):
                obs.append(o)

        return obs

    
    # obs, reward, done, info <- step(actions)
    def step(self, actions):
        
        lastidx = 0
        for _i in range(self.nbehavior):
            self.env.set_actions(behavior_name=self.behavior_names[_i], action=actions[lastidx:lastidx + self.n_each_agent[_i], :])
            lastidx = self.n_each_agent[_i]

        self.env.step()
        self.decision_steps = []
        self.terminal_steps = []

        for _i in range(self.nbehavior):
            d_s, t_s = self.env.get_steps(self.behavior_names[_i])
            self.decision_steps.append(d_s)
            self.terminal_steps.append(t_s)

        obs = []
        reward = []
        done = []
        info = {}

        for _i in range(self.nbehavior):
            _j = 0
            for o in self.reshape_obs(self.decision_steps[_i]):
                obs.append(o)
                reward.append(self.decision_steps[_i].reward[_j])
                done.append(False)
                _j += 1

        return obs, reward, done, info


    # None <- close(None)
    # Close Environment
    def close(self):
        self.env.close()

