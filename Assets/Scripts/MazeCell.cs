using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell:MonoBehaviour {
	private GameObject floor;
	private GameObject wall;
	private GameObject wallTop;
	private GameObject[] children;
	private Light lightbulb;
	public LightFlicker flicker;

	/// <summary>
	/// Initializes the MazeCell.
	/// </summary>
	public void Init(Point3 pos, Mesh cellFloor, Mesh cellWall, Mesh cellWallTop, Material cellFloorMat, Material cellWallMat,
		Material cellWallTopMat, AnimationCurve curve, Vector3 a, Vector3 x, Vector3 y, Vector3 z) {
			if (pos==Point3.one)
				print("pos == Point3.one: "+a);

		// create cell floor
		floor = new GameObject("floor");
		floor.AddComponent<MeshFilter>().mesh = Morph(cellFloor, a, x, y, z);
		floor.AddComponent<MeshRenderer>().material = (Material)Instantiate(cellFloorMat);
		floor.transform.parent = transform;

		// create cell wall
		wall = new GameObject("cell wall");
		wall.AddComponent<MeshFilter>().mesh = Morph(cellWall, a, x, y, z);
		wall.AddComponent<MeshRenderer>().material = (Material)Instantiate(cellWallMat);
		wall.transform.parent = transform;

		// create cell wall top
		wallTop = new GameObject("cell wall top");
		wallTop.AddComponent<MeshFilter>().mesh = Morph(cellWallTop, a, x, y, z);
		wallTop.AddComponent<MeshRenderer>().material = (Material)Instantiate(cellWallTopMat);
		wallTop.transform.parent = transform;

		// initialize lightbulb
		lightbulb = new GameObject("light").AddComponent<Light>();
		lightbulb.type = LightType.Point;
		lightbulb.range = 12;
		lightbulb.intensity = 0.4f;
		lightbulb.renderMode = LightRenderMode.ForcePixel;
		lightbulb.transform.parent = transform;

		// add LightFlicker
		flicker = lightbulb.gameObject.AddComponent<LightFlicker>();
		flicker.brightness = curve;
		flicker.low_intensity = 0.3f;
		flicker.high_intensity = 0.89f;
		flicker.randomness = 0.05f;

		children = new GameObject[]{floor, wall, wallTop};
	}

	private Mesh Morph(Mesh m, Vector3 a, Vector3 x, Vector3 y, Vector3 z) {
		Mesh result = new Mesh();

		// create new vertices, the Vector Space given by x,y,z, with base at a
		Vector3[] verts = new Vector3[m.vertices.Length];
		//print("verts.Length: "+verts.Length);
		for (int i=0; i<verts.Length; ++i) {
			verts[i] = m.vertices[i]+new Vector3(0.01f, 0, 0.01f);
			verts[i] = a + 100*(verts[i].x*x + verts[i].y*y + verts[i].z*z);
			Vector3 floor = a + 100*(m.vertices[i].x*x + m.vertices[i].z*z);
			verts[i] = MazeStructure.Vector3FromCubeToSphere(verts[i], 10, floor, 10);
			//print("verts["+i+"] = "+verts[i]);
		}
		result.vertices = verts;
		if (a==Vector3.zero)
			print("verts[0]: "+verts[0]+"; orig[0]: "+m.vertices[0]);

		// calculate new normals
		Vector3[] norms = new Vector3[m.normals.Length];
		for (int i=0; i<norms.Length; ++i)
			norms[i] = Vector3.zero;
		result.normals = norms;

		// directly copy uvs
		result.uv = (Vector2[])m.uv.Clone();

		// directly copy triangles
		result.triangles = (int[])m.triangles.Clone();

		return result;
	}

	public void SetBrightness(Color val) {
		for (int i=0; i<children.Length; ++i)
			children[i].renderer.material.color = val;
	}
}
