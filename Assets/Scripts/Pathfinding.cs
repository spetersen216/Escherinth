using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding {
	private Point3 center;
	private int[,,] data;
	private bool[,,] walls;

	public Pathfinding(bool[,,] walls, Point3 center) {
		// initialize member variables
		this.center = center;
		data = new int[walls.GetLength(0), walls.GetLength(1), walls.GetLength(2)];
		data.Initialize();
		this.walls = new bool[walls.GetLength(0), walls.GetLength(1), walls.GetLength(2)];
		for (int i=0; i<walls.GetLength(0); ++i)
			for (int j=0; j<walls.GetLength(1); ++j)
				for (int k=0; k<walls.GetLength(2); ++k)
					this.walls[i, j, k] = walls[i, j, k];

		// create visited matrix
		bool[, ,] visited = new bool[walls.GetLength(0), walls.GetLength(1), walls.GetLength(2)];
		visited.Initialize();

		// initialize vars
		Queue<VisitPoint> toVisit = new Queue<VisitPoint>(walls.GetLength(0)*walls.GetLength(1));
		toVisit.Enqueue(new VisitPoint(center, 0));

		// visit every possible cell
		while (toVisit.Count>0) {
			// visit the given cell
			int value = toVisit.Peek().value;
			Point3 pos = toVisit.Dequeue().pos;
			data[pos.x, pos.y, pos.z] = value;
			visited[pos.x, pos.y, pos.z] = true;

			// add appropriate neighbors to toVisit
			Point3[] neighbors = Point3.Scramble(pos.neighbors(2));
			foreach (Point3 newPos in neighbors)
				if (ValidMove(pos, newPos) && (!visited[newPos.x, newPos.y, newPos.z]))
					toVisit.Enqueue(new VisitPoint(newPos, value+1));
		}
	}

	/// <summary>
	/// Returns a Point3[] that gives directions from the center to the target.
	/// This includes both endpoints.
	/// </summary>
	public Point3[] PathToPoint(Point3 target) {
		target = MazeStructure.Point3FromGameToData(new Point3[]{target});
		Point3[] result = new Point3[data[target.x, target.y, target.z]+1];
		int index = result.Length;

		while (target!=center) {
			Debug.Log(target);
			Point3 previous = target;
			Point3[] neighbors = target.neighbors(2);
			foreach (Point3 newPos in neighbors) {
				if (ValidMove(target, newPos) && data[newPos.x, newPos.y, newPos.z]<data[target.x, target.y, target.z]) {
					result[--index] = MazeStructure.Point3FromDataToGame(target)[0];
					target = newPos;
					break;
				}
			}
			// if no valid neighbor was found, there is no path
			if (target==previous) {
				Debug.Log("Null");
				return null;
			}
		}
		result[--index] = MazeStructure.Point3FromDataToGame(target)[0];
		Debug.Log("Index: "+index);
		return result;
	}

	public int GetGamePos(Point3 pos) {
		pos = MazeStructure.Point3FromGameToData(new Point3[]{pos});
		return data[pos.x, pos.y, pos.z];
	}

	private bool ValidMove(Point3 p1, Point3 p2) {
		Point3 avg = (p1+p2)/2;
		return !walls[avg.x, avg.y, avg.z];
	}

	private struct VisitPoint {
		public Point3 pos;
		public int value;
		public VisitPoint(Point3 pos, int value) {
			this.pos = pos;
			this.value = value;
		}
	}
}
