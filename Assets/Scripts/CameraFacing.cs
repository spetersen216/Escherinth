using UnityEngine;
using System.Collections;

public class CameraFacing : MonoBehaviour {
	public Camera cameraFacing; 

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		transform.LookAt (cameraFacing.transform.position);
		transform.Rotate (0, 180, 0);
		//transform.position = CameraFacing.transform.position + 
		//	CameraFacing.transform.rotation * Vector3.forward *1.5f;
	}
}
