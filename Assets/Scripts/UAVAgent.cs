using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

namespace PA_DronePack
{
    public class UAVAgent : Agent
    {
        // Drone Controll Scripts (PA_DroneControoller)
        private PA_DroneController dcoScript;

        // pre-distance current-distance
        float preDist, curDist;

        // drone in-game parameter
        public bool isHold;
        public int boxType;
        public Vector3 destinationPos;
        public Vector3 boxPos;
        public LineRenderer line;

        // public float energy;
        // public bool isCharging;

        // public float energyRate = 0.12f;

        GameObject MAP;

        public List<string[]> rowData = new List<string[]>();

        void Start()
        {
            //
        }

        void Update()
        {
            //
        }

        public override void Initialize()
        {
            dcoScript = gameObject.GetComponent<PA_DroneController>();
            MAP = GameObject.FindGameObjectWithTag("map");

            gameObject.transform.position = new Vector3(Random.Range(-2f, 2f), Random.Range(3f, 6f), Random.Range(-2f, 2f));

            preDist = 0f;
            curDist = 0f;

            // parameters
            isHold = false;
            boxType = 0;

            // Line Renderer
            line = GetComponent<LineRenderer>();
            line.startWidth = 0.05f; line.endWidth = 0.05f;
            line.SetPosition(0, new Vector3(0f, -10f, 0f));
            line.SetPosition(1, new Vector3(0f, -10f, 0f));

            //MAP.GetComponent<map>().InitWorld();

            // energy = 100f;
            // isCharging = false;
        }

