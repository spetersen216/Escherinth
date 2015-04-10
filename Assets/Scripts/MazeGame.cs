using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MazeGame:MonoBehaviour {

	public LightSystem lights;

	public MazeTool top;
	public MazeTool bottom;
	public MazeTool left;
	public MazeTool right;
	public MazeTool back;
	public MazeTool front;

	public Mesh cellFloor;
	public Mesh[] cellWalls;
	public Mesh[] cellWallTops;
	public Material cellFloorMat;
	public Material cellWallMat;
	public Material cellWallTopMat;

	private float angle;
	public Material skyboxMaterial;
	public float radius;
	public Key key;
	public GameObject door;
	//private GameObject player_collider;
	public MazeStructure mazeStruct;
	public GameObject left_cam;
	public GameObject right_cam;
	public AnimationCurve lightFlicker;
	public Monster monster;
	private MazeCell[, ,] cells;
	private GameObject mazeSphere;
	public Light cLight;
	public AudioClip lightsOutInit;
	public AudioClip lightOff;
	private Rigidbody playerRigid;

	public Lamp lamp;
	private GameObject lantern;

	// Use this for initialization
	void Start() {
		// initialize all MazeTools
		MazeTool[] tools = { top, bottom, left, right, back, front };
		for (int i=0; i<tools.Length; i++) {
			tools[i].Start();
			tools[i].gameObject.SetActive(false);
		}

		
		// initialize the MazeStructure
		mazeStruct = new MazeStructure(top, bottom, left, right, front, back, radius);
		cells = mazeStruct.MakeCells(cellFloor, cellWalls, cellWallTops,
			cellFloorMat, cellWallMat, cellWallTopMat, lightFlicker, radius);

		// initialize this
		transform.position = mazeStruct.GetStartSphere();
		skyboxMaterial = Resources.Load<Material>("Overcast2 Skybox");
		skyboxMaterial.SetColor("_Tint", new Color32((byte)128, (byte)128, (byte)128, (byte)128));
		GetComponent<OVRCameraRig>().Init();
		this.playerRigid = gameObject.AddComponent<Rigidbody> ();
		this.playerRigid.freezeRotation = true;
		Physics.gravity = Vector3.zero;
		SphereCollider collider = gameObject.AddComponent<SphereCollider>();
		collider.material = (PhysicMaterial)Resources.Load("WallPhysics", typeof(PhysicMaterial));
		collider.radius = 1.5f;
		collider.center = new Vector3(0, .4f, 0);

		// initialize other objects
		Vector3 keyPos = mazeStruct.FindKeySphere().normalized*(radius-2.5f);
		Quaternion keyRot = Quaternion.LookRotation(Vector3.Cross(-keyPos, Vector3.one), -keyPos);
		key = (Key)Instantiate(key, keyPos, keyRot);
		cLight = ((GameObject)Instantiate(cLight.gameObject, cLight.gameObject.transform.localPosition, Quaternion.identity)).GetComponent<Light>();
		lights = ((GameObject)Instantiate(lights.gameObject, new Vector3(85.4f, 100f, 100f), Quaternion.identity)).GetComponent<LightSystem>();

		lamp = ((GameObject)Instantiate(lamp.gameObject, lamp.gameObject.transform.localPosition, Quaternion.identity)).GetComponent<Lamp>();
		lamp.transform.rotation = Quaternion.Euler(70.7834f,342.207f,321.425f);
		lamp.gameObject.SetActive (false);

		lantern = GameObject.Find("Lantern");
		lantern.SetActive(false);

		// create and initialize player and monster
		//print("Monster: "+monster);
		monster = (Monster)Instantiate(monster, new Vector3(4.79f, 47.36f, -.038f), Quaternion.identity);
		AudioSource src = gameObject.AddComponent<AudioSource>();
		src.clip = lightsOutInit;
		monster.gameObject.SetActive(false);
		angle = Mathf.PI/2;

		AudioSource src1 = gameObject.AddComponent<AudioSource>();
		src1.clip = lightOff;
		lights.Init(mazeStruct, cells, src1);

	}

	void Update() {
		// calculate up, forwards and right
		Vector3 up = -transform.position.normalized;
		Vector3 forwards = Vector3.RotateTowards(up, transform.forward, Mathf.PI/2, 1).normalized;
		if (Vector3.Angle(up, forwards)<90)
			forwards = Vector3.RotateTowards(-up, transform.forward, Mathf.PI/2, 1).normalized;
		Vector3 rights = Vector3.Cross(forwards, up).normalized;

		// handle left-right camera movement (from mouse)
		if (Input.GetAxis("Mouse X")>0)
			forwards = Vector3.RotateTowards(forwards, -rights, Input.GetAxis("Mouse X")/4, 1);
		else
			forwards = Vector3.RotateTowards(forwards, rights, -Input.GetAxis("Mouse X")/4, 1);
		rights = Vector3.Cross(forwards, up).normalized;

		// sum the WASD/Arrows movement
		float forward = 0, right = 0;
		if (Input.GetKey(KeyCode.W)  || Input.GetKey(KeyCode.UpArrow))
			forward += 1;
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
			forward -= 1;
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			right += 1;
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			right -= 1;


		// calculate where to move, then move
		Vector3 dest = transform.position + (forward*forwards + right*transform.right.normalized).normalized*0.4f;
		this.playerRigid.velocity = (dest-transform.position)/Time.fixedDeltaTime;
		this.playerRigid.MovePosition(this.playerRigid.position.normalized*(radius-3.5f));

		// handle up-down camera movement (from mouse, from sphere)
		if (angle<Mathf.PI/2)
			forwards = Vector3.RotateTowards(up, forwards, angle, 1);
		else
			forwards = Vector3.RotateTowards(forwards, -up, angle-Mathf.PI/2, 1);
		transform.localRotation = Quaternion.LookRotation(forwards, -transform.position);
		angle = Mathf.Max(Mathf.Min(angle-Input.GetAxis("Mouse Y")/4, 0.99f*Mathf.PI), 0.01f*Mathf.PI);
	}

	// The OnTriggerEnter function is called when the collider attached to this game object (whatever object the script is attached to) overlaps another collider set to be a "trigger"
	void OnTriggerEnter(Collider collider) {
		//Debug.Log(collider.name);

		// We want to check if the thing we're colliding with is a collectable, this will differentiate it from other trigger objects which we might add in the future
		if (collider.GetComponent<Key>() == key) {
			GameObject.Find("CenterLight(Clone)").GetComponent<Light>().intensity = 0.2f;
			mazeStruct.RemoveDoor();
			collider.gameObject.SetActive(false);
			lights.keyTime = 0;
			//print(skyboxMaterial.GetColor("_Tint"));
			skyboxMaterial.SetColor("_Tint", new Color32((byte)44, (byte)28, (byte)53, (byte)128));
			//gameObject.GetComponent<OVRPlayerController>().Acceleration = 0.3f;
			//gameObject.GetComponentInChildren<Light>().enabled = true;
			//gameObject.GetComponentInChildren<LightFlicker>().enabled = true;
			this.lamp.gameObject.SetActive(true);
			monster.Init(mazeStruct, cells, transform, mazeStruct.GetStartSphere());

			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.transform.localScale = new Vector3(96.8f, 96.8f, 96.8f);
			sphere.renderer.material = new Material(Shader.Find("Transparent/Diffuse"));
			sphere.renderer.material.color = new Color(1, 1, 1, 0.8f);
		}
		if (collider.GetComponent<Lamp>() == lamp) {
			GameObject.Find("CenterLight(Clone)").GetComponent<Light>().intensity = 0.0f;
			collider.gameObject.SetActive(false);
			this.lantern.SetActive(true);
			Light[] l = this.lantern.GetComponentsInChildren<Light>();

			GameObject.Find("Sphere").renderer.material.color = new Color(0,0,0,.8f);

			foreach(Light light in l){
				if(light.enabled == false){
					light.enabled = true;
				}
				if(light.type == LightType.Directional){
					light.transform.rotation = gameObject.transform.rotation;
				}
			}



		}

	}
}