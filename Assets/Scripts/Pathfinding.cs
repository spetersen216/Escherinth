using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding {
	public int[,,] data;

	public Pathfinding(bool[,,] data, Point3 start) {
		this.data.Initialize();

		// create visited matrix
		bool[, ,] visited = new bool[data.GetLength(0), data.GetLength(1), data.GetLength(2)];
		visited.Initialize();

		// initialize vars
		Queue<VisitPoint> toVisit = new Queue<VisitPoint>(data.GetLength(0)*data.GetLength(1));
		toVisit.Enqueue(new VisitPoint(start, 0));

		// visit every possible cell
		while (toVisit.Count>0) {
			// visit the given cell
			int value = toVisit.Peek().value;
			Point3 pos = toVisit.Dequeue().pos;
			this.data[pos.x, pos.y, pos.z] = value;

			// add appropriate neighbors to toVisit
			Point3[] neighbors = Point3.Scramble(pos.neighbors(2));
			foreach (Point3 newPos in neighbors) {
				Point3 avg = (pos+newPos)/2;
				if ((!data[avg.x, avg.y, avg.z]) && (!visited[newPos.x, newPos.y, newPos.z]))
					toVisit.Enqueue(new VisitPoint(newPos, value+1));
			}
		}
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
