﻿using UnityEngine;
using System.Collections;

public class RunTime:MonoBehaviour {

	private LightSystem lights;
	private GameObject door;
	private Key key;
	public Material skyboxMaterial;
	private float radius;
	private MazeStructure mazeStruct;


	public void Init(LightSystem lights, GameObject door, Key key, Material mat, float radius, MazeStructure mazeStruct) {
		this.lights = lights;
		this.door = door;
		this.key = key;
		this.radius = radius;
		this.mazeStruct = mazeStruct;
		//transform.position = MazeStructure.Vector3FromCubeToSphere(mazeStruct.GetStart()[0].ToVector3(), 
		//mazeStruct.length, mazeStruct.GetStart()[0].ToVector3(), radius);
		transform.position = Vector3.up*radius + Vector3.right*5 + Vector3.forward*5;
		//transform.localRotation = ; 
		skyboxMaterial = mat;
		mat.SetColor("_Tint", new Color32((byte)128, (byte)128, (byte)128, (byte)128));
		//GetComponent<OVRPlayerController> ().GetComponentInChildren<Light> ().enabled = false;
	}

	void Update() {
		// calculate up, forwards and right
		Vector3 up = -transform.position.normalized;
		Vector3 forwards = Vector3.RotateTowards(up, transform.forward, Mathf.PI/2, 1).normalized;
		if (Vector3.Angle(up, forwards)<90)
			forwards = Vector3.RotateTowards(-up, transform.forward, Mathf.PI/2, 1).normalized;
		Vector3 rights = Vector3.Cross(forwards, up).normalized;

		// handle camera angle (from mouse movement)
		if (Input.GetAxis("Mouse X")>0)
			forwards = Vector3.RotateTowards(forwards, -rights, Input.GetAxis("Mouse X")/4, 1);
		else
			forwards = Vector3.RotateTowards(forwards, rights, -Input.GetAxis("Mouse X")/4, 1);
		rights = Vector3.Cross(forwards, up).normalized;
		//transform.rotation.SetFromToRotation(-transform.position, forwards);

		//Quaternion.RotateTowards(Quaternion.Euler(Vector3.zero),
		//transform.Rotate(-transform.position, -Input.GetAxis("Mouse X")*10);
		//transform.Rotate(transform.right.normalized, Input.GetAxis("Mouse Y")*10);

		// sum the WASD/Arrows movement
		float forward = 0, right = 0;
		if (Input.GetKey(KeyCode.W)  || Input.GetKey(KeyCode.UpArrow))
			forward += 0.4f;
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
			forward -= 0.4f;
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			right += 0.4f;
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			right -= 0.4f;

		// calculate where to move, then move
		Vector3 dest = transform.position + forward*forwards + right*transform.right.normalized;
		
		rigidbody.MovePosition(dest.normalized * (radius - 5));
		//rigidbody.MovePosition(rigidbody.position+rigidbody.velocity*2);
		//rigidbody.velocity = Vector3.zero;
		rigidbody.velocity/= 2;
		rigidbody.
		// aim the rotation forwards
		transform.localRotation = Quaternion.LookRotation(forwards, -transform.position);
	}


	// The OnTriggerEnter function is called when the collider attached to this game object (whatever object the script is attached to) overlaps another collider set to be a "trigger"
	void OnTriggerEnter(Collider collider) {
		Debug.Log (collider.name);

		// We want to check if the thing we're colliding with is a collectable, this will differentiate it from other trigger objects which we might add in the future
		if (collider.GetComponent<Key>() == key) {
			GameObject.Find("CenterLight(Clone)").GetComponent<Light>().intensity = 0.1f;
			door.SetActive(false);
			collider.gameObject.SetActive(false);
			lights.keyTime = 0;
			print(skyboxMaterial.GetColor("_Tint"));
			skyboxMaterial.SetColor("_Tint", new Color32((byte)44, (byte)28, (byte)53, (byte)128));
			//gameObject.GetComponent<OVRPlayerController>().Acceleration = 0.3f;
			gameObject.GetComponentInChildren<Light>().enabled = true;
			gameObject.GetComponentInChildren<LightFlicker>().enabled = true;

			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.transform.localScale = new Vector3(90, 90, 90);
			sphere.renderer.material = new Material(Shader.Find("Transparent/Diffuse"));
			sphere.renderer.material.color = new Color(1, 1, 1, 0.8f);
		}
	}
}
