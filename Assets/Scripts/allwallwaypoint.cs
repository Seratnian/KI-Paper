using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class allwallwaypoint : MonoBehaviour 
{
	public Transform Walls;
	public Transform[] walls;
	public int index=0;
	public NavMeshAgent agent;
	private int zähl=0;
	void Start () 
	{
		walls = new Transform[Walls.transform.childCount];
		Debug.Log (Walls.transform.childCount);	 
		foreach (Transform child in Walls) 
		{	 
			walls [zähl] = child;
			zähl++;
		}
		NavMeshAgent agent = GetComponent<NavMeshAgent> ();
		agent.autoBraking = false;
		GotoNextPoint ();
	}

	void GotoNextPoint()
	{
		if (walls.Length == 0) 
		{
			return;
		}


		index = (int)(Random.value* Walls.transform.childCount);
		agent.destination = walls[index].position;
		Debug.Log (index);
	}
	void Update()
	{
		if (agent.remainingDistance < 0.5f && !agent.pathPending) 
		{
			GotoNextPoint ();
		}
	}


}


