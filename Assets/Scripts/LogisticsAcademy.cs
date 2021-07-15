using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class LogisticsAcademy : MonoBehaviour
{
    public GameObject MAP;

    public void Awake() {
        Academy.Instance.OnEnvironmentReset += EnvironmentReset;

        MAP = GameObject.Find("MAP");
    }

    public void EnvironmentReset() {
        MAP.GetComponent<map>().InitWorld();
    }
}
