# Environment

### Python API

This Logistics Environment follows [OpenAI Gym](https://github.com/openai/gym) API design :

- `from UnityGymWrapper import GymEnv` - import
- `env = GymEnv(name="path to Unity Environment")` - Returns wrapped environment object.
- `obs = reset()` - Resets environment to the initial state. Returns initial observation.
- `obs, reward, done, info = step(action)` - A single step. Require actions, returns observation, reward, done, information list.

### Unity GymWrapper

/

### Environment Configuration

/

# Observations

Observation size for each agent

```bash
31 + 7 x (nagents - 1) + (27 : raycast observation)
```

**This UAV Information**

- 3 : (x, y, z) coordinates of this UAV
- 3 : (x, y, z) velocity of this UAV
- 3 : one hot encoding of box type (not holding, small box, big box)
- 7 x (n - 1) : other UAVs information (3 - coordinates, 1 - magnitude, 3 - box type)
- 6 : (x, y, z, x, y, z) big box hub and small box hub coordinates
- 2 : each magnitude from this to the big box hub and small box hub
- 6 : (x, y, z, x, y, z) nearest big and small box coordinates (if there's no box nearby, zero)
- 2 : each magnitude from this to the nearest big box and the nearest small box (if there's no box nearby, zero)
- 4 : (x, y, z, m) if UAV holds any box, the coordinates and magnitudes are given. if not, zero

**Raycast Observation (from [Unity MLAgents](https://github.com/Unity-Technologies/ml-agents/blob/release_18_docs/docs/Learning-Environment-Design-Agents.md#raycast-observations))**

- 1 - magnitude value (0 if nothing is detected)
- 2 - one hot encoding of detected object (nothing, building)
- (1 + 2) x 9 - rays per direction

# Actions

UAV can move to 6 directions (up, down, forward, backward, left, right) or not move

The action is discrete action, and size of action set is 7.

- index 0 : not move
- index 1 : forward
- index 2 : backward
- index 3 : left
- index 4 : right
- index 5 : up
- index 6 : down

# Reward

/

**Driving Reward**

```python
(pre distance - current distance) * 0.5
```

To make UAV learn driving forward destination, ***distance penalty*** is given per every step. If UAV holds any parcel, the distance is calculated with a destination where the parcel have to shipped. If UAV have to pick some parcel, distance between UAV and a big box or a small box, whichever is closer to UAV is calculated.

UAV를 목적지를 향해 움직이도록 하기 위하여, 거리 페널티가 매 스텝마다 주어진다. 만약 UAV가 짐을 들고 있다면, 그 짐이 배달되어야 하는 목적지와의 거리를 이용해 계산된다. 만약 UAV가 짐을 들어야 한다면, UAV와 큰 박스와 작은 박스 중 가까운 것과의 거리가 계산된다.

**Shipping Reward**

- +20.0 : Pick up a small box
- +20.0 : complete small box shipping
- +10.0 : First UAV picks up a big box
- +10.0 : First UAV which holds a big box when second UAV picks up
- +20.0 : Second UAV picks up a big box
- +30.0 : complete big box shipping
- -8.0 : when the first UAV dropped a big box
- -15.0 : when tow UAVs dropped a big box

**Collision Penalty**

- -10.0 : when UAV collide with another UAV or a building

# Scenario
![Environment Image](https://s3.us-west-2.amazonaws.com/secure.notion-static.com/160c52b6-b1fd-4ed2-a78f-d1d2adb85bb1/Untitled.png?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAT73L2G45O3KS52Y5%2F20210803%2Fus-west-2%2Fs3%2Faws4_request&X-Amz-Date=20210803T115842Z&X-Amz-Expires=86400&X-Amz-Signature=414e0e9e85800214d445f5317d5b12156839fd6622a241536008646db627500f&X-Amz-SignedHeaders=host&response-content-disposition=filename%20%3D%22Untitled.png%22)
