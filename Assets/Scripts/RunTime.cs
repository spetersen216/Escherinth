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
		transform.position = MazeStructure.Vector3FromCubeToSphere(mazeStruct.GetStart()[0].ToVector3(), 
		                                                           mazeStruct.length, mazeStruct.GetStart()[0].ToVector3(), radius);
		//transform.localRotation = ; 
		skyboxMaterial = mat;
		mat.SetColor("_Tint", new Color32 ((byte)128,(byte)128,(byte)128,(byte)128));
		//GetComponent<OVRPlayerController> ().GetComponentInChildren<Light> ().enabled = false;
	}

	void Update(){

		/*Vector3 angle = transform.eulerAngles;
		angle.y += Input.GetAxis("Mouse X")*10;
		angle.x -= Input.GetAxis("Mouse Y")*10;
		transform.eulerAngles = angle;*/


		float forward = 0, right = 0;
	
		if(Input.GetKey(KeyCode.W)  || Input.GetKey(KeyCode.UpArrow)){
			forward += 1;
		}else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
			right += 1;
		}else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
			right += -1;
		}else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
			forward += -1;
		}

		Vector3 dest = forward * transform.forward + right * transform.right;
		
		transform.position = mazeStruct.Move (transform.position, dest).normalized * (radius - 0);
		//transform.localRotation = Quaternion.LookRotation (transform.forward, -transform.position);

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
