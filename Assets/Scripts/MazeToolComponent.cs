using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class MazeToolComponent:MonoBehaviour {
	public bool selectMazeTool=false;
	public static bool scaleGizmoIcon=false;

	public virtual void Update() {
		if (selectMazeTool) {
			selectMazeTool = false;
			Selection.activeGameObject = transform.parent.parent.gameObject;
		}
	}
}

[CustomEditor(typeof(MazeToolWall))]
public class MazeToolWallEditor:Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		MazeToolComponent.scaleGizmoIcon = EditorGUILayout.Toggle("Scale Gizmo Icons", MazeToolComponent.scaleGizmoIcon);
	}
}


[CustomEditor(typeof(MazeToolCell))]
public class MazeToolCellEditor:Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		MazeToolComponent.scaleGizmoIcon = EditorGUILayout.Toggle("Scale Gizmo Icons", MazeToolComponent.scaleGizmoIcon);
	}
}