        public override void OnEpisodeBegin()
        {
            gameObject.transform.position = new Vector3(Random.Range(-2f, 2f), Random.Range(3f, 6f), Random.Range(-2f, 2f));

            preDist = 0f;
            curDist = 0f;

            // parameters
            isHold = false;
            boxType = 0;

            // Line Renderer
            line = GetComponent<LineRenderer>();
            line.startWidth = 0.05f; line.endWidth = 0.05f;
            line.SetPosition(0, new Vector3(0f, -10f, 0f));
            line.SetPosition(1, new Vector3(0f, -10f, 0f));
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // AddVectorObs(float state);

            // total 29 + ( 7 x (num_of_uavs - 1) ) + raycast ()

            // this uav physics ( x 6 )
            sensor.AddObservation(gameObject.transform.position);
            sensor.AddObservation(gameObject.GetComponent<Rigidbody>().velocity);
            // sensor.AddObservation(gameObject.GetComponent<Rigidbody>().angularVelocity);

            // energy ( x 11 )
            /*sensor.AddObservation(energy / 100f);
            int idx = (int)((Mathf.Round(energy) + 9) / 10);
            for (int i = 0; i < 11; i++) {
                if (i == idx) sensor.AddObservation(1f);
                else sensor.AddObservation(0f);
            }*/

            // is this charging ( x 2 )
            /*if (isCharging) {
                sensor.AddObservation(0f);
                sensor.AddObservation(1f);
            }
            else {
                sensor.AddObservation(1f);
                sensor.AddObservation(0f);
            }*/

            // box Type ( x 3 )
            if (boxType == 2) {
                sensor.AddObservation(0f);
                sensor.AddObservation(0f);
                sensor.AddObservation(1f);
            }
            else if (boxType == 1) {
                sensor.AddObservation(0f);
                sensor.AddObservation(1f);
                sensor.AddObservation(0f);
            }
            else {
                sensor.AddObservation(1f);
                sensor.AddObservation(0f);
                sensor.AddObservation(0f);
            }
            
            // other uavs position, distance, boxtype ( 7 x (num_of_uavs - 1) )
            GameObject[] uavs = GameObject.FindGameObjectsWithTag("uav");
            foreach (GameObject uav in uavs) {
                if (uav.gameObject.name != gameObject.name) {
                    sensor.AddObservation(uav.transform.position);
                    sensor.AddObservation((uav.transform.position - gameObject.transform.position).magnitude);

                    // other uav action (one-hot)
                    int boxtype = uav.GetComponent<UAVAgent>().boxType;
                    if (boxtype == 2) {
                        sensor.AddObservation(0f);
                        sensor.AddObservation(0f);
                        sensor.AddObservation(1f);
                    }
                    else if (boxtype == 1) {
                        sensor.AddObservation(0f);
                        sensor.AddObservation(1f);
                        sensor.AddObservation(0f);
                    }
                    else {
                        sensor.AddObservation(1f);
                        sensor.AddObservation(0f);
                        sensor.AddObservation(0f);
                    }

                    // other uav energy
                    // sensor.AddObservation(100f - (uav.GetComponent<UAVAgent>().energy / 100f));
                }
            }

            // hub position ( x 6 )
            sensor.AddObservation(MAP.GetComponent<map>().bigHub.transform.position);
            sensor.AddObservation(MAP.GetComponent<map>().smallHub.transform.position);

            // hub destination ( x 2 )
            sensor.AddObservation((MAP.GetComponent<map>().bigHub.transform.position - gameObject.transform.position).magnitude);
            sensor.AddObservation((MAP.GetComponent<map>().smallHub.transform.position - gameObject.transform.position).magnitude);

            // Find nearest boxes
            GameObject[] boxes = GameObject.FindGameObjectsWithTag("parcel");

            GameObject minBigBox = null;
            GameObject minSmallBox = null;
            float minBigBoxDist = 15f;
            float minSmallBoxDist = 15f;

            foreach (GameObject b in boxes) {
                if (b.name.Contains("big") && (gameObject.transform.position - b.transform.position).magnitude < minBigBoxDist && (gameObject.transform.position - b.transform.position).magnitude < 15f) {
                    minBigBox = b;
                    minBigBoxDist = (gameObject.transform.position - b.transform.position).magnitude;
                }
                else if (b.name.Contains("small") && (gameObject.transform.position - b.transform.position).magnitude < minBigBoxDist && (gameObject.transform.position - b.transform.position).magnitude < 15f) {
                    minSmallBox = b;
                    minSmallBoxDist = (gameObject.transform.position - b.transform.position).magnitude;
                }
            }

            // nearest box position ( x 6 ) + nearest box destination ( x 2 )
            if (minBigBox) sensor.AddObservation(minBigBox.transform.position);
            else sensor.AddObservation(Vector3.zero);

            if (minSmallBox) sensor.AddObservation(minSmallBox.transform.position);
            else sensor.AddObservation(Vector3.zero);
            
            if (minBigBox) sensor.AddObservation(minBigBoxDist);
            else sensor.AddObservation(0f);

            if (minSmallBox) sensor.AddObservation(minSmallBoxDist);
            else sensor.AddObservation(0f);
           
            // destination position and distance ( x 4 )
            if (isHold) {
                sensor.AddObservation(destinationPos);
                sensor.AddObservation((destinationPos - gameObject.transform.position).magnitude);
            }
            else {
                sensor.AddObservation(Vector3.zero);
                sensor.AddObservation(0f);
            }

            // charging station position ( x 3 )
            // sensor.AddObservation(MAP.GetComponent<map>().chargingStation.transform.position);

            // charging station distance
            // sensor.AddObservation((gameObject.transform.position - MAP.GetComponent<map>().chargingStation.transform.position).magnitude);
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {   
            /*if (!isCharging) {
                if (energy > 0f) {
                    if (boxType == 0) {
                        energy -= energyRate;
                    }
                    else if (boxType == 1) {
                        energy -= (energyRate * 1.5f);
                    }
                    else if (boxType == 2) {
                        energy -= (energyRate * 2f);
                    }
                }
                // energy is 0%
                else {
                    AddReward(-30f);
                    EndEpisode();
                }    
            }*/

            // Discrete Action
            float drive = 0f;
            float strafe = 0f;
            float lift = 0f;
            
            // forward
            if (actionBuffers.DiscreteActions[1] == 1) {
                drive = 1f;
            }

            // backward
            else if (actionBuffers.DiscreteActions[2] == 1) {
                drive = -1f;
            }

            // left
            if (actionBuffers.DiscreteActions[3] == 1) {
                strafe = 1f;
            }

            // right
            if (actionBuffers.DiscreteActions[4] == 1) {
                strafe = -1f;
            }

            // up
            if (actionBuffers.DiscreteActions[5] == 1) {
                lift = 1f;
            }

            // down
            if (actionBuffers.DiscreteActions[6] == 1) {
                lift = -1f;
            }

            /*if (energy == 0f) {
                dcoScript.DriveInput(0f);
                dcoScript.StrafeInput(0f);
                dcoScript.LiftInput(-1f);
            }
            else {*/
            dcoScript.DriveInput(drive);
            dcoScript.StrafeInput(strafe);
            dcoScript.LiftInput(lift);
            // }

            // Rotation Test
            // Quaternion target = Quaternion.Euler(60f, 60f, 60f);
            // transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 0.5f);


            // Give Reward following Magnitude between destination and this, when this holds parcel.
            if (isHold) {
                curDist = (destinationPos - gameObject.transform.position).magnitude;
                float reward = (preDist - curDist) * 0.5f;
                if (preDist != 0f) {
                    AddReward(reward);
                }
                preDist = curDist;
            }
            else {
                float smallHubDist = (MAP.GetComponent<map>().smallHub.transform.position - gameObject.transform.position).magnitude;
                float bigHubDist = (MAP.GetComponent<map>().bigHub.transform.position - gameObject.transform.position).magnitude;
                curDist = Mathf.Min(smallHubDist, bigHubDist);
                float reward = (preDist - curDist) * 0.5f;
                if (preDist != 0f) {
                    AddReward(reward);
                }
                preDist = curDist;
            }

            // Penalty per each frame
            // AddReward(-0.0001f);
            
            // if UAV is holding
            if (isHold) {
                if (gameObject.transform.position.y < 1f) {
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, 1f, gameObject.transform.position.z);
                }
                line.SetPosition(0, gameObject.transform.position);
                line.SetPosition(1, boxPos);
            }

            else {
                line.SetPosition(0, new Vector3(0f, -10f, 0f));
                line.SetPosition(1, new Vector3(0f, -10f, 0f));
            }

            if (boxType == 0 || !isHold) {
                boxType = 0;
                isHold = false;
            }

            /*if (isHold) {
                if ((gameObject.transform.position - boxPos).magnitude > 5f) {
                    isHold = false;
                    boxPos = Vector3.zero;
                }
            }*/

            // energy
            // float energyReward = (energy - 50f) / 5000f;
            // AddReward(energyReward);

            // step++;
            // Debug.Log(step);

            /*if (step == 1000) {
                step = 0;
                EndEpisode();
            }*/

            MAP.GetComponent<map>().NumberCheck();
        }

