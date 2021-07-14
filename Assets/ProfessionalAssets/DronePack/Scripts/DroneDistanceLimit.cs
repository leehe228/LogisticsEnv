using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PA_DronePack
{
    public class DroneDistanceLimit : MonoBehaviour
    {
        Vector3 startPosition;
        PA_DroneController droneScript;
        public CameraTeleportEffect teleportScript;
        public float distanceLimit = 150f;

        void Start()
        {
            startPosition = transform.position;
            droneScript = transform.GetComponent<PA_DroneController>();
            StartCoroutine(CheckDistance());
        }

        IEnumerator CheckDistance()
        {
            while (true)
            {
                if (Vector3.Distance(startPosition, transform.position) > distanceLimit)
                {
                    droneScript.ResetDronePosition();
                    teleportScript.StartEffects();
                    teleportScript.StopEffects();
                }
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
