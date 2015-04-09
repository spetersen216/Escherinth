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
	public String test;
	private AudioSource sound;

	public void Init(MazeStructure mazeStruct, MazeCell[,,] cells, AudioSource lightOff) {
		this.cells = cells;
		this.path = mazeStruct.Pathfind(mazeStruct.FindKey()[0]);
		this.sound = lightOff;
	}


	void Update() {
		keyTime -= Time.deltaTime;
		if (this.cells != null) {

			//print(cells.GetLength(0));
			for (int i=1; i<this.cells.GetLength(0); ++i) {
				for (int j=1; j<cells.GetLength(1); ++j) {
					for (int k =1; k<cells.GetLength(2); k++) {
						if (cells[i, j, k] != null) {
							byte value = (byte)Mathf.Max(Mathf.Min(GetLightAtPoint(i, j, k), 255), 130);
							cells[i, j, k].SetBrightness(new Color32(value, value, value, 255)); //= Mathf.Max(Mathf.Min(GetLightAtPoint(i, j), 2), 0);
							if(!this.sound.isPlaying && this.keyTime < 0 && this.keyTime > -45){
								this.sound.volume = .5f;
								this.sound.Play();
								//Debug.Log ("KEYTIME ***** " + this.keyTime);
							}	
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Returns the appropriate brightness for the light at (i, j, k)
	/// </summary>
	private float GetLightAtPoint(int i, int j, int k) {
		float result = path.GetDistanceToEnd(new Point3(i, j, k))*1.2f+keyTime;
		result = brightness.Evaluate(1-result/numLightsTurningOff);
		return 255*result;
	}
}
