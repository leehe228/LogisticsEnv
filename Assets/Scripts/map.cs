using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class map : MonoBehaviour
{
    // Waiting Queue
    public int smallQueuePointer, bigQueuePointer;
    public int smallQueueMaxLen;
    public int bigQueueMaxLen;
    public float smallQueueInterval;
    public float bigQueueInterval;
    public int maxSmallBoxNum;
    public int maxBigBoxNum;

    // Building Obstacles
    public GameObject buildingPrefab;

    // Prefabs
    public GameObject smallBoxPrefab;
    public GameObject bigBoxPrefab;
    public GameObject smallDestinationPrefab;
    public GameObject bigDestinationPrefab;
    public GameObject smallHubPrefab;
    public GameObject bigHubPrefab;

    // Hub Instance
    public GameObject smallHub;
    public GameObject bigHub;

    // map parameter
    int mapSize;
    int numBuilding;

    // map table array
    int[,] world;

    void Start()
    {
        // set parameters
        smallQueuePointer = 0;
        bigQueuePointer = 0;

        smallQueueMaxLen = 3;
        bigQueueMaxLen = 3;

        smallQueueInterval = 20f;
        bigQueueInterval = 20f;

        maxSmallBoxNum = 10;
        maxBigBoxNum = 5;

        mapSize = 40;
        numBuilding = 15;

        // init table
        world = new int[mapSize, mapSize];
        for (int i = 0; i < mapSize; i++) {
            for (int j = 0; j < mapSize; j++) {
                world[i, j] = 0;
            }
        }

        // InvokeRepeating("AddSmallBox", 0f, smallQueueInterval);
        // InvokeRepeating("AddBigBox", 0f, bigQueueInterval);

        InitWorld();
    }

    void InitWorld() {
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
                    building.transform.position = new Vector3((float)(x - mapSize / 2), Random.Range(-2f, 5f), (float)(z - mapSize / 2));
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

    /*void AddSmallBox() {
        if (smallQueuePointer < smallQueueMaxLen) {
            smallQueuePointer++;
        }
    }*/

    /*void AddBigBox() {
        if (bigQueuePointer < bigQueueMaxLen) {
            bigQueuePointer++;
        }
    }*/

    public void SpawnSmallBox() {
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
                destinationInstance = Instantiate(smallDestinationPrefab);
                destinationInstance.transform.position = new Vector3((float)(x - mapSize / 2), 0f, (float)(z - mapSize / 2));
                break;
            }
        }
        boxInstance.GetComponent<smallbox>().destPos = destinationInstance.transform.position;
    }

    public void SpawnBigBox() {
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
                destinationInstance = Instantiate(bigDestinationPrefab);
                destinationInstance.transform.position = new Vector3((float)(x - mapSize / 2), 0f, (float)(z - mapSize / 2));
                break;
            }
        }
        boxInstance.GetComponent<smallbox>().destPos = destinationInstance.transform.position;
    }
}
