using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct Point2 {
	public int x, y;

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

	public static bool operator >(Point2 p1, Point2 p2) {
		return p1.x>p2.x || p1.y>p2.y;
	}

	public static bool operator <(Point2 p1, Point2 p2) {
		return p1.x<p2.x || p1.y<p2.y;
	}
}