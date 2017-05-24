using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RundeFahren : MonoBehaviour {
	public Transform Checkpoint;
	public Transform[] checkpoints;
	private int index=0;
	private int zähl=0;
	public NavMeshAgent agent;
	private bool ziel = false;
	public float secs = 0.0f;
	public int Collisionen = 0;
	public bool bestrafen = false;

	// Liste der Checkpoints wird inititalisiert
	void Start () 
	{
		checkpoints = new Transform[Checkpoint.transform.childCount];
		Debug.Log (Checkpoint.transform.childCount);	 
		foreach (Transform child in Checkpoint) 
		{	 
			checkpoints [zähl] = child;
			zähl++;
		}
		NavMeshAgent agent = GetComponent<NavMeshAgent> ();
		agent.autoBraking = true;
		GotoNextPoint ();
	}

	//Funktion zum übergeben des nächsten Ziels
	void GotoNextPoint()
	{
		if (checkpoints.Length == 0) 
		{
			Debug.Log("Keine Elemente");
			return;
		}

		agent.destination = checkpoints[index].position;
		//Debug.Log (index);


	}
	void Update()
	{
		//Abfangen ob das Ziel erreicht wurde, wenn ja --> GoToNextPoint() aufrufen
		if (agent.remainingDistance < 6f && !agent.pathPending) 
		{
			index++;
			//Check ob die Ziellinie durchlaufen wurde, wenn ja --> Zeit anhalten
			if (index == checkpoints.Length) 
			{
				ziel = true;
				index = 0;
			}

			GotoNextPoint ();
		}

	}
	//Stoppuhr
	void FixedUpdate()
	{
		if (!ziel) 
		{
			
			secs = secs + Time.deltaTime;
		}

	}

	void OnCollisionEnter()
	{
		if (!ziel) 
		{
			Collisionen++;
		}
		if (bestrafen) 
		{
			agent.velocity = new Vector3 (0, 0, 0);
		}
	}


}
