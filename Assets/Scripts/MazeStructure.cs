﻿using UnityEngine;
using System.Collections;
using System;

public class MazeStructure {
	private MazeTool mazeTool;
	private bool[,,] data;
	private Point3 door;
	private Point3 key;
	public int length;
	public float radius;
	private Point3 startPos;
	
	public MazeStructure(MazeTool top, MazeTool bottom, MazeTool left, MazeTool right, MazeTool front, MazeTool back, float radius) {
		// initialize data
		this.mazeTool = top;
		data = new bool[2+mazeTool.walls.GetLength(0), 2+mazeTool.walls.GetLength(0), 2+mazeTool.walls.GetLength(1)];
		for (int i=0; i<data.GetLength(0); ++i)
			for (int j=0; j<data.GetLength(1); ++j)
				for (int k=0; k<data.GetLength(2); ++k)
					data[i, j, k] = true;
		
		// verify that MazeTools have the same dimensions
		this.radius = radius;
		length = top.walls.GetLength(0);
		if (top.walls.GetLength(0)!=length || top.walls.GetLength(1)!=length ||
			bottom.walls.GetLength(0)!=length || bottom.walls.GetLength(1)!=length ||
			left.walls.GetLength(0)!=length || left.walls.GetLength(1)!=length ||
			right.walls.GetLength(0)!=length || right.walls.GetLength(1)!=length ||
			front.walls.GetLength(0)!=length || front.walls.GetLength(1)!=length ||
			back.walls.GetLength(0)!=length || back.walls.GetLength(1)!=length)
				throw new Exception("MazeTool lengths don't match.");

		// parse all the mazeTools
		int high = data.GetLength(0)-2;
		ParseMazeTool(top, (i, j) => new Point3(i+1, high, high-j));
		ParseMazeTool(bottom, (i, j) => new Point3(i+1, 1, j+1));
		ParseMazeTool(left, (i, j) => new Point3(1, j+1, i+1));
		ParseMazeTool(right, (i, j) => new Point3(high, j+1, high-i));
		ParseMazeTool(front, (i, j) => new Point3(high-i, j+1, 1));
		ParseMazeTool(back, (i, j) => new Point3(i+1, j+1, high));
		
		startPos = new Point3(3, 1, 3);
		key = new Point3(9, 19, 11);
		door = new Point3(2, 1, 1);
		Debug.Log("key is at "+key);
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
					if (wall.type==MazeToolWall.WallType.door)
						door = pos;
				}

