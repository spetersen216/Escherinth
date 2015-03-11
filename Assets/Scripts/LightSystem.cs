using UnityEngine;
using System.Collections;
using System;

public class LightSystem:MonoBehaviour {

	public float keyTime=float.MaxValue;
	public Light lightBlueprint;
	public AnimationCurve brightness;
	public float numLightsTurningOff=4f;
	public MazeCell[,,] cells;
	private Pathfinding path;


	public void Init(MazeStructure mazeStruct, GameObject[] walls, GameObject[] tops, GameObject floor, float radius) {
		print(lightBlueprint.name);
		Instantiate(lightBlueprint.gameObject);
		Func<GameObject> getLight = () => {
			return (GameObject)Instantiate(lightBlueprint.gameObject);
		};
		//this.lights = mazeStruct.GetLights(getLight);
		this.cells = mazeStruct.MakeCells (walls, tops, floor, radius);
		this.path = mazeStruct.Pathfind(mazeStruct.FindKey()[0]);
	}


	void Update() {
		keyTime -= Time.deltaTime;

		for (int i=0; i<cells.GetLength(0); ++i)
			for (int j=0; j<cells.GetLength(1); ++j)
				for(int k =0; k < cells.GetLength(2); k++)
					cells[i, j, k].light.intensity = Mathf.Max(Mathf.Min(GetLightAtPoint(i, j), 2), 0);

		cells [1, 0, 1].SetBrightness (Color.white);
	}

	/// <summary>
	/// Returns the appropriate brightness for the light at (i, j)
	/// </summary>
	private float GetLightAtPoint(int i, int j) {
		float result = path.GetGamePos(new Point3(i+1, 1, j+1))+keyTime;
		result = brightness.Evaluate(1-result/numLightsTurningOff);
		return 2*result;
	}
}
