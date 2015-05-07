using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MazeStructure {
	private MazeTool mazeTool;
	private bool[,,] data;
	private MazeCell[,,] cells;
	public int length;
	public float radius;
	public bool is3D;
	public float cellDist;

	private Point3 key;
	private Point3 door;
	private Point3 startPos;
	private Point3 monsterPos;
	private Point3 endPos;
	private Dictionary<Point3, Point3> torches=new Dictionary<Point3, Point3>();

	public Mesh floor;
	public Mesh[] cellWalls;
	public Mesh[] cellWallTops;
	public Material cellFloorMat;
	public Material cellWallMat;
	public Material cellWallTopMat;
	public GameObject doorObj;
	public GameObject torchObj;

	public MazeStructure(MazeTool top, MazeTool bottom, MazeTool left, MazeTool right, MazeTool front, MazeTool back,
		float radius, bool is3D, GameObject torchObj) {

		// verify that MazeTools have the same dimensions
		length = bottom.walls.GetLength(0);
		if (is3D) {
			if (top.walls.GetLength(0)!=length || top.walls.GetLength(1)!=length ||
				bottom.walls.GetLength(0)!=length || bottom.walls.GetLength(1)!=length ||
				left.walls.GetLength(0)!=length || left.walls.GetLength(1)!=length ||
				right.walls.GetLength(0)!=length || right.walls.GetLength(1)!=length ||
				front.walls.GetLength(0)!=length || front.walls.GetLength(1)!=length ||
				back.walls.GetLength(0)!=length || back.walls.GetLength(1)!=length)
				throw new Exception("MazeTool lengths don't match.");
		} else {
			if (bottom.walls.GetLength(1)!=bottom.walls.GetLength(0))
				throw new Exception("MazeTool dimensions don't match.");
		}
		length = 1+length/2;

		// initialize data
		this.radius = radius;
		this.is3D = is3D;
		this.mazeTool = top;
		this.torchObj = torchObj;
		if (this.torchObj==null) throw new Exception("torchobj==null");
		cellDist = 2*radius/length;
		data = new bool[2+mazeTool.walls.GetLength(0), 2+mazeTool.walls.GetLength(0), 2+mazeTool.walls.GetLength(1)];
		for (int i=0; i<data.GetLength(0); ++i)
			for (int j=0; j<data.GetLength(1); ++j)
				for (int k=0; k<data.GetLength(2); ++k)
					data[i, j, k] = true;

		// parse all the mazeTools
		int high = data.GetLength(0)-2;
		ParseMazeTool(bottom, (i, j) => new Point3(i+1, 1, j+1));
		if (is3D) {
			ParseMazeTool(top, (i, j) => new Point3(i+1, high, high-j));
			ParseMazeTool(left, (i, j) => new Point3(1, j+1, i+1));
			ParseMazeTool(right, (i, j) => new Point3(high, j+1, high-i));
			ParseMazeTool(front, (i, j) => new Point3(high-i, j+1, 1));
			ParseMazeTool(back, (i, j) => new Point3(i+1, j+1, high));
		}
		Debug.Log("key: "+key+"; start: "+startPos+"; monster: "+monsterPos+"; door: "+door+"; end: "+endPos);
	}

	/// <summary>
	/// Parses the given maze, and fits it into the data based off of the translate function.
	/// </summary>
	private void ParseMazeTool(MazeTool maze, Func<int, int, Point3> translate) {
		for (int i=0; i<maze.walls.GetLength(0); ++i) {
			for (int j=0; j<maze.walls.GetLength(1); ++j) {
				Point3 pos = translate(i, j);

				// parse walls
				MazeToolWall wall = maze.walls[i, j];
				if (wall!=null) {
					data[pos.x, pos.y, pos.z] = wall.gameObject.activeSelf;
					switch(wall.type) {
					case MazeToolWall.WallType.door:
						door = pos;
						break;
					}
				}

				// parse cells
				MazeToolCell cell = maze.cells[i, j];
				if (cell!=null) {
					switch(cell.type) {
					case MazeToolCell.CellType.startPos:
						startPos = pos;
						break;
					case MazeToolCell.CellType.monsterPos:
						monsterPos = pos;
						break;
					case MazeToolCell.CellType.key:
						key = pos;
						break;
					case MazeToolCell.CellType.torch:
						Point2 p = new Point2(i, j);
						Point2[] neighbors = p.neighbors();
						for (int k=0; k<4; ++k)
							if (maze.walls[neighbors[k].x, neighbors[k].y].type==MazeToolWall.WallType.torch)
								torches[pos] = translate(neighbors[k].x, neighbors[k].y);
						Debug.Log("new torch entry "+torches[pos]+" at "+pos);
						break;
					case MazeToolCell.CellType.end:
						endPos = pos;
						break;
					}
				}
			}
		}
	}

	/// <summary>
	/// Returns true if there is no wall between points p1 and p2 (in data-space).
	/// </summary>
	public bool ValidMove(Point3 p1, Point3 p2) {
		Point3 avg = (p1+p2)/2;
		return !data[avg.x, avg.y, avg.z];
	}

	/// <summary>
	/// Returns Pathfinding to the given Point3 in game-space.
	/// </summary>
	public Pathfinding Pathfind(Point3 pos) {
		return new Pathfinding(this, data, Point3FromGameToData(new Point3[] { pos }));
	}

	/// <summary>
	/// Returns the GameObject that corresponds to the door.
	/// </summary>
	public void RemoveDoor() {
		Point3[] gpts = Point3FromDataToGame(door);
		Point3[] dpts = new Point3[2];
		dpts[0] = Point3FromGameToData(new Point3[]{gpts[0]});
		dpts[1] = Point3FromGameToData(new Point3[]{gpts[1]});
		GameObject obj0 = cells[gpts[0].x, gpts[0].y, gpts[0].z].gameObject;
		GameObject obj1 = cells[gpts[1].x, gpts[1].y, gpts[1].z].gameObject;
		data[door.x, door.y, door.z] = false;
		cells[gpts[0].x, gpts[0].y, gpts[0].z] = MakeMazeCell(dpts[0], obj0.transform.parent.gameObject, true);
		cells[gpts[1].x, gpts[1].y, gpts[1].z] = MakeMazeCell(dpts[1], obj1.transform.parent.gameObject, true);
		GameObject.Destroy(obj0);
		GameObject.Destroy(obj1);
	}

	/// <summary>
	/// Returns the sphere-space start location.
	/// </summary>
	public Vector3 GetStartSphere() {
		Vector3 v = FromGameToCube(Point3FromDataToGame(startPos)[0]);
		return Vector3FromCubeToSphere(v);
	}

	/// <summary>
	/// Returns the game-space start location.
	/// </summary>
	public Point3[] GetStart() {
		return Point3FromDataToGame(startPos);
	}

	/// <summary>
	/// Returns the sphere-space start location.
	/// </summary>
	public Vector3 GetMonsterSphere() {
		Vector3 v = FromGameToCube(Point3FromDataToGame(monsterPos)[0]);
		return Vector3FromCubeToSphere(v);
	}

	/// <summary>
	/// Returns the game-space start location.
	/// </summary>
	public Point3[] GetMonster() {
		return Point3FromDataToGame(monsterPos);
	}

	/// <summary>
	/// Returns the sphere-space location of the door.
	/// </summary>
	public Vector3 FindDoorSphere() {
		Vector3 v = FromGameToCube(Point3FromDataToGame(door)[0]);
		return Vector3FromCubeToSphere(v);
	}

	/// <summary>
	/// Returns the game-space location of the door.
	/// </summary>
	public Point3[] FindDoor() {
		return Point3FromDataToGame(door);
	}

	/// <summary>
	/// Returns the sphere-space location of the key.
	/// </summary>
	public Vector3 FindKeySphere() {
		Vector3 v = FromGameToCube(Point3FromDataToGame(key)[0]);
		return Vector3FromCubeToSphere(v);
	}

	/// <summary>
	/// Returns the game-space location of the key.
	/// </summary>
	public Point3[] FindKey() {
		return Point3FromDataToGame(key);
	}

	public Point3 EndPos() {
		return Point3FromDataToGame(endPos)[0];
	}

	/// <summary>
	/// Transforms a Point3 from data-space to game-space.
	/// If the Point3 is representable by a single Point3, it returns the single Point3.
	/// If the Point3 is must be represented by 2 Point3's, it returns an array of 2 Point3's.
	/// This constraint must always be true: (p+1)==output[0]+output[output.Length-1].
	/// </summary>
	public Point3[] Point3FromDataToGame(Point3 p) {
		Point3 p2 = p;
		if (p.x%2==0) {
			p2.x += 1;
			p.x -= 1;
		}
		else if (p.y%2==0) {
			p2.y += 1;
			p.y -= 1;
		}
		else if (p.z%2==0) {
			p2.z += 1;
			p.z -= 1;
		}
		if (p==p2)
			return new Point3[] { (p+1)/2 };
		else
			return new Point3[] { (p+1)/2, (p2+1)/2 };
	}

	/// <summary>
	/// Transforms the given Point3(s) from game-space to data-space.
	/// The argument must have (Length==1 || Length==2).
	/// This constraint must always be true: (output+1)==p[0]+p[p.Length-1].
	/// </summary>
	public Point3 Point3FromGameToData(Point3[] p) {
		return ((p[0]+p[p.Length-1])-1);
	}

	/// <summary>
	/// Takes a Vector3 in cube-space and returns a corresponding Point3 in game-space.
	/// </summary>
	public Point3 FromCubeToGame(Vector3 v) {
		//return new Point3(v/2)+1;
		return new Point3(v)+1;
	}

	/// <summary>
	/// Takes a Point3 in game-space and returns the corresponding Vector3 in cube-space.
	/// </summary>
	public Vector3 FromGameToCube(Point3 p) {
		return (p+p-1).ToVector3()/2;
	}

	/// <summary>
	/// Takes a Vector3 in cube-space and translates it into a Vector3 in sphere-space.
	/// length is the number of cells in a row/column of the maze.
	/// floor is the cube-coordinate of the floor below v.
	/// radius is the radius of the sphere.
	/// </summary>
	public Vector3 Vector3FromCubeToSphere(Vector3 v) {
		Point3 p = FromCubeToGame(v);
		Vector3 floor = cells[p.x, p.y, p.z].GetFloor(v);
		return Vector3FromCubeToSphere(v, floor);
	}

	/// <summary>
	/// Takes a Vector3 in cube-space and translates it into a Vector3 in sphere-space.
	/// length is the number of cells in a row/column of the maze.
	/// floor is the cube-coordinate of the floor below v.
	/// radius is the radius of the sphere.
	/// </summary>
	public Vector3 Vector3FromCubeToSphere(Vector3 v, Vector3 floor) {
		if (is3D) {
			// translate v, center into a cube around Vector3.zero
			Vector3 center = Vector3.one*(length/2);
			v -= center;
			floor -= center;

			// calculate the result
			v = floor.normalized*radius*(1-(v-floor).magnitude/length);
		} else
			v.Scale(new Vector3(cellDist, cellDist/1.5f, cellDist));
		return v;
	}

	/// <summary>
	/// Takes a Vector3 in sphere-space and translates it into a Vector3 in cube-space.
	/// length is the number of cells in a row/column of the maze.
	/// radius is the radius of the sphere.
	/// </summary>
	public Vector3 Vector3FromSphereToCube(Vector3 v) {
		if (is3D) {
			// morph into a cube around Vector3.zero
			float max = Mathf.Max(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
			Vector3 floor = v*(0.5f*(length-0.01f)/max);

			// translate into a cube with Vector3.zero as the base corner
			Vector3 center = Vector3.one*(length/2);
			floor += center;

			// return floor + the appropriate height
			float height = radius-v.magnitude;
			if (floor.x<floor.y && floor.x<floor.z)
				v = floor+(height*Vector3.right);
			else if (floor.y<floor.z)
				v = floor+(height*Vector3.forward);
			else
				v = floor+(height*Vector3.up);
		} else
			v.Scale(new Vector3(1/cellDist, 1.5f/cellDist, 1/cellDist));
		return v;
	}

	/// <summary>
	/// Returns a 3D array of MazeCells that create a sphere, with indexes in game-space.
	/// </summary>
	public MazeCell[, ,] MakeCells(Mesh floor, Mesh[] cellWalls, Mesh[] cellWallTops, Material cellFloorMat,
		Material cellWallMat, Material cellWallTopMat, GameObject doorObj, AnimationCurve flicker, float radius) {

		// store variables
		this.floor = floor;
		this.cellWalls = cellWalls;
		this.cellWallTops = cellWallTops;
		this.cellFloorMat = cellFloorMat;
		this.cellWallMat = cellWallMat;
		this.cellWallTopMat = cellWallTopMat;
		this.doorObj = doorObj;

		GameObject container = new GameObject("Maze-Sphere Container");
		MazeCell[, ,] result = new MazeCell[(data.GetLength(0)+1)/2, (data.GetLength(1)+1)/2, (data.GetLength(2)+1)/2];
		bool[, ,] visited = new bool[data.GetLength(0), data.GetLength(1), data.GetLength(2)];
		visited.Initialize();
		/*for (int i=0; i<visited.GetLength(0); ++i)
			for (int j=0; j<visited.GetLength(1); ++j)
				for (int k=0; k<visited.GetLength(2); ++k)
					visited[i, j, k] = true;
		for (int i=0; i<5; ++i)
			for (int j=0; j<5; ++j)
				for (int k=0; k<5; ++k)
					visited[i, j, k] = false;*/

		int count=0;
		int yStart=(is3D?0:1);
		int yEnd = (is3D?3:2);
		int sideStart=1;
		int sideEnd = (is3D?data.GetLength(0):2);

		// iterate over {x, y, z}
		for (int yIndex=yStart; yIndex<yEnd; ++yIndex) {
			// iterate over both sides of the given axis
			for (int side=sideStart; side<sideEnd; side+=data.GetLength(yIndex)-3) {
				// create the parent object
				GameObject parent = new GameObject("side");
				parent.transform.parent = container.transform;

				// iterate over all cells in the given axis
				for (int i=1; i<data.GetLength((yIndex+1)%3); i+=2) {
					for (int j=1; j<data.GetLength((yIndex+2)%3); j+=2) {
						int xIndex = (yIndex+(side==1?1:2))%3;
						int zIndex = (yIndex+(side==1?2:1))%3;

						// create a Point3 that corresponds to the current location in data-space
						Point3 p = Point3.zero;
						p[yIndex] = side;
						p[xIndex] = i;
						p[zIndex] = j;

						// return if visited
						if (visited[p.x, p.y, p.z])
							continue;
						visited[p.x, p.y, p.z] = true;
						// error if the loop goes bad
						if (++count>5000)
							throw new Exception("count>1000");

						result[(p.x+1)/2, (p.y+1)/2, (p.z+1)/2] = MakeMazeCell(p, parent);
					}
				}
			}
		}
		this.cells = result;
		return result;
	}

	private MazeCell MakeMazeCell(Point3 p, GameObject parent, bool noDoor=false) {
		// find variables
		int yIndex;
		if (p.y==1||p.y==data.GetLength(1)-2)
			yIndex = 1;
		else if (p.x==1||p.x==data.GetLength(0)-2)
			yIndex = 0;
		else if (p.z==1||p.z==data.GetLength(2)-2)
			yIndex = 2;
		else
			throw new Exception("invalid point in MakeMazeCell("+p+", parent)");
		int side = p[yIndex];
		int xIndex = (yIndex+(side==1?1:2))%3;
		int zIndex = (yIndex+(side==1?2:1))%3;
		int i = p[xIndex];
		int j = p[zIndex];

		// create a vector space
		MazeCell.SquareTransformer sq = new MazeCell.SquareTransformer();
		sq.AddAll((p-1).ToVector3()/2);
		sq.AddAll((side==1?(side-1)/2:(side+1)/2) - (p[yIndex]-1)/2f, yIndex);
		sq.AddX1(1, xIndex);
		int yDiff = (side==1?1:-1);
		sq.vy[yIndex] = yDiff;
		sq.AddZ1(1, zIndex);

		// find cellWallIndex using a 2D point space
		int cellWallIndex=0;
		Point3 right = Point3.zero;
		right[xIndex] = 2;
		Point3 forward = Point3.zero;
		forward[zIndex] = 2;
		Point3 up = Point3.zero;
		up[yIndex] = 2;

		// calculate the index of cellWalls and cellWallTops to use
		cellWallIndex += ValidMove(p, p+right)?0:8;
		cellWallIndex += ValidMove(p, p+forward)?0:4;
		cellWallIndex += ValidMove(p, p-right)?0:2;
		cellWallIndex += ValidMove(p, p-forward)?0:1;

		// if the cell is on an edge, modify the vector space and cellWallIndex
		if (is3D && ((i==1||i==data.GetLength((yIndex+1)%3)-2) || (j==1||j==data.GetLength((yIndex+2)%3)-2))) {
			// modify the vector space and cellWallIndex
			if (i==1) {
				sq.AddX0(yDiff, yIndex);
				sq.vy[xIndex] = 1;
				cellWallIndex -= ValidMove(p, p+up)?2:0;
				cellWallIndex -= ValidMove(p, p-up)?2:0;
			} else if (i==data.GetLength((yIndex+1)%3)-2) {
				sq.AddX1(yDiff, yIndex);
				sq.vy[xIndex] = -1;
				cellWallIndex -= ValidMove(p, p+up)?8:0;
				cellWallIndex -= ValidMove(p, p-up)?8:0;
			} else if (j==1) {
				sq.AddZ0(yDiff, yIndex);
				sq.vy[zIndex] = 1;
				cellWallIndex -= ValidMove(p, p+up)?1:0;
				cellWallIndex -= ValidMove(p, p-up)?1:0;
			} else if (j==data.GetLength((yIndex+1)%3)-2) {
				sq.AddZ1(yDiff, yIndex);
				sq.vy[zIndex] = -1;
				cellWallIndex -= ValidMove(p, p+up)?4:0;
				cellWallIndex -= ValidMove(p, p-up)?4:0;
			}
			sq.vy.Normalize();

			// calculate the index of cellWalls and cellWallTops to use
		}

		// if the cell is on a corner, corner vector space
		if (is3D && (i==1||i==data.GetLength((yIndex+1)%3)-2) && (j==1||j==data.GetLength((yIndex+2)%3)-2)) {
			// modify the vector space
			if (i==1 && j==1) {
				sq.v10[yIndex] += yDiff;
				sq.v00[xIndex] += 1;
				sq.vy[zIndex] = sq.vy[xIndex];
			} else if (i==1 && j==data.GetLength((yIndex+2)%3)-2) {
				sq.v11[yIndex] += yDiff;
				sq.v01[xIndex] += 1;
				sq.vy[zIndex] = -sq.vy[xIndex];
			} else if (i==data.GetLength((yIndex+1)%3)-2 && j==1) {
				sq.v00[yIndex] += yDiff;
				sq.v10[xIndex] -= 1;
				sq.vy[zIndex] = -sq.vy[xIndex];
			} else {
				sq.v01[yIndex] += yDiff;
				sq.v11[xIndex] -= 1;
				sq.vy[zIndex] = sq.vy[xIndex];
			}
			sq.vy.Normalize();

			// calculate the index of cellWalls and cellWallTops to use
		}

		// handle door
		Point3 doorSide = Point3.zero;
		Point3[] gpts = Point3FromDataToGame(door);
		Debug.Log(gpts.Length);
		for (int ii=0; ii<gpts.Length; ++ii)
			Debug.Log("gpts[i] = "+gpts[ii]);
		Point3[] dpts = new Point3[2];
		dpts[0] = Point3FromGameToData(new Point3[]{gpts[0]});
		dpts[1] = Point3FromGameToData(new Point3[]{gpts[1]});
		if ((p==dpts[0] || p==dpts[1]) && !noDoor) {
			doorSide = (dpts[0]+dpts[1])/2 - p;
			Point3 temp = new Point3();
			temp[0] = doorSide[xIndex];
			temp[1] = doorSide[yIndex];
			temp[2] = doorSide[zIndex];
			doorSide = temp;
		}

		// handle torch
		Point3 torchSide = (torches.ContainsKey(p)?torches[p]-p:Point3.zero);
		if (torches.ContainsKey(p)) {
			Point3 temp = new Point3();
			temp[0] = torchSide[xIndex];
			temp[1] = torchSide[yIndex];
			temp[2] = torchSide[zIndex];
			torchSide = temp;
		}

		// create the MazeCell
		MazeCell cell = new GameObject("MazeCell "+p.x+" "+p.y+" "+p.z+" ("+i+", "+j+") - "+cellWallIndex).AddComponent<MazeCell>();
		cell.transform.parent = parent.transform;
		cell.Init(this, p, floor, cellWalls[cellWallIndex], cellWallTops[cellWallIndex], sq, torchSide, doorSide, p==endPos);
		return cell;
	}

	private bool IsEdge(Point3 pos) {
		int count=0;
		for (int i=0; i<3; ++i)
			if (pos[i]<2 || pos[i]>data.GetLength(i)-3)
				++count;
		return count>1;
	}

	private bool IsCorner(Point3 pos) {
		int count=0;
		for (int i=0; i<3; ++i)
			if (pos[i]<2 || pos[i]>data.GetLength(i)-3)
				++count;
		return count==3;
	}
}
