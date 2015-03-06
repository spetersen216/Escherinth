using UnityEngine;
using System.Collections;
using System;

public class LightSystem : MonoBehaviour {
	
	public float keyTime=float.MaxValue;
	public Light lightBlueprint;
	public AnimationCurve brightness;
	public float numLightsTurningOff=4f;
	private Light[,] lights;
	private Pathfinding path;


	public void Init(MazeStructure mazeStruct) {
		print(lightBlueprint.name);
		Instantiate(lightBlueprint.gameObject);
		Func<GameObject> getLight = ()=>{
			return (GameObject)Instantiate(lightBlueprint.gameObject);
		};
		this.lights = mazeStruct.GetLights(getLight);
		this.path = mazeStruct.Pathfind(mazeStruct.FindKey()[0]);
	}
	
	
	void Update () {
		keyTime -= Time.deltaTime;

		for (int i=0; i<lights.GetLength (0); ++i)
			for (int j=0; j<lights.GetLength (1); ++j)
				lights[i, j].intensity = Mathf.Max (Mathf.Min (GetLightAtPoint (i, j), 2), 0);

		lights [1, 0].intensity = 2;
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
