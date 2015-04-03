using UnityEngine;
using System.Collections;

public class Player:MonoBehaviour {

	/*private LightSystem lights;
	private GameObject door;
	private Key key;
	public Material skyboxMaterial;
	private float radius;
	private MazeStructure mazeStruct;
	private Monster monster;
	private MazeCell[,,] cells;
	private Lamp lamp;
	private GameObject lantern;

	public void Init(LightSystem lights, MazeCell[,,] cells, GameObject door, Key key, Material mat, float radius, MazeStructure mazeStruct, Monster monster, AudioSource lightsOutInit, Lamp lamp, GameObject lantern) {
		this.lights = lights;
		this.cells = cells;
		this.door = door;
		this.key = key;
		this.radius = radius;
		this.monster = monster;
		this.mazeStruct = mazeStruct;
		this.lamp = lamp;
		this.lantern = lantern;
		// initialize this
		//transform.position = MazeStructure.Vector3FromCubeToSphere(mazeStruct.GetStart()[0].ToVector3(), 
		//mazeStruct.length, mazeStruct.GetStart()[0].ToVector3(), radius);
		transform.position = Vector3.up*radius + Vector3.right*5 + Vector3.forward*5;
		skyboxMaterial = mat;
		mat.SetColor("_Tint", new Color32((byte)128, (byte)128, (byte)128, (byte)128));

		// initialize other components
		GetComponent<OVRCameraRig>().Init();
		gameObject.AddComponent<Rigidbody>().useGravity = false;
		//GetComponentInChildren<LightFlicker>().enabled = false;
		//GetComponentInChildren<Light>().enabled = false;
		CapsuleCollider collider = gameObject.AddComponent<CapsuleCollider> ();
		collider.material = (PhysicMaterial)Resources.Load ("WallPhysics", typeof(PhysicMaterial));
		collider.height = 2;
		collider.center = new Vector3 (0, .4f, 0);
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
		Vector3 dest = transform.position + (forward*forwards + right*transform.right.normalized).normalized;
		
		rigidbody.MovePosition(dest.normalized * (radius - 3.5f));
		//rigidbody.MovePosition(rigidbody.position+rigidbody.velocity*2);
		//rigidbody.velocity = Vector3.zero;
		rigidbody.velocity*= 0.9f;
		rigidbody.
		// aim the rotation forwards
		transform.localRotation = Quaternion.LookRotation(forwards, -transform.position);
	}


	// The OnTriggerEnter function is called when the collider attached to this game object (whatever object the script is attached to) overlaps another collider set to be a "trigger"
	void OnTriggerEnter(Collider collider) {
		Debug.Log (collider.name);

		// We want to check if the thing we're colliding with is a collectable, this will differentiate it from other trigger objects which we might add in the future
		if (collider.GetComponent<Key>() == key){
			GameObject.Find("CenterLight(Clone)").GetComponent<Light>().intensity = 0.1f;
			door.SetActive(false);
			collider.gameObject.SetActive(false);
			lights.keyTime = 0;
			print(skyboxMaterial.GetColor("_Tint"));
			skyboxMaterial.SetColor("_Tint", new Color32((byte)44, (byte)28, (byte)53, (byte)128));
			//gameObject.GetComponent<OVRPlayerController>().Acceleration = 0.3f;
			//gameObject.GetComponentInChildren<Light>().enabled = true;
			//gameObject.GetComponentInChildren<LightFlicker>().enabled = true;
			this.lamp.gameObject.SetActive(true);
			monster.Init(mazeStruct, cells, transform, mazeStruct.GetStartSphere());

			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.transform.localScale = new Vector3(95, 95, 95);
			sphere.renderer.material = new Material(Shader.Find("Transparent/Diffuse"));
			sphere.renderer.material.color = new Color(1, 1, 1, 0.8f);
		}
		if(collider.GetComponent<Lamp>() == lamp){
			collider.gameObject.SetActive(false);
			this.lantern.SetActive(true);
		}

	}*/
}
