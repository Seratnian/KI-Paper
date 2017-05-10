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

    public bool trackSplitTimes = false;
    public Transform[] whereToTrack = new Transform[5];
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
    public float everyXSeconds = 1;
    public int discreteAreaInto = 100;
    public GameObject area = null;
    private Vector3 sizeOfGround;
    private float timeOfLastPositionTrack = 0;

    public bool trackDistanceToObject = false;
    public GameObject[] distanceTo = new GameObject[1];

    public float everyYSeconds = 1;
    public bool fromCornerToCorner = false;
    private float timeOfLastDistanceTrack = 0;


    // Use this for initialization
    void Start ()
    {
        sizeOfGround = area.GetComponent<Renderer>().bounds.size;
    }

    // Update is called once per frame
    void Update () {
        if (trackSplitTimes)
        {
            if (Time.time - timeOfLastTimeTrack >= TIME_BETWEEN_TRACKS)
            {
                foreach (Transform splitTimeTracker in whereToTrack)
                {
                    if (Vector3.Distance(this.transform.position, splitTimeTracker.position) <= DISTANCE_TO_TARGET)
                    {
                        timeOfLastTimeTrack = Time.time;
                        log("split time", splitTimeTracker.name, + timeOfLastTimeTrack);
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
                log("position", x, z);
            }
        }

        if (trackDistanceToObject)
        {
            if (Time.time - timeOfLastDistanceTrack >= everyYSeconds)
            {
                timeOfLastDistanceTrack = Time.time;
                foreach (GameObject target in distanceTo)
                {
                    Vector3 thisPosition = transform.position;
                    Vector3 targetPosition;
                    if (fromCornerToCorner)
                    {
                        targetPosition = getClosestPoint(target, thisPosition);
                        thisPosition = getClosestPoint(gameObject, targetPosition);
                    }
                    else
                    {
                        targetPosition = target.transform.position;
                    }
                    log("distance", (thisPosition.x - targetPosition.x), (thisPosition.y - targetPosition.y), (thisPosition.z - targetPosition.z));
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (trackCollisions)
        {
            log("collision", collision.collider.name);
        }
    }

    private void log(object text1, object text2 = null, object text3 = null, object text4 = null, object text5 = null, object text6 = null, object text7 = null, object text8 = null)
    {
        object[] objects = { text1, text2, text3, text4, text5, text6, text7, text8 };
        string text = "";

        foreach (object o in objects)
        {
            if (o != null)
            {
                text += o.ToString().PadRight(16);
            }
        }
        text += Environment.NewLine;

        if (writeToFile)
        {
            string directoryPath = Application.dataPath.Replace("Assets", "") + "Logs/";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            System.IO.File.AppendAllText(directoryPath + "log_" + now.ToString("yyyy'-'MM'-'dd HH'_'mm") + ".txt", text);
        }
        else
        {
            print(text);
        }
    }

    private Vector3 getClosestPoint(GameObject from, Vector3 to)
    {
        Vector3 fromPosition;
        if (from.GetComponent<Collider>() != null)
        {
            fromPosition = from.GetComponent<Collider>().ClosestPoint(to);
        }
        else if (from.GetComponent<Renderer>() != null)
        {
            fromPosition = from.GetComponent<Renderer>().bounds.ClosestPoint(to);
        }
        else
        {
            fromPosition = from.transform.position;
        }
        return fromPosition;
    }
}
