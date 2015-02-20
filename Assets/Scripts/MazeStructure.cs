using UnityEngine;
using System.Collections;

public class MazeStructure {
	private bool[,,] data;
	private Point3 door;
	private Point3 key;
	
	public MazeStructure(MazeTool mt) {
		// initialize data
		data = new bool[2+mt.walls.GetLength(0), 3, 2+mt.walls.GetLength(1)];
		for (int i=0; i<data.GetLength(0); ++i)
			for (int j=0; j<data.GetLength(1); ++j)
				for (int k=0; k<data.GetLength(2); ++k)
					data[i, j, k] = true;

		// parse MazeTool walls, cells
		for (int i=0; i<mt.walls.GetLength(0); ++i) {
			for (int j=0; j<mt.walls.GetLength(1); ++j) {
				// parse walls
				if (mt.walls[i, j]!=null) {
					data[1+i, 1, 1+j] = mt.walls[i, j].gameObject.activeSelf;
					if (mt.walls[i, j].type==MazeToolWall.WallType.door)
						door = new Point3(1+i, 1, 1+j);
				}

				// parse cells
				else if (mt.cells[i, j]!=null) {
					if (mt.cells[i, j].type==MazeToolCell.CellType.key)
						key = new Point3(1+i, 1, 1+j);
				}
			}
		}

	}

	public Pathfinding Pathfind(Point3 pos) {
		return new Pathfinding(data, pos);
	}
	
	public static Vector3 FromCubeToSphere(Vector3 v, float radius) {
		return v;
	}
	
	public static Vector3 FromSphereToCube(Vector3 v, float radius) {
		return v;
	}
	
	public Point3[] FindDoor() {
		return Point3FromDataToGame(door);
	}
	
	public Point3 FindKey() {
		return Point3FromDataToGame(key)[0];
	}

	/// <summary>
	/// Transforms a Point3 from data-space to game-space.
	/// If the Point3 is representable by a single Point3, it returns the single Point3.
	/// If the Point3 is must be represented by 2 Point3's, it returns an array of 2 Point3's.
	/// This constraint must always be true: (p+1)==output[0]+output[output.Length-1].
	/// </summary>
	public static Point3[] Point3FromDataToGame(Point3 p) {
		Point3 p2 = p;
		if (p.x%2==0) {
			p2.x += 1;
			p.x -= 1;
		}
		if (p.y%2==0) {
			p2.y += 1;
			p.y -= 1;
		}
		if (p==p2)
			return new Point3[]{(p+1)/2};
		else
			return new Point3[]{(p+1)/2, (p2+1)/2};
	}

	/// <summary>
	/// Transforms the given Point3(s) from game-space to data-space.
	/// The argument must have (Length==1 || Length==2).
	/// This constraint must always be true: (p+1)==output[0]+output[output.Length-1].
	/// </summary>
	public static Point3 Point3FromGameToData(Point3[] p) {
		return (p[0]+p[p.Length-1]-1);
	}
}
