using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gridSensor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("uav")) {
            gameObject.GetComponent<MeshRenderer>().material.color = new Color(255f/255f, 208f/255f, 0f/255f, 20f/255f);
        }
    }

    void OnTriggerStay(Collider other) {
        /*if (other.gameObject.CompareTag("uav")) {
            
        }*/
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("uav")) {
            gameObject.GetComponent<MeshRenderer>().material.color = new Color(255f/255f, 255f/255f, 255f/255f, 0f/255f);
        }
    }
}
