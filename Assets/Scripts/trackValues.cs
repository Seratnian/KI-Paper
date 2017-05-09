using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class trackValues : MonoBehaviour
{
    private readonly float DISTANCE_TO_TARGET = 3;
    private readonly int TIME_BETWEEN_TRACKS = 5;
    private float timeOfLastTrack = 0;
    public bool trackTimes = false;
    public Transform[] whereToTrack = { };

    public bool detectSwinging = false;
    public MeshRenderer ground = null;
    private int lastAreaID;

    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update () {
        if (trackTimes)
        {
            if (Time.time - timeOfLastTrack >= TIME_BETWEEN_TRACKS)
            {
                foreach (Transform splitTimeTracker in whereToTrack)
                {
                    if (Vector3.Distance(this.transform.position, splitTimeTracker.position) <= DISTANCE_TO_TARGET)
                    {
                        timeOfLastTrack = Time.time;
                        print("split time " + splitTimeTracker.name + ": " + timeOfLastTrack);
                    }
                }
            }
        }
        if (detectSwinging)
        {
            string path = Application.dataPath + "\\Scenes\\maze\\NavMesh.asset";
            print(path);
            Mesh navMesh = (Mesh) Resources.Load(path);
            print(navMesh);

            string[] allPaths = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories);
            foreach (string path2 in allPaths)
            {
                print(path2);
                print(path);
                //Object mesh = Asset
                //print(mesh);
            }
        }
    }
}
