using UnityEngine;
using System.Collections;

public class MazeGame : MonoBehaviour {

	private MazeTool maze;
	public Key key;
	private Point3 key_loc;
	private GameObject door;
	private GameObject map;
	private GameObject plane;
	public OVRPlayerController player_control;
	private MazeStructure mazeStruct;
	private Transform walls;
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
		map = new GameObject ();
		plane = GameObject.CreatePrimitive (PrimitiveType.Plane);
		//plane.transform.position = new Vector3 (0, 1, 0);
		plane.transform.localScale = new Vector3 (30, 30, 30);
		plane.transform.position = new Vector3(50,1,50);
		Instantiate (player_control, new Vector3 (0, 3, 0), Quaternion.identity);
		//Instantiate (maze, new Vector3 (0, 0, 0), Quaternion.identity);
		maze = map.AddComponent<MazeTool>();
		//maze.transform.localScale = new Vector3(10,10,10);
		walls = maze.wallContainer;
		map.transform.localPosition = new Vector3 (0f, 33.2f, 0f);
		map.transform.localScale = new Vector3 (500, 500, 500);
		
	}
}
