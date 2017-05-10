using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class trackValues : MonoBehaviour
{
    public bool writeToFile = false;
    private System.DateTime now = System.DateTime.Now;

    public bool trackTimes = false;
    public Transform[] whereToTrack = { };
    private readonly float DISTANCE_TO_TARGET = 3;
    private readonly int TIME_BETWEEN_TRACKS = 5;
    private float timeOfLastTimeTrack = 0;

    /*
    public bool detectSwinging = false;
    public MeshRenderer ground = null;
    private int lastAreaID;
    */

    public bool trackCollisions = false;

    public bool trackPosition = false;
    public float everyXSeconds = 1.0f;
    public int discreteAreaInto = 100;
    public GameObject relativeTo = null;
    private Vector3 sizeOfGround;
    private float timeOfLastPositionTrack = 0;

    // Use this for initialization
    void Start ()
    {
        sizeOfGround = relativeTo.GetComponent<Renderer>().bounds.size;
    }

    // Update is called once per frame
    void Update () {
        if (trackTimes)
        {
            if (Time.time - timeOfLastTimeTrack >= TIME_BETWEEN_TRACKS)
            {
                foreach (Transform splitTimeTracker in whereToTrack)
                {
                    if (Vector3.Distance(this.transform.position, splitTimeTracker.position) <= DISTANCE_TO_TARGET)
                    {
                        timeOfLastTimeTrack = Time.time;
                        log("split time\t" + splitTimeTracker.name + "\t" + timeOfLastTimeTrack);
                    }
                }
            }
        }
        /*
        if (detectSwinging)
        {
            string path = Application.dataPath + "\\Scenes\\maze\\NavMesh.asset";
            log(path);
            Mesh navMesh = (Mesh) Resources.Load(path);
            log(navMesh);

            string[] allPaths = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories);
            foreach (string path2 in allPaths)
            {
                log(path2);
                log(path);
                //Object mesh = Asset
                //print(mesh);
            }
        }
        */
        if (trackPosition)
        {
            if (Time.time - timeOfLastPositionTrack >= everyXSeconds)
            {
                timeOfLastPositionTrack = Time.time;
                int x = (int)Math.Floor(transform.position.x / sizeOfGround.x * discreteAreaInto) + discreteAreaInto/2;
                int z = (int)Math.Floor(transform.position.z / sizeOfGround.z * discreteAreaInto) + discreteAreaInto/2;
                log("position\t" + x + "\t" + z);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (trackCollisions)
        {
            log("collision" + collision.collider.name);
        }
    }

    private void log(object text)
    {
        if (writeToFile)
        {
            string directoryPath = Application.dataPath.Replace("Assets", "") + "Logs/";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            System.IO.File.AppendAllText(directoryPath + "log_" + now.ToString("yyyy'-'MM'-'dd HH'_'mm") + ".txt", text.ToString() + Environment.NewLine);
        }
        else
        {
            print(text);
        }
    }
}
