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
    public float smallQueueInterval;
    public float bigQueueInterval;
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

    // step Text
    // public Text text;

    void Start()
    {
        // set parameters
        smallQueuePointer = 0;
        bigQueuePointer = 0;

        smallQueueMaxLen = 3;
        bigQueueMaxLen = 3;

        smallQueueInterval = 20f;
        bigQueueInterval = 20f;

        maxSmallBoxNum = 5;
        maxBigBoxNum = 3;

        mapSize = 40;
        numBuilding = 10;

        maxEpisodeLen = 3000;
        nowEpisodeStep = 0;

        smallBoxSuccCount = 0;
        bigBoxSuccCount = 0;

        // InvokeRepeating("AddSmallBox", 0f, smallQueueInterval);
        // InvokeRepeating("AddBigBox", 0f, bigQueueInterval);

        // text = GameObject.Find("Text").GetComponent<Text>();

        InitWorld();
    }

    void InitWorld() {
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
        // Make this episode done
        /*if (smallQueuePointer == maxSmallBoxNum && bigQueuePointer == maxBigBoxNum) {
            GameObject[] uavs = GameObject.FindGameObjectsWithTag("uav");

            foreach (GameObject uav in uavs) {
                uav.GetComponent<UAVAgent>().MakeEpisodeEnd();
                InitWorld();
            }
        }

        if (nowEpisodeStep > maxEpisodeLen) {
            GameObject[] uavs = GameObject.FindGameObjectsWithTag("uav");

            foreach (GameObject uav in uavs) {
                uav.GetComponent<UAVAgent>().MakeEpisodeEnd();
            }
        }
        nowEpisodeStep++;

        text.text = nowEpisodeStep.ToString() + "/" + maxEpisodeLen.ToString();*/

        // text.text = "Episode Info\nSmall Box - 성공 : " + smallBoxSuccCount.ToString() + " / 생성 : " + smallQueuePointer.ToString() + " / 전체 : " + maxSmallBoxNum.ToString() + "\nBig Box - 성공 : " + bigBoxSuccCount.ToString() + " / 생성 : " + bigQueuePointer.ToString() + " / 전체 : " + maxBigBoxNum.ToString();
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
                    destinationInstance = Instantiate(smallDestinationPrefab);
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
                    destinationInstance = Instantiate(bigDestinationPrefab);
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
