using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class LogisticsAcademy : MonoBehaviour
{
    public GameObject MAP;

    // Environment Parameters
    int mapsize = 13;
    int numbuilding = 3;
    int slimit = 10;
    int blimit = 10;

    public void Awake() {
        Academy.Instance.OnEnvironmentReset += EnvironmentReset;
        var envParams = Academy.Instance.EnvironmentParameters;

        MAP = GameObject.Find("MAP");

        mapsize = (int)(envParams.GetWithDefault("mapsize", 13f));
        numbuilding = (int)(envParams.GetWithDefault("building_num", 3f));
        slimit = (int)(envParams.GetWithDefault("slimit", 10f));
        blimit = (int)(envParams.GetWithDefault("blimit", 10f));
    }

    public void EnvironmentReset() {
        MAP.GetComponent<map>().WriteCSV();
        MAP.GetComponent<map>().InitWorld(mapsize, numbuilding, slimit, blimit);
    }
}