				// parse cells
				MazeToolCell cell = maze.cells[i, j];
				if (cell!=null) {
					if (cell.type==MazeToolCell.CellType.key)
						key = pos;
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
	/// Returns Pathfinding to the given Point3.
	/// </summary>
	public Pathfinding Pathfind(Point3 pos) {
		return new Pathfinding(this, data, Point3FromGameToData(new Point3[]{pos}));
	}
	
	/// <summary>
	/// Returns the GameObject that corresponds to the door.
	/// </summary>
	public GameObject GetDoor() {
		return mazeTool.walls[door.x-1, door.y-1].gameObject;
	}

	/// <summary>
	/// Returns the sphere-space start location.
	/// </summary>
	public Vector3 GetStartSphere() {
		Vector3 v = FromGameToCube(Point3FromDataToGame(startPos)[0]);
		return Vector3FromCubeToSphere(v, length, v, radius);
	}

	/// <summary>
	/// Returns the game-space start location.
	/// </summary>
	public Point3[] GetStart() {
		return Point3FromDataToGame(startPos);
	}
	
	/// <summary>
	/// Returns the sphere-space location of the door.
	/// </summary>
	public Vector3 FindDoorSphere() {
		Vector3 v = FromGameToCube(Point3FromDataToGame(door)[0]);
		return Vector3FromCubeToSphere(v, length, v, radius);
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
		return Vector3FromCubeToSphere(v, length, v, radius);
	}

	/// <summary>
	/// Returns the game-space location of the key.
	/// </summary>
	public Point3[] FindKey() {
		return Point3FromDataToGame(key);
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
	/// This constraint must always be true: (output+1)==p[0]+p[p.Length-1].
	/// </summary>
	public static Point3 Point3FromGameToData(Point3[] p) {
		return ((p[0]+p[p.Length-1])-1);
	}

	/// <summary>
	/// Takes a Vector3 in cube-space and returns a corresponding Point3 in game-space.
	/// </summary>
	public static Point3 FromCubeToGame(Vector3 v) {
		return new Point3(v);
	}

	/// <summary>
	/// Takes a Point3 in game-space and returns the corresponding Vector3 in cube-space.
	/// </summary>
	public static Vector3 FromGameToCube(Point3 p) {
		return (p+p+1).ToVector3()/2;
	}

	/// <summary>
	/// Takes a Vector3 in cube-space and translates it into a Vector3 in sphere-space.
	/// length is the number of cells in a row/column of the maze.
	/// floor is the cube-coordinate of the floor below v.
	/// radius is the radius of the sphere.
	/// </summary>
	public static Vector3 Vector3FromCubeToSphere(Vector3 v, int length, Vector3 floor, float radius=-1) {
		// handle radius
		if (radius<0)
			radius = length/2;

		// translate v, center into a cube around Vector3.zero
		Vector3 center = Vector3.one*(length/2);
		v -= center;
		floor -= center;

		// calculate the result
		v = floor.normalized*radius*(1-(v-floor).magnitude/length);
		return v;
	}

	/// <summary>
	/// Takes a Vector3 in sphere-space and translates it into a Vector3 in cube-space.
	/// length is the number of cells in a row/column of the maze.
	/// radius is the radius of the sphere.
	/// </summary>
	public static Vector3 Vector3FromSphereToCube(Vector3 v, int length, float radius=-1) {
		// handle radius
		if (radius<0)
			radius = length/2;
		// morph into a cube around Vector3.zero
		float max = Mathf.Max(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
		Vector3 floor = v*(0.5f*length/max);

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
		return v;
	}

	public Vector3 Move(Vector3 from, Vector3 to) {
		/*Func<Vector3, Point3> convert = (v)=> Point3FromGameToData(new Point3[]{FromCubeToGame(Vector3FromSphereToCube(v, length, radius))});
		Point3 start = convert(from.normalized*radius);
		Point3 end = convert(to.normalized*radius);

		// calculate offsets from "to"
		Point3[] offsets = Point3.zero.neighbors();
		for (int i=0; i<offsets.Length; ++i)
			offsets[i] = convert(to+(offsets[i].ToVector3()/10));

		// check if near an invalid wall
		for (int i=0; i<offsets.Length; ++i) {
			// count the number of different fields between offsets[i] and start
			int diff=0;
			for (int j=0; j<3; ++j)
				if (offsets[i][j]!=start[j])
					++diff;

			// if no differences, continue
			if (diff==0)
				continue;

			// if one difference, check if the move is valid
			if (diff==1) {
				if (ValidMove(start, offsets[i]))
					continue;
				else
					return from;
			}

			// if multiple differences, move is invalid
			if (diff==2)
				return from;
		}*/

		return to;
	}

	/// <summary>
	/// Returns a 3D array of MazeCells that create a sphere, with indexes in game-space.
	/// </summary>
	public MazeCell[,,] MakeCells(Mesh floor, Mesh[] cellWalls, Mesh[] cellWallTops, Material cellFloorMat,
		Material cellWallMat, Material cellWallTopMat, AnimationCurve flicker, float radius)
	{
		GameObject container = new GameObject("Maze-Sphere Container");
		MazeCell[,,] result = new MazeCell[(data.GetLength(0)+1)/2, (data.GetLength(1)+1)/2, (data.GetLength(2)+1)/2];
		bool[,,] visited = new bool[data.GetLength(0), data.GetLength(1), data.GetLength(2)];
		visited.Initialize();
/*for (int i=0; i<visited.GetLength(0); ++i)
	for (int j=0; j<visited.GetLength(1); ++j)
		for (int k=0; k<visited.GetLength(2); ++k)
			visited[i, j, k] = true;
		for (int i=0; i<3; ++i)
			for (int j=0; j<3; ++j)
				for (int k=0; k<3; ++k)
					visited[i, j, k] = false;*/
		int count=0;

		// iterate over {x, y, z}
		for (int yIndex=0; yIndex<3; ++yIndex) {
			// iterate over both sides of the given axis
			for (int side=1; side<data.GetLength(yIndex); side+=data.GetLength(yIndex)-3) {
				// create the parent object
				GameObject parent = new GameObject("side");
				parent.transform.parent = container.transform;

				// iterate over all cells in the given axis
				for (int i=1; i<data.GetLength((yIndex+1)%3); i+=2) {
					for (int j=1; j<data.GetLength((yIndex+2)%3); j+=2) {
						int xIndex = (yIndex+(side==1?1:2))%3;
						int zIndex = (yIndex+(side==1?2:1))%3;
						//int xIndex2 = (yIndex+1)%3;
						//int yIndex2 = (yIndex+2)%3;

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
						if (++count>1000)
							throw new Exception("count>1000");

						// create a vector space
						MazeCell.VectorSpaceish v = new MazeCell.VectorSpaceish();
						v.AddAll((p-1).ToVector3()/2);
						v.AddAll((side==1?(side-1)/2:(side+1)/2) - (p[yIndex]-1)/2f, yIndex);
						v.AddX1(1, xIndex);
						int yDiff = (side==1?1:-1);
						v.vy[yIndex] = yDiff;
						v.AddZ1(1, zIndex);

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
						if ((i==1||i==data.GetLength((yIndex+1)%3)-2) || (j==1||j==data.GetLength((yIndex+2)%3)-2)) {
							// modify the vector space and cellWallIndex
							if (i==1) {
								v.AddX0(yDiff, yIndex);
								v.vy[xIndex] = 1;
								cellWallIndex -= ValidMove(p, p+up)?2:0;
								cellWallIndex -= ValidMove(p, p-up)?2:0;
							} else if (i==data.GetLength((yIndex+1)%3)-2) {
								v.AddX1(yDiff, yIndex);
								v.vy[xIndex] = -1;
								cellWallIndex -= ValidMove(p, p+up)?8:0;
								cellWallIndex -= ValidMove(p, p-up)?8:0;
							} else if (j==1) {
								v.AddZ0(yDiff, yIndex);
								v.vy[zIndex] = 1;
								cellWallIndex -= ValidMove(p, p+up)?1:0;
								cellWallIndex -= ValidMove(p, p-up)?1:0;
							} else if (j==data.GetLength((yIndex+1)%3)-2) {
								v.AddZ1(yDiff, yIndex);
								v.vy[zIndex] = -1;
								cellWallIndex -= ValidMove(p, p+up)?4:0;
								cellWallIndex -= ValidMove(p, p-up)?4:0;
							}
							v.vy.Normalize();

							// calculate the index of cellWalls and cellWallTops to use
						}

						// if the cell is on a corner, corner vector space
						if ((i==1||i==data.GetLength((yIndex+1)%3)-2) && (j==1||j==data.GetLength((yIndex+2)%3)-2)) {
							// modify the vector space
							if (i==1 && j==1) {
								v.v10[yIndex] += yDiff;
								v.v00[xIndex] += 1;
								v.vy[zIndex] = v.vy[xIndex];
							} else if (i==1 && j==data.GetLength((yIndex+2)%3)-2) {
								v.v11[yIndex] += yDiff;
								v.v01[xIndex] += 1;
								v.vy[zIndex] = -v.vy[xIndex];
							} else if (i==data.GetLength((yIndex+1)%3)-2 && j==1) {
								v.v00[yIndex] += yDiff;
								v.v10[xIndex] -= 1;
								v.vy[zIndex] = -v.vy[xIndex];
							} else {
								v.v01[yIndex] += yDiff;
								v.v11[xIndex] -= 1;
								v.vy[zIndex] = v.vy[xIndex];
							}
							v.vy.Normalize();

							// calculate the index of cellWalls and cellWallTops to use
						}

						// create the MazeCell
						MazeCell cell = new GameObject("MazeCell "+p.x+" "+p.y+" "+p.z+" ("+i+", "+j+") - "+cellWallIndex).AddComponent<MazeCell>();
						cell.transform.parent = parent.transform;
						//Debug.Log("cellWallIndex: "+cellWallIndex);
						cell.Init(p, floor, cellWalls[cellWallIndex], cellWallTops[cellWallIndex], cellFloorMat, cellWallMat,
							cellWallTopMat, flicker, v);
						result[(p.x+1)/2, (p.y+1)/2, (p.z+1)/2] = cell;
					}
				}
			}
		}
		return result;
	}
}
