using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MoveTo : MonoBehaviour 
{

	public Transform goal;
	public NavMeshAgent agent;
	private bool ziel = false;
	public float secs = 0.0f;
	void Start () 
	{
		NavMeshAgent agent = GetComponent<NavMeshAgent> ();
		agent.autoBraking = true;
		GotoPoint ();
			
	}

	void GotoPoint()
	{
		
		agent.destination = goal.position;

	}
	void Update()
	{
		if (agent.remainingDistance < 0.5f && !agent.pathPending) 
		{
			ziel = true;

		}
	}

	void FixedUpdate()
	{
		if (!ziel) 
		{
			secs = secs + Time.deltaTime;
		}
	}


}
	

