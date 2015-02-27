using UnityEngine;
using System.Collections;
using System;

public class LightSystem : MonoBehaviour {
	
	public float keyTime=float.MaxValue;
	public Light lightBlueprint;
	private Light[,] lights;
	private Pathfinding path;

	// Use this for initialization
	public void Init(MazeStructure mazeStruct) {
		print(lightBlueprint.name);
		Instantiate(lightBlueprint.gameObject);
		Func<GameObject> getLight = ()=>{
			return (GameObject)Instantiate(lightBlueprint.gameObject);
		};
		this.lights = mazeStruct.GetLights(getLight);
		this.path = mazeStruct.Pathfind(mazeStruct.FindKey()[0]);
	}
	
	// Update is called once per frame
	void Update () {
		keyTime -= Time.deltaTime;

		for (int i=0; i<lights.GetLength (0); ++i)
			for (int j=0; j<lights.GetLength (1); ++j)
				lights[i, j].intensity = Mathf.Max (Mathf.Min (GetLightAtPoint (i, j), 2), 0);

		lights [1, 0].intensity = 2;
	}

	private float GetLightAtPoint(int i, int j) {
		float result = (path.GetGamePos(new Point3(i+1, 1, j+1))+keyTime);
		if (result<1)
			return 0;
		return result;
	}
}
