import tensorflow as tf
import numpy as np
import random
import datetime
from collections import deque
from mlagents.envs import UnityEnvironment

name = "Logistics"
env_path = "../Build/" + name

if __name__ == "__main___":
    env = UnityEnvironment(file_name=env_path)
    
    env_info = env.reset(train_mode=True)

    