using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PA_DronePack;

public class bigbox : MonoBehaviour
{
    public GameObject UAV1, UAV2;
    private bool isHold1, isHold2;
    public Vector3 destPos;

    // MAP
    public GameObject MAP;

    public int dx, dz;

    void Start()
    {
        isHold1 = isHold2 = false;

        MAP = GameObject.FindGameObjectWithTag("map");
    }


    void Update()
    {
        if (isHold1 && isHold2)
        {
            Vector3 agent1pos = UAV1.transform.position;
            Vector3 agent2pos = UAV2.transform.position;

            if ((agent1pos - agent2pos).magnitude > 5f) {
                isHold1 = false;
                isHold2 = false;
                UAV1.GetComponent<UAVAgent>().isHold = false;
                UAV2.GetComponent<UAVAgent>().isHold = false;
                UAV1.GetComponent<UAVAgent>().boxType = 0;
                UAV2.GetComponent<UAVAgent>().boxType = 0;
                UAV1.GetComponent<UAVAgent>().GiveReward(-15f);
                UAV2.GetComponent<UAVAgent>().GiveReward(-15f);
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

                UAV1 = null;
                UAV2 = null;
            }
            else {
                Vector3 p = (agent1pos + agent2pos) / 2;
                p.y = Mathf.Max(0.3f, p.y - 1.2f);
                gameObject.transform.position = p;
                UAV1.GetComponent<UAVAgent>().boxPos = p;
                UAV2.GetComponent<UAVAgent>().boxPos = p;
                //
            }
        }

        if (isHold1 && !isHold2)
        {
            if ((UAV1.transform.position - gameObject.transform.position).magnitude > 5f) {
                isHold1 = false;
                isHold2 = false;
                UAV1.GetComponent<UAVAgent>().isHold = false;
                UAV1.GetComponent<UAVAgent>().boxType = 0;

                UAV1.GetComponent<UAVAgent>().GiveReward(-8f);

                UAV1 = null;
            } 
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (!isHold1)
        {
            if (other.gameObject.CompareTag("uav"))
            {
                UAV1 = GameObject.Find(other.gameObject.name);
                if (!UAV1.GetComponent<UAVAgent>().isHold)
                {
                    isHold1 = true;
                    UAV1.GetComponent<UAVAgent>().boxPos = gameObject.transform.position;
                    UAV1.GetComponent<UAVAgent>().isHold = true;
                    UAV1.GetComponent<UAVAgent>().boxType = 2;
                    UAV1.GetComponent<UAVAgent>().destinationPos = destPos;
                    
                    UAV1.GetComponent<UAVAgent>().GiveReward(10f);
                }
                else {
                    UAV1 = null;
                }
            }
        }
            
        if (isHold1 && !isHold2)
        {
            if (other.gameObject.CompareTag("uav"))
            {
                UAV2 = GameObject.Find(other.gameObject.name);
                if (!UAV2.GetComponent<UAVAgent>().isHold) {
                    isHold2 = true;
                    UAV2.GetComponent<UAVAgent>().boxPos = gameObject.transform.position;
                    UAV2.GetComponent<UAVAgent>().isHold = true;
                    UAV2.GetComponent<UAVAgent>().boxType = 2;
                    UAV2.GetComponent<UAVAgent>().destinationPos = destPos;
                    
                    UAV1.GetComponent<UAVAgent>().GiveReward(10f);
                    UAV2.GetComponent<UAVAgent>().GiveReward(20f);

                    // Spawn new parcel
                    MAP.GetComponent<map>().SpawnBigBox();
                }
                else {
                    UAV2 = null;
                }
            }
        }

        if (isHold1 && isHold2)
        {
            if (other.gameObject.CompareTag("destination"))
            {
                if (destPos == other.transform.position && other.gameObject.name.Contains("big_dest")) {
                    UAV1.GetComponent<UAVAgent>().isHold = false;
                    UAV2.GetComponent<UAVAgent>().isHold = false;
                    UAV1.GetComponent<UAVAgent>().boxType = 0;
                    UAV2.GetComponent<UAVAgent>().boxType = 0;
                    isHold1 = false;
                    isHold2 = false;

                    UAV1.GetComponent<UAVAgent>().GiveReward(30f);
                    UAV2.GetComponent<UAVAgent>().GiveReward(30f);

                    Destroy(gameObject);
                    Destroy(GameObject.Find(other.gameObject.name));
                    MAP.GetComponent<map>().world[dx, dz] = 0;

                    UAV1 = null;
                    UAV2 = null;
                }
            }
        }
    }
}
