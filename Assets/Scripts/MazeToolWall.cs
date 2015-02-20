using UnityEngine;
using System.Collections;
using System;

public class MazeToolWall:MazeToolComponent {
	
	private string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
	public enum WallType {normal, door};
	public WallType type=WallType.normal;
	
	public override string ToString() {
		return chars[(((int)type)<<1)+(gameObject.activeSelf?1:0)].ToString();
	}
	
	public void FromString(string str, int version) {
		switch(version) {
		case 0:
			gameObject.SetActive(true);
			type = WallType.normal;
			break;
		case 1:
			gameObject.SetActive((chars.IndexOf(str)&1)==1);
			type = (WallType)(chars.IndexOf(str)>>1);
			break;
		default:
			break;
		}
	}

	
}