        // Player Heuristic Controll
        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discreteActionsOut = actionsOut.DiscreteActions;
            
            discreteActionsOut[0] = 0;
            discreteActionsOut[1] = 0;
            discreteActionsOut[2] = 0;
            discreteActionsOut[3] = 0;
            discreteActionsOut[4] = 0;
            discreteActionsOut[5] = 0;
            discreteActionsOut[6] = 0;

            // forward, backward
            if (Input.GetKey(KeyCode.W)) {
                discreteActionsOut[1] = 1;
            }
            if (Input.GetKey(KeyCode.S)) {
                discreteActionsOut[2] = 1;
            }
   
            // left, right
            if (Input.GetKey(KeyCode.D)) {
                discreteActionsOut[3] = 1;
            }
            if (Input.GetKey(KeyCode.A)) {
                discreteActionsOut[4] = 1;
            }

            // up, down
            if (Input.GetKey(KeyCode.Q)) {
                discreteActionsOut[5] = 1;
            }
            if (Input.GetKey(KeyCode.E)) {
                discreteActionsOut[6] = 1;
            }
        }

        void OnCollisionEnter(Collision other)
        {
            // collide with another agent
            if (other.gameObject.CompareTag("uav"))
            {
                AddReward(-10.0f);
            }

            // collide with obstacles or walls
            if (other.gameObject.CompareTag("obstacle") || other.gameObject.CompareTag("wall"))
            {
                AddReward(-10.0f);
            }

            /*if (other.gameObject.CompareTag("station")) {
                AddReward(1f);
                isCharging = true;
            }*/
        }

        void OnCollisionStay(Collision other) {
            // charge energy

            /*if (other.gameObject.CompareTag("station")) {
                if (energy < 100f) {
                    energy += 1f;
                    AddReward(0.01f);
                }
                else {
                    energy = 100f;
                    AddReward(-0.0003f);
                }
            }*/
        }

        void OnCollisionExit(Collision other) {
            /*if (other.gameObject.CompareTag("station")) {
                isCharging = false;
            }*/
        }

        // Give Reward to this UAV at outside
        public void GiveReward(float reward)
        {
            AddReward(reward);
        }

        public void MakeEpisodeEnd() {
            EndEpisode();
        }
    }

}
