using UnityEngine;
using System.Collections;

public class MazeGame : MonoBehaviour {

	private GameObject maze;
	private GameObject key;
	private GameObject door;
	private GameObject rig;
	private GameObject plane;
	private CharacterController char_control;
	private OVRGamepadController control;
	private OVRPlayerController player_control;
	private OVRMainMenu menu ;

	// Use this for initialization
	void Start () {
		//call for start menu

		//create/import maze
		plane = GameObject.CreatePrimitive (PrimitiveType.Plane);
		plane.transform.position = new Vector3 (0, -1, 0);
		rig = new GameObject();
		char_control = rig.AddComponent<CharacterController>();
		control = rig.AddComponent <OVRGamepadController>();
		player_control = rig.AddComponent <OVRPlayerController>();
		//rig.AddComponent <OVRMainMenu>();

	
	
		//add in OVR player controller 
		/*control = GetComponent<OVRGamepadController> ();
		player_control = GetComponent<OVRPlayerController> ();
		menu = GetComponent<OVRMainMenu> ();
		char_control = GetComponent<CharacterController> ();
*/
		/*rig.AddComponent (Transform);
		rig.AddComponent (control);
		rig.AddComponent (player_control);
		rig.AddComponent (char_control);
		rig.AddComponent (menu);
*/

	
		//add key
		//add door 
		//lights
	}

	
	// Update is called once per frame
	void Update () {
	
	}
}
