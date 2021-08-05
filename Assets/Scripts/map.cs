using System.Collections;
using System.Collections.Generic;
// using UnityEngine.UI;
using UnityEngine;

using PA_DronePack;
public class map : MonoBehaviour
{
    // Waiting Queue
    public int smallQueuePointer, bigQueuePointer;
    public int smallQueueMaxLen;
    public int bigQueueMaxLen;
    public int maxSmallBoxNum;
    public int maxBigBoxNum;

    public int smallBoxSuccCount;
    public int bigBoxSuccCount;

    // Building Obstacles
    public GameObject buildingPrefab;

    // Prefabs
    public GameObject smallBoxPrefab;
    public GameObject bigBoxPrefab;
    public GameObject smallDestinationPrefab;
    public GameObject bigDestinationPrefab;
    public GameObject smallHubPrefab;
    public GameObject bigHubPrefab;
    public GameObject gridPrefab;

    // Hub Instance
    public GameObject smallHub;
    public GameObject bigHub;

    // map parameter
    int mapSize;
    int numBuilding;

    public int maxEpisodeLen;
    private int nowEpisodeStep;

    // map table array
    public int[,] world;

    void Start()
    {
        // set parameters
        smallQueueMaxLen = 3;
        bigQueueMaxLen = 3;

        maxSmallBoxNum = 100;
        maxBigBoxNum = 100;

        mapSize = 13; // 13
        numBuilding = 3; // 3

        InitWorld();
    }

    public void InitWorld() {

        smallQueuePointer = 0;
        bigQueuePointer = 0;

        smallBoxSuccCount = 0;
        bigBoxSuccCount = 0;

        // delete all
        GameObject[] hubs = GameObject.FindGameObjectsWithTag("hub");
        GameObject[] destinations = GameObject.FindGameObjectsWithTag("destination");
        GameObject[] parcels = GameObject.FindGameObjectsWithTag("parcel");
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("obstacle");

        foreach (GameObject hub in hubs) {
            Destroy(hub);
        }

        foreach (GameObject destination in destinations) {
            Destroy(destination);
        }

        foreach (GameObject parcel in parcels) {
            Destroy(parcel);
        }

        foreach (GameObject building in buildings) {
            if (building.name != "wall") {
                Destroy(building);
            }
        }

        // init table
        world = new int[mapSize, mapSize];
        for (int i = 0; i < mapSize; i++) {
            for (int j = 0; j < mapSize; j++) {
                world[i, j] = 0;
            }
        }

        int x1 = Random.Range(0, mapSize);
        int z1 = Random.Range(0, mapSize);
        int x2 = Random.Range(0, mapSize);
        int z2 = Random.Range(0, mapSize);
        world[x1, z1] = 1;
        world[x2, z2] = 1;
        
        smallHub = Instantiate(smallHubPrefab);
        bigHub = Instantiate(bigHubPrefab);
        smallHub.transform.position = new Vector3((float)(x1 - mapSize / 2), 0f, (float)(z1 - mapSize / 2));
        bigHub.transform.position = new Vector3((float)(x2 - mapSize / 2), 0f, (float)(z2 - mapSize / 2));

        for (int k = 0; k < numBuilding; k++) {
            while (true) {
                int x = Random.Range(0, mapSize);
                int z = Random.Range(0, mapSize);

                if (world[x, z] == 0) {
                    world[x, z] = 1;
                    GameObject building = Instantiate(buildingPrefab);
                    building.transform.position = new Vector3((float)(x - mapSize / 2), Random.Range(0f, 1f), (float)(z - mapSize / 2));
                    break;
                }
            }
        }

        SpawnSmallBox();
        SpawnBigBox();
    }
    
    void Update()
    {   
        
    }

    public void SpawnSmallBox() {

        if (smallQueuePointer < maxSmallBoxNum) {
            smallQueuePointer++;

            GameObject boxInstance;
            GameObject destinationInstance;
            while (true) {
                int x = Random.Range(0, mapSize);
                int z = Random.Range(0, mapSize);

                if (world[x, z] == 0) {
                    boxInstance = Instantiate(smallBoxPrefab);
                    Vector3 hubPos = smallHub.transform.position;
                    hubPos.y += 5f;
                    boxInstance.transform.position = hubPos;
                    boxInstance.name = "small_box(" + x.ToString() + "," + z.ToString() + ")";
                    destinationInstance = Instantiate(smallDestinationPrefab);
                    destinationInstance.name = "small_dest(" + x.ToString() + "," + z.ToString() + ")";
                    destinationInstance.transform.position = new Vector3((float)(x - mapSize / 2), 0f, (float)(z - mapSize / 2));
                    boxInstance.GetComponent<smallbox>().destPos = destinationInstance.transform.position;
                    boxInstance.GetComponent<smallbox>().dx = x;
                    boxInstance.GetComponent<smallbox>().dz = z;
                    break;
                }
            }
        }
    }

    public void SpawnBigBox() {

        if (bigQueuePointer < maxBigBoxNum) {
            bigQueuePointer++;

            GameObject boxInstance;
            GameObject destinationInstance;
            while (true) {
                int x = Random.Range(0, mapSize);
                int z = Random.Range(0, mapSize);

                if (world[x, z] == 0) {
                    boxInstance = Instantiate(bigBoxPrefab);
                    Vector3 hubPos = bigHub.transform.position;
                    hubPos.y += 5f;
                    boxInstance.transform.position = hubPos;
                    boxInstance.name = "big_box(" + x.ToString() + "," + z.ToString() + ")";
                    destinationInstance = Instantiate(bigDestinationPrefab);
                    destinationInstance.name = "big_dest(" + x.ToString() + "," + z.ToString() + ")";
                    destinationInstance.transform.position = new Vector3((float)(x - mapSize / 2), 0f, (float)(z - mapSize / 2));
                    boxInstance.GetComponent<bigbox>().destPos = destinationInstance.transform.position;
                    boxInstance.GetComponent<bigbox>().dx = x;
                    boxInstance.GetComponent<bigbox>().dz = z;
                    break;
                }
            }
        }
    }
}
