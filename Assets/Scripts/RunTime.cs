using UnityEngine;
using System.Collections;

public class RunTime : MonoBehaviour {

	private LightSystem lights;
	private GameObject door;
	private Key key;
	public Material skyboxMaterial;
	private float radius;
	private MazeStructure mazeStruct;


	public void Init(LightSystem lights, GameObject door, Key key, Material mat, float radius, MazeStructure mazeStruct)
	{
		this.lights = lights;
		this.door = door;
		this.key = key;
		this.radius = radius;
		this.mazeStruct = mazeStruct;
		//transform.position = MazeStructure.Vector3FromCubeToSphere(mazeStruct.GetStart()[0].ToVector3(), 
			//mazeStruct.length, mazeStruct.GetStart()[0].ToVector3(), radius);
		transform.position = Vector3.up*radius;
		//transform.localRotation = ; 
		skyboxMaterial = mat;
		mat.SetColor("_Tint", new Color32 ((byte)128,(byte)128,(byte)128,(byte)128));
		//GetComponent<OVRPlayerController> ().GetComponentInChildren<Light> ().enabled = false;
	}

	void Update(){
		// handle camera angle (from mouse movement)
		transform.Rotate(transform.up, -Input.GetAxis("Mouse X")*10);
		transform.Rotate(transform.right, Input.GetAxis("Mouse Y")*10);

		// calculate forwards
		Vector3 forwards = Vector3.RotateTowards(-transform.position, transform.forward, Mathf.PI/2, 1).normalized;
		if (Vector3.Angle(-transform.position, forwards)<90)
			forwards = Vector3.RotateTowards(transform.position, transform.forward, Mathf.PI/2, 1).normalized;

		// sum the WASD/Arrows movement
		float forward = 0, right = 0;
		if(Input.GetKey(KeyCode.W)  || Input.GetKey(KeyCode.UpArrow))
			forward += 0.1f;
		else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
			forward -= 0.1f;
		else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			right += 0.1f;
		else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			right -= 0.1f;

		// calculate where to move, then move
		Vector3 dest = transform.position + forward*forwards + right*transform.right.normalized;
		transform.position = mazeStruct.Move(transform.position, dest).normalized * (radius-5);

		// aim the rotation forwards
		transform.localRotation = Quaternion.LookRotation(forwards, -transform.position);
	}


	// The OnTriggerEnter function is called when the collider attached to this game object (whatever object the script is attached to) overlaps another collider set to be a "trigger"
	void OnTriggerEnter(Collider collider)
	{
		// We want to check if the thing we're colliding with is a collectable, this will differentiate it from other trigger objects which we might add in the future
		if (collider.GetComponent<Key>() == key)
		{
			/*foreach(Light l in lights.cells)
			{
				l.GetComponent<LightFlicker>().stopFlicker();
			}*/
			//Debug.Log("Collision with collection");
			door.SetActive(false);
			collider.gameObject.SetActive(false);
			lights.keyTime = 0;
			print (skyboxMaterial.GetColor("_Tint"));
			skyboxMaterial.SetColor("_Tint", new Color32((byte)44, (byte)28, (byte)53, (byte)128));
			gameObject.GetComponent<OVRPlayerController>().Acceleration = 0.3f;
			gameObject.GetComponentInChildren<Light>().enabled = true;
			gameObject.GetComponentInChildren<LightFlicker>().enabled = true;

		}
	}
}
