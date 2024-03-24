## UAV Logistics Environment for MLRL
This **UAV Logistics Environment** with a continuous observation and discrete action space, along with **physical based UAVs** and parcels which powered by Unity Engine. Used in Paper ["Multiagent Reinforcement Learning Based on Fusion-Multiactor-Attention-Critic for Multiple-Unmanned-Aerial-Vehicle Navigation Control"](https://www.mdpi.com/1996-1073/15/19/7426)(MDPI Energies 2022, 15(19), 7426 (SCIE), 2022.10.10.) and ["Multi-agent Reinforcement Learning-Based UAS Control for Logistics EnvironmentsMulti-agent Reinforcement Learning-Based UAS Control for Logistics Environments"](https://link.springer.com/chapter/10.1007/978-981-19-2635-8_71)(Springer LNEE, volume 913 (SCOPUS). 2022.09.30.)
<br><br>

### 📌 LogisticsEnv Builds Release (1.0.0)
(2024. 3. 11.)
- [Windows(x86-64)](https://konkukackr-my.sharepoint.com/:u:/g/personal/leehe228_konkuk_ac_kr/ETkqZMK1N4hJoETrxWPzzKkBNhSc-zeMMA1y3jkYguvltg?e=C7tmZr)
- [Linux](https://konkukackr-my.sharepoint.com/:u:/g/personal/leehe228_konkuk_ac_kr/EeXk4mSlfs9LmU2sITwh9UEBiSa_SzBqEv1lZk3Lr3C1KA?e=awadJA)
- [MacOS(Intel 64bit)](https://konkukackr-my.sharepoint.com/:f:/g/personal/leehe228_konkuk_ac_kr/Euhm6CZyGwRDvX8AHMmvi74BpHjcytjgcyaUvXat5eYARQ?e=vhFYns)
- [MacOS(Apple Silicon](https://konkukackr-my.sharepoint.com/:f:/g/personal/leehe228_konkuk_ac_kr/EmHkAWmasaFIiuJjQvJQyJYBpxX4zdgNbvxQssbRtVNtSg?e=xEHhER)
<br>

### 📌 Trained Model
<img width="239" alt="image" src="https://github.com/leehe228/LogisticsEnv/assets/37548919/759d14dc-1f29-4513-bd53-e1f4ab2d7eda"><br/>
- [Download - MAAC Trained Model files(.pt)](https://drive.google.com/drive/folders/1cd17nqOY6nNDjxIUsFIw1KQF8No4MgIV?usp=share_link)
```python
model_path = '~~/<trained_model_name>.pt'  # write model path
model = AttentionSAC.init_from_save(model_path) # load model data from saved file
```
<br>

### Requirements

- Python 3.8 (minimum)
- [OpenAI `baselines`](https://github.com/openai/baselines), version 0.1.6
- [`pytorch`](https://pytorch.org/get-started/locally/), < 1.9.0, >= 1.6.0 (compatible to your CUDA version)
- [`tensorboard`](https://github.com/tensorflow/tensorboard) (compatible to PyTorch version)
- [OpenAI `gym`](https://github.com/openai/gym), version 0.15.7
- [Unity `mlagents`](https://github.com/Unity-Technologies/ml-agents), version 0.27.0 (Release 18)
<br>

### My Environments
- Ubuntu 20.04 LTS / Python 3.8.10 / Anaconda 3.1 (Virtual Environment)
- NVIDIA GeForce RTX 3070 / Intel(R) Core(TM) i5-10400F (@2.90GHz) / 32GB Memory (RAM)
- CUDA 11.1 / [CuDNN 8.1.1](https://developer.nvidia.com/rdp/cudnn-archive#a-collapse811-111)
- `torch 1.8.2+cu111` / `torchaudio 0.8.2` / `torchvision 0.9.2+cu111`
- It took 13.9 days to train MAAC model with 150K episodes.

<br>

**Unity Editor**

- Unity Editor, version 2020.3.x (LTS) (minimum)
- Unity ML-Agents Package Release 2.1.0 exp1 (Not compatible with other versions)

<br>

### Getting Started
- install requirements packages (in a virtual-env)
- `git clone https://github.com/dmslab-konkuk/LogisticsEnv.git`
- `cd MAAC` or `cd MADDPG`
- edit parameters in main.py (learning parameters)
- Followed by your OS, select built environment file between `Build_Windows` or `Build_Linux` (give right path)
- if your OS is Linux(Ubuntu), before training grant permission is essential, with `sudo chmod a+xwr /Build_Linux/Logistics.x86_64`
- `python main.py` to run training
- To replay with your trained model, set path to the `model.pt` on `replay.py` and `python replay.py` to replay

**Tensorboard**

- `MAAC/models/Logistics/MAAC/` or `MADDPG/models/Logistics/MADDPG`
- `tensorboard --logdir=runX`
- open `localhost:6006`

**Parcel Counter**

- `MAAC/CSV/countXXXX.csv` : number of successfully shipped parcel is written in this csv file. (XXXX must be yyyyMMddHHmmss of training start time)
- first row is number of small box, second is number of big box, third is sum of both.

**Timer**

- `MAAC/CSV/timerXXXX.csv` : spent time to finish shipping given boxes. (finishing condigion follows `max_smallbox` and `max_bigbox` parameters) 
- if UAVs failed to ship all of given parcels, a time-written line will be not appended. 
- the line is milli-second. (1Kms is a second)

<br>

### Scenario
![](https://images.velog.io/images/leehe228/post/8d01796d-133d-41ba-8ec6-bf43f9937032/image.png)
- UAVs need to move parcels(boxes) from **hubs** to **destinations**.
- There are two types of boxes - big boxes and small boxes. A big box can only be moved if two or more UAVs cooperate.
- The size of map and the number of UAVs and obstacles can be customized for various environments.
- When UAV touches the box, the box is connected. If UAV is farther than a certain distance from the box, the box will fall off.

<br>

### Used Algorithm
- [Multi-Actor-Attention-Ctritic](https://github.com/shariqiqbal2810/MAAC) (MAAC) 
from [Actor-Attention-Critic for Multi-Agent Reinforcement Learning](https://arxiv.org/abs/1810.02912)  (Iqbal and Sha, ICML 2019)
- [Multi-Agent DDPG](https://github.com/shariqiqbal2810/maddpg-pytorch) (MADDPG)

<br>

### Python API
**Gym Functions**

This Logistics Environment follows [OpenAI Gym](https://github.com/openai/gym) API design :

- `from UnityGymWrapper5 import GymEnv` - import class (newest version is Wrapper5)
- `env = GymEnv(name="path to Unity Environment", ...)` - Returns wrapped environment object.
- `obs = reset()` - Resets environment to the initial state. Returns initial observation.
- `obs, reward, done, info = step(actions)` - A single step. Require actions, returns observation, reward, done, information list.

**example**
```python
from UnityGymWrapper5 import GymEnv # Unity Gym Style Wrapper
env = GymEnv(name="../Build_Linux/Logistics") # Call Logistics Environment
done, obs = False, env.reset() # reset Environment

while not done:
    actions = get_actions(obs) # get actions
    next_obs, reward, done, info = env.step(actions) # next step
    obs = next_obs
```
<br>

**Unity Gym Wrapper**
This Wrapper can wrap Unity ML-Agents Environment (API version 2.1.0 exp1, mlagents version 0.27.0) which has multiple Discrete-Action-Agent.

GymWrapper provided by Unity supports only single agent environment.
[UnityGymWrapper5.py](https://github.com/dmslab-konkuk/LogisticsEnv/blob/main/MAAC/UnityGymWrapper5.py) is in Github Repository.
<br>

**Parameter Configurations**
`env = GymEnv(name='', width=0, height=0, ...)`

- `width` : Defines the width of the display. (Must be set alongside height)
- `height` : Defines the height of the display. (Must be set alongside width)
- `timescale` : Defines the multiplier for the deltatime in the simulation. If set to a higher value, time will pass faster in the simulation but the physics may perform unpredictably.
- `quality_level` : Defines the quality level of the simulation.
- `target_frame_rate` : Instructs simulation to try to render at a specified frame rate.
- `capture_frame_rate` : Instructs the simulation to consider time between updates to always be constant, regardless of the actual frame rate.
- `name` : path to Unity Built Environment (ex : `../Build_Linux/Logistics`)
- `mapsize` : size of map in virtual environment (x by x)
- `numbuilding` : number of buildings (obstacle)
- `max_smallbox` : max number of small box will be generated
- `max_bigbox` : max number of big box will be generated

<br>

### Observation
Observation size for each agent
```
29 + 7 x (nagent - 1) + (27 : ray-cast obs)
```

**This UAV Information**

- 3 : (x, y, z) coordinates of this UAV
- 3 : (x, y, z) velocity of this UAV
- 3 : one hot encoding of box type (not holding, small box, big box)
- 7 x (n - 1) : other UAVs information (3 - coordinates, 1 - distance, 3 - box type one-hot encoding)
- 6 : (x, y, z, x, y, z) big box hub and small box hub coordinates
- 2 : each distance from this to the big box hub and small box hub
- 6 : (x, y, z, x, y, z) nearest big and small box coordinates (if there's no box nearby, zero)
- 2 : each distance from this to the nearest big box and the nearest small box (if there's no box nearby, zero)
- 4 : (x, y, z, d) if UAV holds any box, the coordinates and distances are given. if not, zero

**Raycast Observation** (from [Unity ML-Agents](https://github.com/Unity-Technologies/ml-agents/blob/release_18_docs/docs/Learning-Environment-Design-Agents.md#raycast-observations))

- 1 - distance (0 if nothing is detected)
- 2 - one hot encoding of detected object (nothing, building)
- (1 + 2) x 9 - rays per direction

<br>

### Actions
UAV can move to 6 directions (up, down, forward, backward, left, right) or not move

The action is **discrete action**, and size of action set is 7.

- index 0 : not move
- index 1 : forward
- index 2 : backward
- index 3 : left
- index 4 : right
- index 5 : up
- index 6 : down

<br>

### Reward
**Driving Reward**

```python
(pre distance - current distance) * 0.5
```

To make UAV learn driving forward destination, ***distance penalty*** is given per every step. If UAV holds any parcel, the distance is calculated with a destination where the parcel have to shipped. If UAV have to pick some parcel, distance between UAV and a big box or a small box, whichever is closer to UAV is calculated.

**Shipping Reward**

- +20.0 : Pick up a small box
- +20.0 : complete small box shipping
- +10.0 : First UAV picks up a big box
- +10.0 : First UAV which holds a big box when second UAV picks up
- +20.0 : Second UAV picks up a big box
- +30.0 : complete big box shipping
- -8.0 : when the first UAV dropped a big box
- -15.0 : when tow UAVs dropped a big box

These values are designed to make UAV work efficiently.

**Collision Penalty**

- -10.0 : when UAV collide with another UAV or a building

UAV has to avoid buildings and another UAV with raycast observation.

<br>

### Training Result
We trained model with random-decision model, reinforcement model (SAC, DQN, MADDPG) and MAAC (Multi-Attention-Actor-Critic for Multi-Agent) model. We trained **30k episode** each model.

<br>

### Credit
developed by [Hoeun Lee](https://github.com/leehe228) (in [DMS Lab](https://github.com/dmslab-konkuk) in [Dept. of Computer Science and Engineering](http://cse.konkuk.ac.kr/), [Konkuk University](http://www.konkuk.ac.kr/do/Index.do), Seoul, Korea)

Copyright Hoeun Lee, 2021, All Right Reserved. 
