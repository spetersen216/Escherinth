using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class MazeToolWall:MazeToolComponent {
	
	private string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
	public enum WallType {normal, door, sliding};
	public WallType type;
	private WallType last;

	public override void Update() {
		base.Update();
		if (type!=last) {
			last = type;
			MazeTool maze = transform.parent.parent.GetComponent<MazeTool>();
			if (maze!=null)
				maze.toString = maze.ToString();
		}
	}

	public override string ToString() {
		if (MazeTool.version!=1)
			throw new Exception("Wrong Version - MazeTool is using version "+MazeTool.version+
				", but MazeToolWall is using version "+1);
		return chars[(((int)type)<<1)+(gameObject.activeSelf?1:0)].ToString();
	}
	
	public void FromString(string str, int version) {
		switch(version) {
		case 0:
			gameObject.SetActive(true);
			type = WallType.normal;
			break;
		case 1:
			gameObject.SetActive((chars.IndexOf(str)&1)==1);
			type = (WallType)(chars.IndexOf(str)>>1);
			break;
		default:
			break;
		}
	}

	void OnDrawGizmos() {
		switch(type) {
		case WallType.door:
			Gizmos.DrawIcon(transform.position, "door.png", scaleGizmoIcon);
			break;
		case WallType.sliding:
			Gizmos.DrawIcon(transform.position, "sliding.png", scaleGizmoIcon);
			break;
		default:
			break;
		}
	}
}
