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
	public float radius;
	public Key key;
	public GameObject door;
	public OVRPlayerController player_control;
	public MazeStructure mazeStruct;
	public GameObject left_cam;
	public GameObject right_cam;
	public GameObject[] walls;
	public GameObject[] tops;
	public GameObject floor;
	public AnimationCurve lightFlicker;

	// Use this for initialization
	void Start () {
		//call for start menu
		top.Start ();
		bottom.Start ();
		left.Start ();
		right.Start ();
		back.Start ();
		front.Start ();
		
		mazeStruct = new MazeStructure (top, bottom, left, right, front, back);//, lightFlicker);

		Vector3 position = mazeStruct.FindKey()[0].ToVector3()+new Vector3(-0.5f, -1, -0.5f);
		position.Scale(bottom.transform.localScale);
		key = ((GameObject)Instantiate(key.gameObject, position, Quaternion.identity)).GetComponent<Key>();
		key.transform.rotation = Quaternion.Euler (90,0,0);
		key.transform.localPosition += new Vector3 (0,1.5f,0);
		//lights = ((GameObject)Instantiate (lights.gameObject, new Vector3 (85.4f, 100f, 100f),Quaternion.identity)).GetComponent<LightSystem>();
		lights.Init(mazeStruct,walls,tops,floor, radius);
		door = mazeStruct.GetDoor();
		
		GameObject player = (GameObject)Instantiate(player_control.gameObject, new Vector3 (1, 1.11f, 1), Quaternion.identity);
		left_cam = GameObject.Find("LeftEyeAnchor");
		right_cam = GameObject.Find("RightEyeAnchor");
		left_cam.gameObject.AddComponent<Skybox>().material = 
			(Material)Resources.Load("Overcast2 Skybox", typeof(Material));
		right_cam.gameObject.AddComponent<Skybox>().material = 
			(Material)Resources.Load("Overcast2 Skybox", typeof(Material));
		player.AddComponent<RunTime>().Init(lights, door, key, left_cam.GetComponent<Skybox>().material);
		player.GetComponentInChildren<LightFlicker> ().enabled = false;
		player.GetComponentInChildren<Light> ().enabled = false;



	}

}
