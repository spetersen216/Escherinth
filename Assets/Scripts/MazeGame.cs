using UnityEngine;
using System.Collections;

public class MazeGame : MonoBehaviour {

	public MazeTool Maze;
	private GameObject key;
	private GameObject door;
	private GameObject player;
	private GameObject plane;
	public OVRPlayerController player_control;
	private OVRMainMenu menu;
	private bool inMenu = false;

	// Use this for initialization
	void Start () {
		//call for start menu

		//create maze objects
		createObjects ();

		//add key
		//add door 
		//lights
	}

	
	// Update is called once per frame
	void Update () {
	
	}


	private void createObjects(){
	
		plane = GameObject.CreatePrimitive (PrimitiveType.Plane);
		plane.transform.position = new Vector3 (0, 1, 0);
		plane.transform.localScale = new Vector3 (30, 30, 30);
		Instantiate (player_control, new Vector3 (0, 3, 0), Quaternion.identity);
		Instantiate (Maze, new Vector3 (0, 0, 0), Quaternion.identity);
		Maze.transform.localScale = new Vector3 (10,10,10);
		menu = player_control.GetComponent<OVRMainMenu> ();
		menu.enabled = true;
		//menu.
		inMenu = true;
	}
}
