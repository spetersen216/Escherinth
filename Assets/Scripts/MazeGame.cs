using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MazeGame : MonoBehaviour {

	public Maze maze;
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
		//map = new GameObject ("Map");
		//plane = GameObject.CreatePrimitive (PrimitiveType.Plane);
		//plane.transform.SetParent (map.transform);
		//plane.transform.position = new Vector3 (0, 1, 0);

		Instantiate (key, new Vector3 (0, 0, 0), Quaternion.identity);
		//key.tag = "collectable";
		//Instantiate (lights, new Vector3 (85.4f, 125.9f, 242.1f), Quaternion.identity);
		//lights.gameObject.transform.Rotate (new Vector3 (33.053f, 194.3891f, 335.2719f));
		//Instantiate (maze, new Vector3 (488.9f, -304f, -274.2f), Quaternion.identity);
		mazeObj = GameObject.Find ("Maze");
		mazeTool = mazeObj.GetComponent<MazeTool> ();
		//mazeTool.walls = GameObject.Find ("wall container");
		//mazeTool.cells = GameObject.Find ("cell container");
		//mazeTool.border = GameObject.Find ("border");

		//comps = mazeObj.GetComponents<Component>();

		//foreach (Component c in comps) {
		//	Debug.Log(c.ToString());
		//}
		//mazeTool = GameObject.Find (""+map.name).GetComponent<MazeTool>();
		//mazeTool = (MazeTool)FindObjectOfType(typeof(MazeTool));
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
