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

        void Start()
        {
            dcoScript = gameObject.GetComponent<PA_DroneController>();
        }

        void Update()
        {
            // Penalty per each frame
            SetReward(-0.001f);

            // if UAV is holding
            if (isHold)
            {
                if (gameObject.transform.position.y < 1f)
                {
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, 1f, gameObject.transform.position.z);
                }
                line.SetPosition(0, gameObject.transform.position);
                line.SetPosition(1, boxPos);
            }
            else
            {
                line.SetPosition(0, new Vector3(0f, -10f, 0f));
                line.SetPosition(1, new Vector3(0f, -10f, 0f));
            }
        }

        public override void Initialize()
        {
            gameObject.transform.position = new Vector3(Random.Range(-2f, 2f), Random.Range(3f, 6f), Random.Range(-2f, 2f));

            preDist = 0f;

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

        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // AddVectorObs(float state);
            sensor.AddObservation(gameObject.transform.position);
        }

        public override void OnActionReceived(float[] vectorAction)
        {
            // Give Force to Move (Action)
            dcoScript.DriveInput(Mathf.Clamp(vectorAction[0], -1f, 1f));
            dcoScript.StrafeInput(Mathf.Clamp(vectorAction[1], -1f, 1f));
            dcoScript.LiftInput(Mathf.Clamp(vectorAction[2], -1f, 1f));

            // Give Reward following Magnitude between destination and this, when this holds parcel.
            if (isHold) {
                curDist = (gameObject.transform.position - destinationPos).magnitude;
                float reward = (preDist - curDist) * 0.1f;
                SetReward(reward);
                preDist = curDist;
            }
        }

        // Player Heuristic Controll
        public override void Heuristic(float[] actionsOut)
        {
            actionsOut[0] = Input.GetAxis("Vertical");
            actionsOut[1] = Input.GetAxis("Horizontal");
        }

        void OnCollisionEnter(Collision other)
        {
            // collide with another agent
            if (other.gameObject.CompareTag("uav"))
            {
                SetReward(-0.1f);
            }

            // collide with obstacles or walls
            if (other.gameObject.CompareTag("obstacle"))
            {
                SetReward(-0.1f);
            }
        }

        // Give Reward to this UAV at outside
        public void GiveReward(float reward)
        {
            SetReward(reward);
        }
    }

}
