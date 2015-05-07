﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell:MonoBehaviour {
	private GameObject floor;
	private GameObject wall;
	private GameObject wallTop;
	private GameObject torch;
	private GameObject door;
	private GameObject[] children;
	public Plane plane;

	/// <summary>
	/// Initializes the MazeCell. pos is in data-space.
	/// </summary>
	public void Init(MazeStructure mazeStruct, Point3 pos, Mesh cellFloor, Mesh cellWall, Mesh cellWallTop,
		SquareTransformer vectors, Point3 torchSide, Point3 doorSide, bool end) {

		// create cell floor
		floor = new GameObject("floor - "+cellFloor.name);
		floor.AddComponent<MeshFilter>().mesh = Morph(cellFloor, mazeStruct, vectors, false);
		floor.AddComponent<MeshRenderer>().material = (Material)Instantiate(mazeStruct.cellFloorMat);
		floor.transform.parent = transform;

		// create cell wall
		wall = new GameObject("cell wall - "+cellWall.name);
		wall.AddComponent<MeshFilter>().mesh = Morph(cellWall, mazeStruct, vectors, true);
		wall.AddComponent<MeshCollider>().sharedMesh = wall.GetComponent<MeshFilter>().mesh;
		wall.AddComponent<MeshRenderer>().material = (Material)Instantiate(mazeStruct.cellWallMat);
		wall.transform.parent = transform;

		// create cell wall top
		wallTop = new GameObject("cell wall top - "+cellWallTop.name);
		wallTop.AddComponent<MeshFilter>().mesh = Morph(cellWallTop, mazeStruct, vectors, true);
		wallTop.AddComponent<MeshRenderer>().material = (Material)Instantiate(mazeStruct.cellWallTopMat);
		wallTop.transform.parent = transform;

		plane = new Plane(vectors.vy, vectors.v00);
		children = new GameObject[]{floor, wall, wallTop};

		// place torch
		if (torchSide!=Point3.zero) {
			Vector3 toVec = torchSide.ToVector3()*0.4f;
			Vector3 middle = new Vector3(1, 1+(mazeStruct.is3D?0.1f:0), 1)/2;
			torch = (GameObject)Instantiate(mazeStruct.torchObj);
			torch.transform.localScale = torch.transform.localScale*mazeStruct.cellDist;
			Vector3 floorv = vectors.Translate(middle+toVec, false);
			torch.transform.parent = transform;
			torch.transform.position =
				mazeStruct.Vector3FromCubeToSphere(vectors.Translate(middle+toVec, true), floorv);
			floorv = vectors.Translate(middle, false);
			Vector3 v = mazeStruct.Vector3FromCubeToSphere(vectors.Translate(middle, true), floorv);

			// rotate the torch
			Vector3 up = (mazeStruct.is3D?-torch.transform.position.normalized:Vector3.up);
			Vector3 forward = (v-torch.transform.position).normalized;
			forward = Vector3.Cross(forward, up);
			if (Vector3.Angle(forward, up)<90)
				forward = Vector3.RotateTowards(-up, forward, Mathf.PI/2, 0);
			else
				forward = Vector3.RotateTowards(up, forward, Mathf.PI/2, 0);
			torch.transform.rotation = Quaternion.LookRotation(forward, up);
		}

		// place door
		if (doorSide!=Point3.zero) {
			Vector3 toVec = doorSide.ToVector3()*0.397f;
			Vector3 middle = new Vector3(1, 0, 1)/2;
			door = (GameObject)Instantiate(mazeStruct.doorObj);
			door.transform.localScale = door.transform.localScale*mazeStruct.cellDist;
			Vector3 floorv = vectors.Translate(middle+toVec, false);
			door.transform.parent = transform;
			door.transform.position =
				mazeStruct.Vector3FromCubeToSphere(vectors.Translate(middle+toVec, true), floorv);
			floorv = vectors.Translate(middle, false);
			Vector3 v = mazeStruct.Vector3FromCubeToSphere(vectors.Translate(middle, true), floorv);

			// rotate the door
			Vector3 up = (mazeStruct.is3D?-door.transform.position.normalized:Vector3.up);
			Vector3 forward = -(v-door.transform.position).normalized;
			if (Vector3.Angle(forward, up)<90)
				forward = Vector3.RotateTowards(-up, forward, Mathf.PI/2, 0);
			else
				forward = Vector3.RotateTowards(up, forward, Mathf.PI/2, 0);
			door.transform.rotation = Quaternion.LookRotation(forward, up);
		}

		// place endZone
		if (end) {
			// find cube mesh
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			Mesh cMesh = cube.GetComponent<MeshFilter>().mesh;
			Vector3[] verts = cMesh.vertices;
			for (int i=0; i<verts.Length; ++i)
				verts[i] = (verts[i]+new Vector3(-0.5f, 0.5f, -0.5f))/100;
			cMesh.vertices = verts;

			// create the endzone
			GameObject endZone = new GameObject("EndZone");
			MeshCollider col = endZone.AddComponent<MeshCollider>();
			col.sharedMesh = Morph(cMesh, mazeStruct, vectors, false);
			col.isTrigger = true;
			endZone.transform.parent = transform;
			Destroy(cube);
		}
	}

	public Vector3 GetFloor(Vector3 v) {
		return v - plane.normal*plane.GetDistanceToPoint(v);
	}

	private Mesh Morph(Mesh m, MazeStructure mazeStruct, SquareTransformer vectors, bool invertTris) {
		Mesh result = new Mesh();

		// create new vertices, the Vector Space given by x,y,z, with base at a
		Vector3[] verts = new Vector3[m.vertices.Length];
		for (int i=0; i<verts.Length; ++i) {
			verts[i] = (100*m.vertices[i]) + new Vector3(1, 0, 1);
			if (verts[i].x<-0.01f || verts[i].y<-0.01f || verts[i].z<-0.01f || verts[i].x>1.01f || verts[i].y>1.01f || verts[i].z>1.01f)
				throw new Exception("Mesh vertices out of range for MazeCell (mesh is "+m.name+", vertex is at ("+verts[i].x+", "+verts[i].y+", "+verts[i].z+"))");
			Vector3 floor = vectors.Translate(verts[i], false);
			verts[i] = vectors.Translate(verts[i], true);
			verts[i] = mazeStruct.Vector3FromCubeToSphere(verts[i], floor);
		}
		result.vertices = verts;

		// handle triangles
		if (invertTris) {
			int[] tris = new int[m.triangles.Length];
			for (int i=0; i<tris.Length; ++i)
				tris[i] = m.triangles[tris.Length-i-1];
			result.triangles = tris;
		} else
			result.triangles = (int[])m.triangles.Clone();

		// directly copy uvs, calculate normals, optimize
		result.uv = (Vector2[])m.uv.Clone();
		result.RecalculateNormals();
		result.Optimize();
		return result;
	}

	/// <summary>
	/// Adjusts the materials to the given color value.
	/// </summary>
	public void SetBrightness(Color val) {
		for (int i=0; i<children.Length; ++i)
			if (children[i]!=null)
				children[i].renderer.material.color = val;
	}

	/// <summary>
	/// This class is used to translate vectors, similar to vector spaces.
	/// </summary>
	public class SquareTransformer {
		public Vector3 v00, v10, v01, v11, vy;
		/// <summary>
		/// Sets every field to Vector3.zero.
		/// </summary>
		public SquareTransformer() {
			v00 = v10 = v01 = v11 = vy = Vector3.zero;
		}

		//////////////// TEN DIFFERENT ADD FUNCTIONS ////////////////
		/// <summary>
		/// Adds to v00 and v01.
		/// </summary>
		public void AddX0(Vector3 add) {
			v00 += add;
			v01 += add;
		}
		/// <summary>
		/// Adds to v00[index] and v01[index].
		/// </summary>
		public void AddX0(float add, int index) {
			v00[index] += add;
			v01[index] += add;
		}
		/// <summary>
		/// Adds to v10 and v11.
		/// </summary>
		public void AddX1(Vector3 add) {
			v10 += add;
			v11 += add;
		}
		/// <summary>
		/// Adds to v10[index] and v11[index].
		/// </summary>
		public void AddX1(float add, int index) {
			v10[index] += add;
			v11[index] += add;
		}
		/// <summary>
		/// Adds to v00 and v10.
		/// </summary>
		public void AddZ0(Vector3 add) {
			v00 += add;
			v10 += add;
		}
		/// <summary>
		/// Adds to v00[index] and v10[index].
		/// </summary>
		public void AddZ0(float add, int index) {
			v00[index] += add;
			v10[index] += add;
		}
		/// <summary>
		/// Adds to v01 and v11.
		/// </summary>
		public void AddZ1(Vector3 add) {
			v01 += add;
			v11 += add;
		}
		/// <summary>
		/// Adds to v01[index] and v11[index].
		/// </summary>
		public void AddZ1(float add, int index) {
			v01[index] += add;
			v11[index] += add;
		}
		/// <summary>
		/// Adds to v00, v10, v01, v11.
		/// </summary>
		public void AddAll(Vector3 add) {
			AddX0(add);
			AddX1(add);
		}
		/// <summary>
		/// Adds to v00[index], v10[index], v01[index], v11[index].
		/// </summary>
		public void AddAll(float add, int index) {
			AddX0(add, index);
			AddX1(add, index);
		}

		/// <summary>
		/// Returns the corresponding Vector3.
		/// For example, Get(0, 1) returns v01.
		/// </summary>
		public Vector3 Get(int xIndex, int zIndex) {
			switch (xIndex+zIndex+zIndex) {
			case 0:
				return v00;
			case 1:
				return v10;
			case 2:
				return v01;
			case 3:
				return v11;
			default:
				throw new IndexOutOfRangeException("Attempted to access an invalid Vector3 in a VectorSpaceish that.");
			}
		}

		/// <summary>
		/// Sets the corresponding Vector3 to val.
		/// For example, Set(val, 1, 0) sets v10 to val.
		/// </summary>
		public void Set(Vector3 val, int xIndex, int zIndex) {
			switch (xIndex+zIndex+zIndex) {
			case 0:
				v00 = val;
				return;
			case 1:
				v10 = val;
				return;
			case 2:
				v01 = val;
				return;
			case 3:
				v11 = val;
				return;
			default:
				throw new IndexOutOfRangeException("Attempted to access an invalid Vector3 in a VectorSpaceish that.");
			}
		}

		/// <summary>
		/// Translates v using this VectorSpaceish. Sets v.y to 0 if useY.
		/// </summary>
		public Vector3 Translate(Vector3 v, bool useY) {
			if (!useY)
				v.y = 0;
			float x0 = 1-v.x, x1 = v.x;
			float z0 = 1-v.z, z1 = v.z;
			return v.y*vy + (x0*(z0*v00+z1*v01) + x1*(z0*v10+z1*v11));
		}
	}
}
