﻿using UnityEngine;
using System.Collections;
using System;

public class LightSystem:MonoBehaviour {

	public float keyTime=float.MaxValue;
	public Light lightBlueprint;
	public AnimationCurve brightness;
	public float numLightsTurningOff=4f;
	public MazeCell[,,] cells;
	private Pathfinding path;
	public String test;


	public void Init(MazeStructure mazeStruct, MazeCell[,,] cells) {
		this.cells = cells;
		this.path = mazeStruct.Pathfind(mazeStruct.FindKey()[0]);
	}

	void Awake(){
		this.test = "stay";
	}


	void Update() {
		keyTime -= Time.deltaTime;
		if (this.cells != null) {

			//print(cells.GetLength(0));
			for (int i=1; i<this.cells.GetLength(0); ++i) {
				for (int j=1; j<cells.GetLength(1); ++j) {
					for (int k =1; k<cells.GetLength(2); k++) {
						if (cells[i, j, k] != null) {
							byte value = (byte)Mathf.Max(Mathf.Min(GetLightAtPoint(i, j, k), 255), 0);
							cells[i, j, k].SetBrightness(new Color32(value, value, value, 255)); //= Mathf.Max(Mathf.Min(GetLightAtPoint(i, j), 2), 0);
						}
					}
				}
			}
			
			//cells [1, 0, 1].SetBrightness (Color.white);
		}
	}

	/// <summary>
	/// Returns the appropriate brightness for the light at (i, j)
	/// </summary>
	private float GetLightAtPoint(int i, int j, int k) {
		float result = path.GetDistanceToEnd(new Point3(i, j, k))*1.2f+keyTime;
		result = brightness.Evaluate(1-result/numLightsTurningOff);
		return 255*result;
	}
}
