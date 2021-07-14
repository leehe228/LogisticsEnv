using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PA_DronePack;

public class smallbox : MonoBehaviour
{
    public GameObject UAV;
    private bool isHold;

    public Vector3 destPos;

    void Start()
    {
        isHold = false;
    }

    void Update()
    {
        if (isHold)
        {
            Vector3 uavPos = UAV.transform.position;
            uavPos.y = Mathf.Max(0.3f, uavPos.y - 1.2f);
            gameObject.transform.position = uavPos;
            UAV.GetComponent<UAVAgent>().boxPos = gameObject.transform.position;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (!isHold && other.gameObject.CompareTag("uav"))
        {
            UAV = GameObject.Find(other.gameObject.name);

            if (!UAV.GetComponent<UAVAgent>().isHold)
            {
                isHold = true;
                UAV.GetComponent<UAVAgent>().boxPos = gameObject.transform.position;
                UAV.GetComponent<UAVAgent>().isHold = true;
                UAV.GetComponent<UAVAgent>().boxType = 1;
                UAV.GetComponent<UAVAgent>().destinationPos = destPos;
                UAV.GetComponent<UAVAgent>().GiveReward(1.0f);
            }
        }

        if (other.gameObject.CompareTag("destination"))
        {
            if (destPos == other.transform.position)
            {
                isHold = false;
                UAV.GetComponent<UAVAgent>().isHold = false;
                UAV.GetComponent<UAVAgent>().boxType = 0;
                UAV.GetComponent<UAVAgent>().GiveReward(5.0f);

                Destroy(this);
                Destroy(GameObject.Find(other.gameObject.name));
            }
        }
    }
}
