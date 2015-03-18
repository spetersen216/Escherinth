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
		Material cellWallTopMat, AnimationCurve curve, Vector3 a, Vector3 x1, Vector3 x2, Vector3 y, Vector3 z1, Vector3 z2) {
			if (pos==Point3.one)
				print("pos == Point3.one: "+a);

		// create cell floor
		floor = new GameObject("floor");
		floor.AddComponent<MeshFilter>().mesh = Morph(cellFloor, a, x1, x2, y, z1, z2);
		floor.AddComponent<MeshRenderer>().material = (Material)Instantiate(cellFloorMat);
		floor.transform.parent = transform;

		// create cell wall
		/*wall = new GameObject("cell wall");
		wall.AddComponent<MeshFilter>().mesh = Morph(cellWall, a, x, y, z);
		wall.AddComponent<MeshRenderer>().material = (Material)Instantiate(cellWallMat);
		wall.transform.parent = transform;

		// create cell wall top
		wallTop = new GameObject("cell wall top");
		wallTop.AddComponent<MeshFilter>().mesh = Morph(cellWallTop, a, x, y, z);
		wallTop.AddComponent<MeshRenderer>().material = (Material)Instantiate(cellWallTopMat);
		wallTop.transform.parent = transform;*/

		// initialize lightbulb
		/*lightbulb = new GameObject("light").AddComponent<Light>();
		lightbulb.transform.position = MazeStructure.Vector3FromCubeToSphere(a+0.5f*(x+z)+2*y, 10, a+0.5f*(x+z), 10);
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
		flicker.randomness = 0.05f;*/

		children = new GameObject[]{floor, wall, wallTop};
	}

	private Mesh Morph(Mesh m, Vector3 a, Vector3 x1, Vector3 x2, Vector3 y, Vector3 z1, Vector3 z2) {
		Mesh result = new Mesh();

		// create new vertices, the Vector Space given by x,y,z, with base at a
		Vector3[] verts = new Vector3[m.vertices.Length];
		//print("verts.Length: "+verts.Length);
		for (int i=0; i<verts.Length; ++i) {
			verts[i] = m.vertices[i]+new Vector3(0.01f, 0, 0.01f);
			float vx = verts[i].x*100;
			float vz = verts[i].z*100;
			Vector3 floor = a + 100*(verts[i].x*(x1*(1-vz)+x2*(vz-0)) + verts[i].z*(z1*(1-vx)+z2*(vx-0)));
			verts[i] = floor + 100*verts[i].y*y;
			//verts[i] = MazeStructure.Vector3FromCubeToSphere(verts[i], 10, floor, 10);
		}
		result.vertices = verts;
		if (a==Vector3.zero)
			print("verts[0]: "+verts[0]+"; orig[0]: "+m.vertices[0]);

		// directly copy uvs and triangles
		result.uv = (Vector2[])m.uv.Clone();
		result.triangles = (int[])m.triangles.Clone();

		result.RecalculateNormals();
		result.Optimize();
		return result;
	}

	public void SetBrightness(Color val) {
		for (int i=0; i<children.Length; ++i)
			if (children[i]!=null)
				children[i].renderer.material.color = val;
	}
}
