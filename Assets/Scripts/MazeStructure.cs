using UnityEngine;
using System.Collections;

public class MazeStructure {
	private MazeTool mazeTool;
	private bool[,,] data;
	private Point3 door;
	private Point3 key;
	
	public MazeStructure(MazeTool mazeTool) {
		// initialize data
		this.mazeTool = mazeTool;
		data = new bool[2+mazeTool.walls.GetLength(0), 3, 2+mazeTool.walls.GetLength(1)];
		for (int i=0; i<data.GetLength(0); ++i)
			for (int j=0; j<data.GetLength(1); ++j)
				for (int k=0; k<data.GetLength(2); ++k)
					data[i, j, k] = true;

		// parse MazeTool walls, cells
		for (int i=0; i<mazeTool.walls.GetLength(0); ++i) {
			for (int j=0; j<mazeTool.walls.GetLength(1); ++j) {
				// parse walls
				if (mazeTool.walls[i, j]!=null) {
					data[1+i, 1, 1+j] = mazeTool.walls[i, j].gameObject.activeSelf;
					if (mazeTool.walls[i, j].type==MazeToolWall.WallType.door)
						door = new Point3(1+i, 1, 1+j);
				}

				// parse cells
				else if (mazeTool.cells[i, j]!=null) {
					if (mazeTool.cells[i, j].type==MazeToolCell.CellType.key)
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
	
	public GameObject GetDoor() {
		return mazeTool.walls[door.x, door.y].gameObject;
	}
	
	public GameObject GetKey() {
		GameObject result = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		result.transform.position = key.ToVector3()+(Vector3.one/2);
		return result;
	}

	public void FindDoor() { }
	public void FindKey() { }

	public GameObject GetWalls() {
		return (GameObject)Object.Instantiate(mazeTool.wallContainer.gameObject);
	}

	public Light[,] GetLights(Light l) {
		Light[,] result = new Light[mazeTool.width, mazeTool.height];
		GameObject container = new GameObject("lights");
		for (int i=0; i<mazeTool.cells.GetLength(0); ++i) {
			for (int j=0; j<mazeTool.cells.GetLength(1); ++j) {
				GameObject light = (GameObject)Object.Instantiate(l.gameObject);
				result[i, j] = light.GetComponent<Light>();
				light.transform.position = new Vector3(i+0.5f, 1, j+0.5f);
				light.transform.parent = container.transform;
			}
		}
		return result;
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
