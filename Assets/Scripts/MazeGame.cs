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

	public float playerSpeed=0.3f;
	public float playerRunSpeed=0.4f;
	public float playerHeight=3.5f;
	public float monsterSpeed=1f;
	public float monsterHeight=3.5f;
	public float keyHeight=2.5f;

	public Mesh cellFloor;
	public Mesh[] cellWalls;
	public Mesh[] cellWallTops;
	public Material cellFloorMat;
	public Material cellWallMat;
	public Material cellWallTopMat;

	private float angle;
	public Material skyboxMaterial;
	public float radius;
	public bool is3D=true;
	public bool isScary=true;
	public Key key;
	//private GameObject player_collider;
	public MazeStructure mazeStruct;
	public GameObject left_cam;
	public GameObject right_cam;
	public AnimationCurve lightFlicker;
	public Monster monster;
	public GameObject torchObj;
	public GameObject doorObj;
	private MazeCell[, ,] cells;
	private GameObject mazeSphere;
	public Light cLight;
	public AudioClip lightsOutInit;
	public AudioClip lightOff;
	private Main main;

	public Lamp lamp;
	private GameObject lantern;

	public void Init(Main main) {
		this.main = main;

		// initialize all MazeTools (from prefabs)
		MazeTool[] tools = { top, bottom, left, right, front, back };
		for (int i=0; i<tools.Length; i++) {
			tools[i] = (MazeTool)Instantiate(tools[i]);
			tools[i].Start();
			tools[i].gameObject.SetActive(false);
		}

		
		// initialize the MazeStructure
		mazeStruct = new MazeStructure(tools[0], tools[1], tools[2], tools[3], tools[4], tools[5], radius, is3D, torchObj);
		cells = mazeStruct.MakeCells(cellFloor, cellWalls, cellWallTops,
			cellFloorMat, cellWallMat, cellWallTopMat, doorObj, lightFlicker, radius);

		// initialize this
		transform.position = mazeStruct.GetStartSphere();
		skyboxMaterial = Resources.Load<Material>("Overcast2 Skybox");
		skyboxMaterial.SetColor("_Tint", new Color32((byte)128, (byte)128, (byte)128, (byte)128));
		GetComponent<OVRCameraRig>().Init();
		Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
		rigidbody.freezeRotation = true;
		Physics.gravity = Vector3.zero;
		SphereCollider collider = gameObject.AddComponent<SphereCollider>();
		collider.material = (PhysicMaterial)Resources.Load("WallPhysics", typeof(PhysicMaterial));
		collider.radius = 1.5f;
		collider.center = new Vector3(0, .4f, 0);

		// initialize other objects
		Vector3 keyPos = mazeStruct.FindKeySphere().normalized*(radius-keyHeight);
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
		monster = (Monster)Instantiate(monster, new Vector3(4.79f, 47.36f, -.038f), Quaternion.identity);
		AudioSource src = gameObject.AddComponent<AudioSource>();
		src.clip = lightsOutInit;
		monster.gameObject.SetActive(false);
		angle = Mathf.PI/2;

		AudioSource src1 = gameObject.AddComponent<AudioSource>();
		src1.clip = lightOff;
		lights.Init(mazeStruct, cells, src1);

	}

	void FixedUpdate() {
		if (rigidbody==null) {
			print("unity hasn't set the rigidbody yet");
			return;
		}

		if (is3D) {
			// calculate up, forwards and right
			Vector3 up = -transform.position.normalized;
			Vector3 forwards = Vector3.RotateTowards(up, transform.Find("LeftEyeAnchor").forward, Mathf.PI/2, 1).normalized;
			if (Vector3.Angle(up, forwards)<90)
				forwards = Vector3.RotateTowards(-up, transform.forward, Mathf.PI/2, 1).normalized;
			Vector3 right = Vector3.Cross(forwards, up).normalized;

			// handle left-right camera movement (from keyboard)
			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
				forwards = Vector3.RotateTowards(forwards, -right, 6*Time.fixedDeltaTime, 1);
			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
				forwards = Vector3.RotateTowards(forwards, right, 6*Time.fixedDeltaTime, 1);
			right = Vector3.Cross(forwards, up).normalized;

			// sum the WASD/Arrows movement
			float forward = 0;
			if (Input.GetKey(KeyCode.W)  || Input.GetKey(KeyCode.UpArrow))
				forward += 1;
			if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
				forward -= 1;

			// calculate where to move, then move
			Vector3 dest = transform.position + (forward*forwards).normalized
				*(Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift)?playerRunSpeed:playerSpeed);
			rigidbody.MovePosition(rigidbody.position.normalized*(radius-playerHeight));
			rigidbody.velocity = (dest-transform.position)/Time.fixedDeltaTime;
			transform.localRotation = Quaternion.LookRotation(forwards, up);
		} // if 2D
		else {
			// calculate up, forwards and right
			Vector3 up = Vector3.up;
			Vector3 forwards = Vector3.RotateTowards(up, transform.Find("LeftEyeAnchor").forward, Mathf.PI/2, 1).normalized;
			if (Vector3.Angle(up, forwards)<90)
				forwards = Vector3.RotateTowards(-up, transform.forward, Mathf.PI/2, 1).normalized;
			Vector3 right = Vector3.Cross(forwards, up).normalized;

			// handle left-right camera movement (from keyboard)
			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
				forwards = Vector3.RotateTowards(forwards, -right, 6*Time.fixedDeltaTime, 1);
			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
				forwards = Vector3.RotateTowards(forwards, right, 6*Time.fixedDeltaTime, 1);
			right = Vector3.Cross(forwards, up).normalized;

			// sum the WASD/Arrows movement
			float forward = 0;
			if (Input.GetKey(KeyCode.W)  || Input.GetKey(KeyCode.UpArrow))
				forward += 1;
			if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
				forward -= 1;

			Vector3 dest = transform.position + (forward*forwards).normalized
				*(Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift)?playerRunSpeed:playerSpeed);
			Vector3 v = rigidbody.position;
			v[1] = playerHeight;
			rigidbody.MovePosition(v);
			rigidbody.velocity = (dest-transform.position)/Time.fixedDeltaTime;
			transform.localRotation = Quaternion.LookRotation(forwards, up);
		}
	}

	// The OnTriggerEnter function is called when the collider attached to this game object (whatever object the script is attached to) overlaps another collider set to be a "trigger"
	void OnTriggerEnter(Collider collider) {
		//Debug.Log(collider.name);

		// We want to check if the thing we're colliding with is a collectable, this will differentiate it from other trigger objects which we might add in the future
		if (collider.GetComponent<Key>() == key) {
			//Time.timeScale = 0;
			GameObject.Find("CenterLight(Clone)").GetComponent<Light>().intensity = 0.2f;
			mazeStruct.RemoveDoor();
			collider.gameObject.SetActive(false);
			lights.keyTime = 0;
			//print(skyboxMaterial.GetColor("_Tint"));
			skyboxMaterial.SetColor("_Tint", new Color32((byte)44, (byte)28, (byte)53, (byte)128));
			this.lamp.gameObject.SetActive(true);
			monster.Init(main, mazeStruct, cells, transform, mazeStruct.GetMonsterSphere(), monsterSpeed, monsterHeight, is3D);

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

		if (collider.name=="EndZone") {
			main.LevelEndMenu(true);
			Destroy(gameObject);
		}
	}
}