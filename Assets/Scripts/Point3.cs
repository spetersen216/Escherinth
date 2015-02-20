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
	
	public Point3(Vector3 v) {
		this.x = (int)v.x;
		this.y = (int)v.y;
		this.z = (int)v.z;
	}

	public override string ToString() {
		return "("+x+", "+y+", "+z+")";
	}
	
	public Vector3 ToVector3() {
		return new Vector3(x, y, z);
	}
	
	public static Point3 operator +(Point3 p1, Point3 p2) {
		return new Point3(p1.x+p2.x, p1.y+p2.y, p1.z+p2.z);
	}
	
	public static Point3 operator +(Point3 p1, int i) {
		return (p1 + new Point3(i, i, i));
	}
	
	public static Point3 operator -(Point3 p1, Point3 p2) {
		return new Point3(p1.x-p2.x, p1.y-p2.y, p1.z-p2.z);
	}
	
	public static Point3 operator -(Point3 p1, int i) {
		return (p1 - new Point3(i, i, i));
	}
}