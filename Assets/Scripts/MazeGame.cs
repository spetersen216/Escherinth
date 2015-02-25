using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MazeGame : MonoBehaviour {

	public GameObject maze;
	public LightSystem lights;
	public string mazeToGenerate;
	public Component[] comps;
	private GameObject mazeObj;
	private MazeTool mazeTool;
	public IEnumerable walls;
	public Key key;
	private Point3 key_loc;
	private GameObject door;
	private GameObject map;
	private GameObject plane;
	public OVRPlayerController player_control;
	private MazeStructure mazeStruct;
	//private Transform walls;
	private GameObject left_cam;
	private GameObject right_cam;

	//private OVRMainMenu menu;
	//private bool inMenu = false;

	// Use this for initialization
	void Start () {
		//call for start menu

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

		key = (GameObject)Instantiate(key, new Vector3 (0, 0, 0), Quaternion.identity);
		lights = (GameObject)Instantiate(lights, new Vector3(85.4f,))
		mazeTool = maze.GetComponent<MazeTool>();

		//comps = mazeObj.GetComponents<Component>();

		//foreach (Component c in comps) {
		//	Debug.Log(c.ToString());
		//}

		mazeStruct = new MazeStructure (mazeTool);
//		Debug.Log ("" + map.name);	

		//plane.transform.position = new Vector3(50,1,50);
		Instantiate (player_control, new Vector3 (1, 1, 1), Quaternion.identity);
		left_cam = GameObject.Find ("LeftEyeAnchor");
		right_cam = GameObject.Find("RightEyeAnchor");

		left_cam.gameObject.AddComponent<Skybox> ().material = 
			(Material)Resources.Load ("Overcast2 Skybox", typeof(Material));
		right_cam.gameObject.AddComponent<Skybox> ().material = 
			(Material)Resources.Load ("Overcast2 Skybox", typeof(Material));
		walls = mazeTool.walls;

		mazeStruct.FindKey ();
		mazeStruct.FindDoor ();

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
}
