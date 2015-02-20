using UnityEngine;
using System.Collections;

public class MazeStructure {
	private bool[,,] data;
	
	public MazeStructure(MazeTool mt) {
		// initialize data
		data = new bool[2+mt.walls.GetLength(0), 3, 2+mt.walls.GetLength(1)];
	}
	
	public Pathfinding Pathfind(Point3 pos) {
		return null;
	}
	
	public Vector3 FromCubeToSphere(Vector3 v) {
		return v;
	}
	
	public Vector3 FromSphereToCube(Vector3 v) {
		return v;
	}
	
	public Point3[] FindDoor() {
		return new Point3[]{};
	}
	
	public Point3 FindKey() {
		return new Point3();
	}
}
