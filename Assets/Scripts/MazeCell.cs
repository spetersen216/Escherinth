using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell:MonoBehaviour {
	private GameObject floor;
	private GameObject wall;
	private GameObject wallTop;
	private GameObject[] children;

	/// <summary>
	/// Initializes
	/// </summary>
	public void Init(Point3 pos, GameObject cellFloor, GameObject cellWall, GameObject cellWallTop,
		Vector3 a, Vector3 x, Vector3 y, Vector3 z) {

		// create cell floor
		floor = new GameObject("floor");
		floor.AddComponent<MeshFilter>().mesh = Morph(floor.GetComponent<MeshFilter>().mesh, a, x, y, z);
		floor.AddComponent<Renderer>().material = (Material)Instantiate(floor.renderer.material);

		// create cell wall
		wall = new GameObject("cell wall");
		wall.AddComponent<MeshFilter>().mesh = Morph(cellWall.GetComponent<MeshFilter>().mesh, a, x, y, z);
		wall.AddComponent<Renderer>().material = (Material)Instantiate(cellWall.renderer.material);

		// create cell wall top
		wallTop = new GameObject("cell wall top");
		wallTop.AddComponent<MeshFilter>().mesh = Morph(cellWallTop.GetComponent<MeshFilter>().mesh, a, x, y, z);
		wallTop.AddComponent<Renderer>().material = (Material)Instantiate(cellWallTop.renderer.material);

		children = new GameObject[]{floor, wall, wallTop};
	}

	private Mesh Morph(Mesh m, Vector3 a, Vector3 x, Vector3 y, Vector3 z) {
		Mesh result = new Mesh();

		// create new vertices, the Vector Space given by x,y,z, with base at a
		Vector3[] verts = new Vector3[m.vertices.Length];
		for (int i=0; i<verts.Length; ++i)
			verts[i] = a + m.vertices[i].x*x + m.vertices[i].y*y + m.vertices[i].z*z;
		result.vertices = verts;

		// calculate new normals
		Vector3[] norms = new Vector3[m.normals.Length];
		for (int i=0; i<norms.Length; ++i)
			norms[i] = Vector3.zero;
		result.normals = norms;

		// directly copy uvs
		result.uv = m.uv;

		// directly copy triangles
		result.triangles = m.triangles;

		return result;
	}

	public void SetBrightness(Color val) {
		for (int i=0; i<children.Length; ++i)
			children[i].renderer.material.color = val;
	}
}
