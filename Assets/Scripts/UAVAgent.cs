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

        // is UAV holds a box
        public bool isHold;

        // holding box type (0 : none, 1 : small, 2 : big)
        public int boxType;

        // position of destination
        public Vector3 destinationPos;

        // position of box
        public Vector3 boxPos;

        // line renderer to render line between box and UAV
        public LineRenderer line;

        // MAP gameobject
        GameObject MAP;

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
            // total 29 + ( 7 x (num_of_uavs - 1) ) + raycast ()

            // this uav physics ( x 6 )
            sensor.AddObservation(gameObject.transform.position);
            sensor.AddObservation(gameObject.GetComponent<Rigidbody>().velocity);

            // box Type one-hot encoding ( x 3 )
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
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {   
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

            // make UAV drive
            dcoScript.DriveInput(drive);
            dcoScript.StrafeInput(strafe);
            dcoScript.LiftInput(lift);

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

            // check shipped box number
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
        }

        void OnCollisionStay(Collision other) {
            //
        }

        void OnCollisionExit(Collision other) {
            //
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
