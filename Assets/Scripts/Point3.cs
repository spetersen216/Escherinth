using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct Point3 {
	public int x, y, z;

	/// <summary>Point2(0, 1, 0)</summary>
	public static Point3 up { get { return new Point3(0, 1, 0); } }
	/// <summary>Point2(0, -1, 0)</summary>
	public static Point3 down { get { return new Point3(0, -1, 0); } }
	/// <summary>Point2(-1, 0, 0)</summary>
	public static Point3 left { get { return new Point3(-1, 0, 0); } }
	/// <summary>Point2(1, 0, 0)</summary>
	public static Point3 right { get { return new Point3(1, 0, 0); } }
	/// <summary>Point2(0, 0, 1)</summary>
	public static Point3 forward { get { return new Point3(0, 0, 1); } }
	/// <summary>Point2(0, 0, -1)</summary>
	public static Point3 back { get { return new Point3(0, 0, -1); } }
	/// <summary>Point2(0, 0, 0)</summary>
	public static Point3 zero { get { return new Point3(0, 0, 0); } }
	/// <summary>Point2(1, 1, 1)</summary>
	public static Point3 one { get { return new Point3(1, 1, 1); } }

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

	public static Point3 operator *(Point3 p1, Point3 p2) {
		return new Point3(p1.x*p2.x, p1.y*p2.y, p1.z*p2.z);
	}

	public static Point3 operator *(Point3 p1, int i) {
		return (p1 * new Point3(i, i, i));
	}

	public static Point3 operator /(Point3 p1, Point3 p2) {
		return new Point3(p1.x/p2.x, p1.y/p2.y, p1.z/p2.z);
	}

	public static Point3 operator /(Point3 p1, int i) {
		return (p1 / new Point3(i, i, i));
	}

	public static bool operator ==(Point3 p1, Point3 p2) {
		if (ReferenceEquals(p1, null)!=ReferenceEquals(p2, null))
			return false;
		return (p1.x==p2.x && p1.y==p2.y && p1.z==p2.z);
	}

	public static bool operator !=(Point3 p1, Point3 p2) {
		return !(p1==p2);
	}

	public override bool Equals(object obj) {
		return this==(Point3)obj;
	}

	public override int GetHashCode() {
		return x+y+z;
	}

	/// <summary>
	/// Returns a list of all neighbors for the this Point2.
	/// </summary>
	public Point3[] neighbors() {
		return new Point3[]{
			this+up,
			this+down,
			this+left,
			this+right,
			this+forward,
			this+back
		};
	}

	/// <summary>
	/// Returns a list of all neighbors of this Point2, with the given offset.
	/// </summary>
	public Point3[] neighbors(int offset) {
		return new Point3[]{
			this+(up*offset),
			this+(down*offset),
			this+(left*offset),
			this+(right*offset),
			this+(forward*offset),
			this+(back*offset)
		};
	}

	/// <summary>
	/// Returns a list of all neighbors of this Point2, in the given range, with the given offset.
	/// </summary>
	public Point3[] neighborsInRange(Point3 minVal, Point3 maxVal, int offset) {
		Point3[] neighbors = (offset==1?this.neighbors():this.neighbors(offset));
		int count=0;

		// count valid neighbors
		foreach (Point3 p in neighbors)
			if (p.x>=minVal.x && p.y>=minVal.y && p.z>=minVal.z && p.x<maxVal.x && p.y<maxVal.y && p.z<maxVal.z)
				++count;

		// accumulate the result
		Point3[] result = new Point3[count];
		foreach (Point3 p in neighbors)
			if (p.x>=minVal.x && p.y>=minVal.y && p.z>=minVal.z && p.x<maxVal.x && p.y<maxVal.y && p.z<maxVal.z)
				result[--count] = p;
		return result;
	}

	/// <summary>
	/// Randomizes the order of the given Point3[].
	/// </summary>
	public static Point3[] Scramble(Point3[] points) {
		for (int i=0; i<points.Length; ++i) {
			int j = UnityEngine.Random.Range(i, points.Length);
			Point3 temp = points[i];
			points[i] = points[j];
			points[j] = temp;
		}
		return points;
	}
}
