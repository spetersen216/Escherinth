using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MazeGame : MonoBehaviour {

	public LightSystem lights;
	//public GameObject mazeObj;
	public MazeTool mazeTool;
	public Key key;
	public GameObject door;
	public OVRPlayerController player_control;
	public MazeStructure mazeStruct;
	public GameObject left_cam;
	public GameObject right_cam;

	//private OVRMainMenu menu;
	//private bool inMenu = false;

	// Use this for initialization
	void Start () {
		//call for start menu
		mazeTool.Start ();
		//create maze objects
		createObjects (); //resize ground

		//add key
		//add door 
		//lights
	}

	
	// Update is called once per frame
	void Update () {
	
	}


	private void createObjects(){
		
		mazeStruct = new MazeStructure (mazeTool);

		key = ((GameObject)Instantiate(key.gameObject, mazeStruct.FindKey()[0].ToVector3(), Quaternion.identity)).GetComponent<Key>();
		//lights = ((GameObject)Instantiate (lights.gameObject, new Vector3 (85.4f, 100f, 100f),Quaternion.identity)).GetComponent<LightSystem>();
		lights.Init(mazeStruct);
		door = mazeStruct.GetDoor();

		Instantiate (player_control, new Vector3 (1, 1.11f, 1), Quaternion.identity);
		left_cam = GameObject.Find ("LeftEyeAnchor");
		right_cam = GameObject.Find("RightEyeAnchor");

		left_cam.gameObject.AddComponent<Skybox> ().material = 
			(Material)Resources.Load ("Overcast2 Skybox", typeof(Material));
		right_cam.gameObject.AddComponent<Skybox> ().material = 
			(Material)Resources.Load ("Overcast2 Skybox", typeof(Material));
//		walls = mazeTool.walls;



		//GameObject[] ws = Object.FindObjectsOfType (typeof(MazeToolWall)) as GameObject[];
		/*string[] names = new string[100];
		for (int i = 0; i < mazeTool.width+9; i++) {
			for (int j = 0; j < mazeTool.width+9; j++) {
				string s = "wall " + i + " " + j;
			}
		}
		GameObject[] w = new GameObject[100];
		int count = 0;
		foreach (string s in names) {
			w[count] = GameObject.Find (s);
			count++;
			Debug.Log (s);
		}*/
		//Debug.Log(""+ mazeStruct.FindKey().ToString());
		//key.transform.position.Set (mazeStruct.FindKey().x, mazeStruct.FindKey().y, mazeStruct.FindKey().z);
		//player_control.gameObject.AddComponent<CollectionMechanic> ();

		//key.tag = "collectable";

	}


	// The OnTriggerEnter function is called when the collider attached to this game object (whatever object the script is attached to) overlaps another collider set to be a "trigger"
	void OnTriggerEnter (Collider collider)
	{
		// We want to check if the thing we're colliding with is a collectable, this will differentiate it from other trigger objects which we might add in the future
		if (collider.GetComponent<Key>() == key)
		{
			//Debug.Log("Collision with collection");
			door.SetActive(false);
			collider.gameObject.SetActive(false);
			lights.keyTime = 0;
		}
	}
}
