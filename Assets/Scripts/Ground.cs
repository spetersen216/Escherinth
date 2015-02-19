using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Ground : MonoBehaviour {
	public bool displayGround=false;
	// Use this for initialization
	void Start () {
		GameObject.Find ("Ground").GetComponent<MeshRenderer> ().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (displayGround == true) {
			GameObject.Find ("Ground").GetComponent<MeshRenderer> ().enabled = true;
		} else {
			GameObject.Find ("Ground").GetComponent<MeshRenderer> ().enabled = false;
		}
	}
	
}
