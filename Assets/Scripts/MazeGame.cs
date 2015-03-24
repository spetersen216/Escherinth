using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MazeGame : MonoBehaviour {
	
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

	public float radius;
	public Key key;
	public GameObject door;
	public GameObject player_control;
	public MazeStructure mazeStruct;
	public GameObject left_cam;
	public GameObject right_cam;
	public AnimationCurve lightFlicker;
	public GameObject monster;
	public Pathfinding temp;
	private MazeCell[,,] cells;
	private GameObject mazeSphere;
	public Point3[] points;

	// Use this for initialization
	void Start () {
		//call for start menu
		top.Start ();
		bottom.Start ();
		left.Start ();
		right.Start ();
		back.Start ();
		front.Start ();

		/*bottom.displayBorders = false;
		top.displayBorders = false;
		back.displayBorders = false;

		top.gameObject.transform.localPosition = new Vector3(0,114.5f,93.6f);
		top.gameObject.transform.rotation = Quaternion.Euler (180, 0, 0);

		back.gameObject.transform.localPosition = new Vector3(-1.6f,103.5f,-10.1f);
		back.gameObject.transform.rotation = Quaternion.Euler (90, 0, 0);
		*/


		mazeStruct = new MazeStructure (top, bottom, left, right, front, back, radius);

		Vector3 position = mazeStruct.FindKey()[0].ToVector3()+new Vector3(-0.5f, -1, -0.5f);
		position.Scale(bottom.transform.localScale);
		key = ((GameObject)Instantiate(key.gameObject, position, Quaternion.identity)).GetComponent<Key>();
		key.transform.rotation = Quaternion.Euler (90,0,0);
		key.transform.localPosition += new Vector3 (0,1.5f,0);
		lights = ((GameObject)Instantiate (lights.gameObject, new Vector3 (85.4f, 100f, 100f),Quaternion.identity)).GetComponent<LightSystem>();
		cells = mazeStruct.MakeCells(cellFloor, cellWalls, cellWallTops,
			cellFloorMat, cellWallMat, cellWallTopMat, lightFlicker, radius);
		//MazeCell cell = new GameObject("SingleCell").AddComponent<MazeCell>();
		//cell.Init(new Point3(0,0,0),cellFloor,cellWalls[0], cellWallTops[0], cellFloorMat, cellWallMat, cellWallTopMat,
		  //                                    lightFlicker, Vector3.one, new Vector3(1,1,1), new Vector3(1,1,1), new Vector3(1,1,1));
		//cells = new MazeCell[2,2,2];
		//cells [0, 0, 0] = cell;
		//lights.Init(mazeStruct,cells);

		door = mazeStruct.GetDoor();
		monster = (GameObject)Instantiate(monster.gameObject, player_control.transform.localPosition, Quaternion.identity);
		GameObject player = (GameObject)Instantiate(player_control.gameObject, new Vector3 (1, 1.11f, 1), Quaternion.identity);
		left_cam = GameObject.Find("LeftEyeAnchor");
		right_cam = GameObject.Find("RightEyeAnchor");

		//monster.AddComponent<HUD> ().CameraFacing = left_cam.gameObject.GetComponent<Camera> ();

		left_cam.gameObject.AddComponent<Skybox>().material = 
			(Material)Resources.Load("Overcast2 Skybox", typeof(Material));
		right_cam.gameObject.AddComponent<Skybox>().material = 
			(Material)Resources.Load("Overcast2 Skybox", typeof(Material));
		player.AddComponent<RunTime>().Init(lights, door, key, left_cam.GetComponent<Skybox>().material, radius, mazeStruct);
		player.GetComponentInChildren<LightFlicker> ().enabled = false;
		player.GetComponentInChildren<Light> ().enabled = false;

		//Debug.Log ("t estingalnflkasdflkj");
		//monster.GetComponent<Rigidbody> ().AddRelativeForce (monster.transform.forward * 2);
		
		temp  = mazeStruct.Pathfind(mazeStruct.FindKey()[0]);
		Debug.Log (position);
		//points = temp.PathToPoint(monster);
		//while (temp.GetDistanceToEnd(new Point3(new Vector3(monster.transform.localPosition.x,monster.transform.localPosition.y,monster.transform.localPosition.z))) > 0) {

		//}

		//Debug.Log ("pt 1: "+points[0] + " pt2: "+temp.GetDistanceToEnd(new Point3(position)));

	//	temp.
		//monster.GetComponent<Navigate> ().SetDestination (player.transform);

	}

	void Update(){
		//monster.GetComponent<Navigate> ().SetDestination (this.door.transform);
		/*
		Debug.Log ("pt 1: "+points[0] + " pt2: "+temp.GetDistanceToEnd(new Point3(new Vector3(monster.transform.localPosition.x,monster.transform.localPosition.y,monster.transform.localPosition.z))));*/
	}

}
