using UnityEngine;
using System.Collections;

public class Navigate : MonoBehaviour {
	public Transform goal;
	private NavMeshAgent agent;
	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
	}

	public void SetDestination(Transform dest){
		goal = dest;
		agent.destination = dest.position;
	}

}
