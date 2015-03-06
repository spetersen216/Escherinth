using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell:MonoBehaviour {
	public void Init() {

	}

	public void SetBrightness(Color val) {
		renderer.material.color = val;
	}
}
