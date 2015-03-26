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

	public float radius;
	public Key key;
	public GameObject door;
	public GameObject player_control;
	//private GameObject player_collider;
	public MazeStructure mazeStruct;
	public GameObject left_cam;
	public GameObject right_cam;
	public AnimationCurve lightFlicker;
	public Monster monster;
	private MazeCell[, ,] cells;
	private GameObject mazeSphere;
	public Light cLight;



	// Use this for initialization
	void Start() {
		//call for start menu
		MazeTool[] tools = { top, bottom, left, right, back, front };

		for (int i=0; i<tools.Length; i++) {
			tools[i].Start();
			tools[i].gameObject.SetActive(false);
		}

		mazeStruct = new MazeStructure(top, bottom, left, right, front, back, radius);

		Vector3 position = mazeStruct.FindKeySphere().normalized*(radius-5);
		position = new Vector3(-11.96039f, 44f, -12.60795f);
		Debug.Log(position);
		key = ((GameObject)Instantiate(key.gameObject, position, Quaternion.identity)).GetComponent<Key>();
		key.transform.rotation = Quaternion.Euler(270, 0, 0);
		//key.transform.localPosition += new Vector3(0, 1.5f, 0);
		cLight = ((GameObject)Instantiate(cLight.gameObject, cLight.gameObject.transform.localPosition, Quaternion.identity)).GetComponent<Light>();
		lights = ((GameObject)Instantiate(lights.gameObject, new Vector3(85.4f, 100f, 100f), Quaternion.identity)).GetComponent<LightSystem>();
		cells = mazeStruct.MakeCells(cellFloor, cellWalls, cellWallTops,
			cellFloorMat, cellWallMat, cellWallTopMat, lightFlicker, radius);
		lights.Init(mazeStruct, cells);

		door = mazeStruct.GetDoor();

		// create and initialize player and monster
		GameObject player = (GameObject)Instantiate(player_control.gameObject, new Vector3(1, 1.11f, 1), Quaternion.identity);
		monster = (Monster)Instantiate(monster, new Vector3(4.79f, 47.36f, -.038f), Quaternion.identity);
		player.AddComponent<Player>().Init(lights, cells, door, key, Resources.Load<Material>("Overcast2 Skybox"), radius, mazeStruct, monster);
		monster.gameObject.SetActive(false);
	}
}