using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class TrackValues : MonoBehaviour
{
    public bool debug = false;
    public bool writeToFile = true;
    private string logFile;
    private FileStream logFileStream;
    public bool trackContinuously = true;
    private DateTime now = DateTime.Now;
    public static readonly string CSV_SEPARATOR = ",";

    private bool trackPosition = false;
    private float everyXSeconds = 1;
    public int discreteAreaInto = 100;
    public GameObject area = null;
    private Vector3 sizeOfGround;
    private float timeOfLastPositionTrack = 0;

    public bool trackSplitTimes = false;
    public Transform[] whereToTrack = new Transform[5];
    private readonly float DISTANCE_TO_TARGET = 3;
    private readonly int TIME_BETWEEN_TRACKS = 5;
    private float timeOfLastTimeTrack = 0;
    
    public bool trackCollisions = true;
    private string collisionName = "";
	private int collisionx=0;
	private int collisiony=0;

    public bool trackDistanceToObject = true;
    public GameObject walls = null;
    private GameObject[] distanceTo = new GameObject[1];
    private float everyYSeconds = 1;
    public bool fromCornerToCorner = true;
    private float timeOfLastDistanceTrack = 0;

    public bool trackSpeed = true;
    private Vector2 prevPositiion;


    // Use this for initialization
    void Start ()
    {
        sizeOfGround = area.GetComponent<Renderer>().bounds.size;
        if (trackContinuously)
        {
            if (writeToFile)
            {
				
				logFile = Application.dataPath.Replace("Assets", "") + "Logs/" + "log_" + now.ToString("yyyy'-'MM'-'dd HH'_'mm'_'") +UnityEngine.Random.Range(0,10000)+".csv";
                logFileStream = File.Create(logFile);
                Write("sep=" + CSV_SEPARATOR);
                List<string> titles = new List<string>();
                titles.Add("Time");
                titles.Add("x-Position");
                titles.Add("y-Position");
				//if (trackCollisions) titles.Add("Collision");
                if (trackSplitTimes) titles.Add("Checkpoint");
				if (trackCollisions) titles.Add("Collisionx");
				if (trackCollisions) titles.Add("Collisiony");

                
                Write(separate(titles.ToArray()));
            }
            if (trackDistanceToObject)
            {
                foreach (Transform wall in walls.transform)
                {
                    wall.name = positionToName(calculatePosition(wall));
                }
            }
        }
        prevPositiion = transform.position;
    }

    private void OnDestroy()
    {
        if (writeToFile) logFileStream.Close();
    }

    private void Write(string text)
    {
        Byte[] textBytes = new UTF8Encoding(true).GetBytes(text + Environment.NewLine);
        logFileStream.Write(textBytes, 0, textBytes.Length);
    }

    private void FixedUpdate()
    {
        if (trackContinuously)
        {
            //float distance = float.MaxValue, string collision = null, string splitTime = null
            Vector2 position = calculatePosition(transform);
            Tracker tracker = new Tracker(Time.time, position, debug);
            if (trackSplitTimes) tracker.SplitTime = checkSplitTime();
            if (trackCollisions) tracker.Collision = checkCollision();
			if (trackCollisions) tracker.Trackcollx= checkCollisionx();
			if (trackCollisions) tracker.Trackcolly= checkCollisiony();
            if (trackSpeed) tracker.Speed = getSpeed();
            if (trackDistanceToObject) tracker.Distance = calculateDistance(position);
            
            log(tracker.ToString());
        }
    }

    private float getSpeed()
    {
        float speed = new Vector2(transform.position.x - prevPositiion.x, transform.position.z - prevPositiion.y).magnitude;
        prevPositiion = transform.position;
        return speed;
    }

    private string checkCollision()
    {
        string collision = collisionName;
        collisionName = "";
        return collision;
    }
	private int checkCollisionx()
	{
		int collx = collisionx;
		collisionx=0;
		return collx;
		
	}
	private int checkCollisiony()
	{
		int colly = collisiony;
		collisiony=0;
		return colly;

	}

    private string checkSplitTime()
    {
        foreach (Transform splitTimeTracker in whereToTrack)
        {
            if (Vector3.Distance(this.transform.position, splitTimeTracker.position) <= DISTANCE_TO_TARGET)
            {
				Debug.Log (splitTimeTracker.name);
				return splitTimeTracker.name;
            }
        }
        return "";
    }

    private float calculateDistance(Vector2 position, int discreteDistance = 1)
    {
        List<string> wallNames = new List<string>();
        if (discreteDistance == 1) wallNames.Add(positionToName(position));
        for (int x = -discreteDistance; x <= discreteDistance; x++)
        {
            for (int y = -discreteDistance; y <= discreteDistance; y++)
            {
                if (Math.Abs(x) == discreteDistance || Math.Abs(y) == discreteDistance)
                {
                    int finalX = (int)position.x + x;
                    int finalY = (int)position.y + y;
                    if (finalX >= 0 && finalX < discreteAreaInto && finalY >= 0 && finalY < discreteAreaInto)
                        wallNames.Add(positionToName(finalX, finalY));
                }
            }
        }

        List<float> distances = new List<float>();
        foreach(Transform wall in walls.transform)
        {
            if (wallNames.Exists(x => x == wall.name))
            {
                distances.Add(calculateDistance(gameObject, wall.gameObject).magnitude);
            }
        }
        distances.Sort();
        if (distances.Count > 0) return distances[0];
        return calculateDistance(position, discreteDistance + 1);
    }

    private string positionToName(int x, int y)
    {
        return "Wall_" + x.ToString().PadLeft(2, '0') + "_" + y.ToString().PadLeft(2, '0');
    }

    private string positionToName(Vector2 position)
    {
        return positionToName((int) position.x, (int) position.y);
    }


    private Vector2 calculatePosition(Transform of)
    {
        Vector2 position = new Vector2();
		position.x = (int)Math.Floor (of.position.x / sizeOfGround.x * discreteAreaInto);
		position.y = (int)Math.Floor (of.position.z / sizeOfGround.z * discreteAreaInto);
        return position;
    }
	public int collx(Transform of)
	{
		int positionx = (int)Math.Floor (of.position.x / sizeOfGround.x * discreteAreaInto);
		return positionx;
	}
	public int colly(Transform of)
	{
		int positiony = (int)Math.Floor (of.position.z / sizeOfGround.z * discreteAreaInto);
		return positiony;
	}
    // Update is called once per frame
    void Update()
    {
        if (!trackContinuously)
        {
            if (trackSplitTimes)
            {
                if (Time.time - timeOfLastTimeTrack >= TIME_BETWEEN_TRACKS)
                {
                    foreach (Transform splitTimeTracker in whereToTrack)
                    {
                        if (Vector3.Distance(this.transform.position, splitTimeTracker.position) <= DISTANCE_TO_TARGET)
                        {
                            timeOfLastTimeTrack = Time.time;
                            log("split time", splitTimeTracker.name, +timeOfLastTimeTrack);
                        }
                    }
                }
            }
            if (trackPosition)
            {
                if (Time.time - timeOfLastPositionTrack >= everyXSeconds)
                {
                    timeOfLastPositionTrack = Time.time;
                    Vector2 position = calculatePosition(transform);
                    log("position", position.x, position.y);
                }
            }

            if (trackDistanceToObject)
            {
                if (Time.time - timeOfLastDistanceTrack >= everyYSeconds)
                {
                    timeOfLastDistanceTrack = Time.time;
                    foreach (GameObject target in distanceTo)
                    {
                        Vector3 distance = calculateDistance(gameObject, target);
                        Debug.Log(distance.x+" "+distance.y+" "+distance.z);
                    }
                }
            }
            if (trackCollisions && collisionName != "")
            {
                log("Collision", collisionName);
                collisionName = "";
            }
        }
    }

    private Vector3 calculateDistance(GameObject gameObject, GameObject target)
    {
        Vector3 thisPosition = transform.position;
        Vector3 targetPosition;
        if (fromCornerToCorner)
        {
            targetPosition = calculateClosestPoint(target, thisPosition);
            thisPosition = calculateClosestPoint(gameObject, targetPosition);
        }
        else
        {
            targetPosition = target.transform.position;
        }
        return new Vector3((thisPosition.x - targetPosition.x), (thisPosition.y - targetPosition.y), (thisPosition.z - targetPosition.z));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (trackCollisions)
        {
			collisionx = collx (collision.transform);
			collisiony = colly (collision.transform);
            collisionName = collision.collider.name;
			//Debug.Log ("Collision");
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
                text += o.ToString() + ",";
            }
        }
        text = text.Substring(0, text.Length - 1);

        if (writeToFile)
        {
            Write(text);
        }
        else
        {
            print(text);
        }
    }

    private Vector3 calculateClosestPoint(GameObject from, Vector3 to)
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
    public static string separate(string string1, string string2 = null, string string3 = null, string string4 = null, string string5 = null, string string6 = null, string string7 = null, string string8 = null)
    {
        string[] strings = { string1, string2, string3, string4, string5, string6, string7, string8 };
        return separate(strings);
    }
    public static string separate(string[] strings)
    {
        string textToReturn = "";
        foreach (string text in strings)
        {
            if (text == null)
            {
                break;
            }
            textToReturn += text + CSV_SEPARATOR;
        }
        return textToReturn.Substring(0, textToReturn.Length - 1);
    }
}

