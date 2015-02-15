using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct Point3 {
	public int x, y, z;

	public int this[int index] {
		get {
			switch (index) {
			case 0:
				return x;
			case 1:
				return y;
			case 2:
				return z;
			default:
				throw new UnityException("invalid index");
			}
		}
		private set {
			switch (index) {
			case 0:
				x = value;
				return;
			case 1:
				y = value;
				return;
			case 2:
				z = value;
				return;
			default:
				throw new UnityException("invalid index");
			}
		}
	}

	public Point3(int x, int y) {
		this.x = x;
		this.y = y;
		this.z = 0;
	}

	public Point3(int x, int y, int z) {
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Point3(Point3 p) {
		this.x = p.x;
		this.y = p.y;
		this.z = p.z;
	}

	public Point3(Point2 p) {
		this.x = p.x;
		this.y = p.y;
		this.z = 0;
	}

	public override string ToString() {
		return "("+x+", "+y+", "+z+")";
	}
}