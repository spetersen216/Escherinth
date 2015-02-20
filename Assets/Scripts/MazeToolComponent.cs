using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class MazeToolComponent:MonoBehaviour {
	public bool selectMazeTool;

	public void Update() {
		if (selectMazeTool) {
			selectMazeTool = false;
			Selection.activeGameObject = transform.parent.parent.gameObject;
		}
	}
}