class Tracker
{
    private float time;
    private Vector2 position;
    private bool debug;
    private float distance;
    private string collision;
    private string splitTime;
	private int trackcollx;
	private int trackcolly;
    private float speed;

	public Tracker(float time, Vector2 position, bool debug, float speed = float.MinValue, float distance = float.MaxValue, string collision = null, string splitTime = null)
    {
        this.time = time;
        this.position = position;
        this.debug = debug;
        this.distance = distance;
        this.collision = collision;
        this.speed = speed;
		this.trackcollx = trackcollx;
		this.trackcolly = trackcolly;
        this.splitTime = splitTime;
    }
    public Tracker(float time, Vector2 position, float speed = float.MinValue, float distance = float.MaxValue, string collision = null, string splitTime = null)
    : this(time, position, false, speed, distance, collision, splitTime) { }

    public float Distance
    {
        set
        {
            distance = value;
        }
    }

    public string Collision
    {
        set
        {
            collision = value;
        }
    }
	public int Trackcollx
	{
		set
		{
			trackcollx = value;
		}
	}
	public int Trackcolly
	{
		set
		{
			trackcolly = value;
		}
	}

    public string SplitTime
    {
        set
        {
            splitTime = value;
        }
    }

    public float Speed
    {
        get
        {
            return speed;
        }

        set
        {
            speed = value;
        }
    }

    public override string ToString()
    {
        List<string> strings = new List<string>();
        strings.Add((debug ? "Time: " : "") + time);
        strings.Add((debug ? "Position: " : "") + TrackValues.separate(position.x.ToString(), position.y.ToString()));
 //       if (collision != null) strings.Add((debug ? "Collision: " : "") + collision);
        if (splitTime != null) strings.Add((debug ? "splitTime: " : "") + splitTime);
		strings.Add((debug ? "Collisionx: " : "") + trackcollx);
		strings.Add((debug ? "Collisiony: " : "") + trackcolly);
        strings.Add(speed.ToString());
        return TrackValues.separate(strings.ToArray());
    }
}
