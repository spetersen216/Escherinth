using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct Point2 {
	public int x, y;
	
	/// <summary>Point2(0, 1)</summary>
	public static Point2 up { get { return new Point2(0, 1); } }
	/// <summary>Point2(0, -1)</summary>
	public static Point2 down { get { return new Point2(0, -1); } }
	/// <summary>Point2(-1, 0)</summary>
	public static Point2 left { get { return new Point2(-1, 0); } }
	/// <summary>Point2(1, 0)</summary>
	public static Point2 right { get { return new Point2(1, 0); } }
	/// <summary>Point2(0, 0)</summary>
	public static Point2 zero { get { return new Point2(0, 0); } }
	/// <summary>Point2(1, 1)</summary>
	public static Point2 one { get { return new Point2(1, 1); } }

	public int this[int index] {
		get {
			switch (index) {
			case 0:
				return x;
			case 1:
				return y;
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
			default:
				throw new UnityException("invalid index");
			}
		}
	}

	public Point2(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public Point2(Point2 p) {
		this.x = p.x;
		this.y = p.y;
	}

	public override string ToString() {
		return "("+x+", "+y+")";
	}

	public static Point2 operator +(Point2 p1, Point2 p2) {
		return new Point2(p1.x+p2.x, p1.y+p2.y);
	}

	public static Point2 operator +(Point2 p1, int i) {
		return (p1 + new Point2(i, i));
	}

	public static Point2 operator -(Point2 p1, Point2 p2) {
		return new Point2(p1.x-p2.x, p1.y-p2.y);
	}

	public static Point2 operator -(Point2 p1, int i) {
		return (p1 - new Point2(i, i));
	}

	public static Point2 operator *(Point2 p1, Point2 p2) {
		return new Point2(p1.x*p2.x, p1.y*p2.y);
	}

	public static Point2 operator *(Point2 p1, int i) {
		return (p1 * new Point2(i, i));
	}

	public static Point2 operator /(Point2 p1, Point2 p2) {
		return new Point2(p1.x/p2.x, p1.y/p2.y);
	}

	public static Point2 operator /(Point2 p1, int i) {
		return (p1 / new Point2(i, i));
	}

	public static bool operator ==(Point2 p1, Point2 p2) {
		if (ReferenceEquals(p1, null)!=ReferenceEquals(p2, null))
			return false;
		return (p1.x==p2.x && p1.y==p2.y);
	}

	public static bool operator !=(Point2 p1, Point2 p2) {
		return !(p1==p2);
	}

	public override bool Equals(object obj) {
		return this==(Point2)obj;
	}

	public override int GetHashCode() {
		return x+y;
	}

	/// <summary>
	/// Returns a list of all neighbors for the this Point2.
	/// </summary>
	public Point2[] neighbors() {
		return new Point2[]{
			this+up,
			this+down,
			this+left,
			this+right
		};
	}

	/// <summary>
	/// Returns a list of all neighbors of this Point2, with the given offset.
	/// </summary>
	public Point2[] neighbors(int offset) {
		return new Point2[]{
			this+(up*offset),
			this+(down*offset),
			this+(left*offset),
			this+(right*offset)
		};
	}

	/// <summary>
	/// Returns a list of all neighbors of this Point2, in the given range, with the given offset.
	/// </summary>
	public Point2[] neighborsInRange(Point2 minVal, Point2 maxVal, int offset) {
		Point2[] neighbors = (offset==1?this.neighbors():this.neighbors(offset));
		int count=0;

		// count valid neighbors
		foreach (Point2 p in neighbors)
			if (p.x>=minVal.x && p.y>=minVal.y && p.x<maxVal.x && p.y<maxVal.y)
				++count;

		// accumulate the result
		Point2[] result = new Point2[count];
		foreach (Point2 p in neighbors)
			if (p.x>=minVal.x && p.y>=minVal.y && p.x<maxVal.x && p.y<maxVal.y)
				result[--count] = p;
		return result;
	}

	/// <summary>
	/// Randomizes the order of the given Point3[].
	/// </summary>
	public static Point2[] Scramble(Point2[] points) {
		for (int i=0; i<points.Length; ++i) {
			int j = UnityEngine.Random.Range(i, points.Length);
			Point2 temp = points[i];
			points[i] = points[j];
			points[j] = temp;
		}
		return points;
	}
}