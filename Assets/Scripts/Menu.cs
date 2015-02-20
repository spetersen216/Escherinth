using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public GUISkin gSkin;

	void OnGUI(){
		if (GUI.Button (new Rect (Screen.width/2, Screen.height/2, 0, 0), "Start")) {

		}
	}
}
