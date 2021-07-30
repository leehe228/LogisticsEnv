using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

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

        GameObject MAP;

        void Start()
        {
            dcoScript = gameObject.GetComponent<PA_DroneController>();
            isHold = false;

            MAP = GameObject.FindGameObjectWithTag("map");
        }

        void Update()
        {
            // Penalty per each frame
            AddReward(-0.0001f);

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
        }

        public override void Initialize()
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

            //MAP.GetComponent<map>().InitWorld();
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

            // total 22 + ( 3 x (num_of_uavs - 1) ) + raycast

            // this uav physics ( x 6 )
            sensor.AddObservation(gameObject.transform.position);
            sensor.AddObservation(gameObject.GetComponent<Rigidbody>().velocity);
            // sensor.AddObservation(gameObject.GetComponent<Rigidbody>().angularVelocity);
            
            // other uavs position, distance ( 4 x (num_of_uavs - 1) )
            GameObject[] uavs = GameObject.FindGameObjectsWithTag("uav");
            foreach (GameObject uav in uavs) {
                if (uav.gameObject.name != gameObject.name) {
                    sensor.AddObservation(uav.transform.position);
                    sensor.AddObservation((uav.transform.position - gameObject.transform.position).magnitude);
                }
            }

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
            

            // hub position ( x 6 )
            sensor.AddObservation(MAP.GetComponent<map>().bigHub.transform.position);
            sensor.AddObservation(MAP.GetComponent<map>().smallHub.transform.position);

            // hub destination ( x 2 )
            sensor.AddObservation((MAP.GetComponent<map>().bigHub.transform.position - gameObject.transform.position).magnitude);
            sensor.AddObservation((MAP.GetComponent<map>().smallHub.transform.position - gameObject.transform.position).magnitude);
            
            
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

        public override void OnActionReceived(float[] vectorAction)
        {

            // Discrete Action
            float drive = 0f;
            float strafe = 0f;
            float lift = 0;
            
            // forward
            if (vectorAction[1] == 1f) {
                drive = 1f;
            }

            // backward
            else if (vectorAction[2] == 1f) {
                drive = -1f;
            }

            // left
            if (vectorAction[3] == 1f) {
                strafe = 1f;
            }

            // right
            if (vectorAction[4] == 1f) {
                strafe = -1f;
            }

            // up
            if (vectorAction[5] == 1f) {
                lift = 1f;
            }

            // down
            if (vectorAction[6] == 1f) {
                lift = -1f;
            }

            dcoScript.DriveInput(drive);
            dcoScript.StrafeInput(strafe);
            dcoScript.LiftInput(lift);

            // Give Reward following Magnitude between destination and this, when this holds parcel.
            if (isHold) {
                curDist = (destinationPos - gameObject.transform.position).magnitude;
                // AddReward(curDist * 0.05f);
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
                // AddReward(curDist * 0.05f);
                if (preDist != 0f) {
                    AddReward(reward);
                }
                preDist = curDist;
            }
        }

        // Player Heuristic Controll
        public override void Heuristic(float[] actionsOut)
        {
            actionsOut[0] = 0f;
            actionsOut[1] = 0f;
            actionsOut[2] = 0f;
            actionsOut[3] = 0f;
            actionsOut[4] = 0f;
            actionsOut[5] = 0f;
            actionsOut[6] = 0f;

            // forward, backward
            if (Input.GetKey(KeyCode.W)) {
                actionsOut[1] = 1.0f;
            }
            if (Input.GetKey(KeyCode.S)) {
                actionsOut[2] = 1.0f;
            }
   
            // left, right
            if (Input.GetKey(KeyCode.D)) {
                actionsOut[3] = 1.0f;
            }
            if (Input.GetKey(KeyCode.A)) {
                actionsOut[4] = 1.0f;
            }

            // up, down
            if (Input.GetKey(KeyCode.Q)) {
                actionsOut[5] = 1.0f;
            }
            if (Input.GetKey(KeyCode.E)) {
                actionsOut[6] = 1.0f;
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
