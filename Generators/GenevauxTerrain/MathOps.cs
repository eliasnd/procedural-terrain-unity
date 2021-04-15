using System;
using System.Collections;
using System.Collections.Generic;

public static class MathOps
{
	// Basic numeric operations
	public static int mod(int v, int b)
	{
		return ((v % b) + b) % b;
	}

	public static float clamp(float v, float a, float b)
	{
		return Math.Max(Math.Min(v, b), a);
	}

	public static float lerp(float a, float b, float f)
	{
		return a + (b - a) * clamp(f, 0, 1);
	}

	public static float inverseLerp(float a, float b, float f)
	{
		return (f - a) / (b - a);
	}

	// Vec2 operations

	public static float angle(Vec2 from, Vec2 to)
	{
		return (float)(Math.Atan2((double)(from.x*to.y - from.y*to.x), (double)(from.x*to.x + from.y*to.y)) + 360) % 360;
	}

	public static float distToLineSegment(Vec2 l1, Vec2 l2, Vec2 p)
	{
		float length = Vec2.Distance(l1, l2);
		if (length == 0)
			return Vec2.Distance(p, l1);
		float t = clamp(Vec2.Dot(p-l1, l2-l1) / (float)Math.Pow(length, 2), 0, 1);
		Vec2 proj = l1 + t * (l2-l1);
		return Vec2.Distance(p, proj);
	}

	public static bool onLineSegment(Vec2 l1, Vec2 l2, Vec2 p)
	{
		if (p == l1 || p == l2)
			return true;
		return (p-l1).normalized == (l2-l1).normalized && (p-l2).normalized == (l1-l2).normalized;
	}

	public static Vec2 rotate(Vec2 vec, float theta)
	{
		double deg2Rad = (Math.PI * 2) / 360;
		float x = (float)Math.Cos((double)theta * deg2Rad) * vec.x - (float)Math.Sin((double)theta * deg2Rad) * vec.y;
		float y = (float)Math.Sin((double)theta * deg2Rad) * vec.x + (float)Math.Cos((double)theta * deg2Rad) * vec.y;

		return new Vec2(x, y);
	}

	public static float distToPolygon(Polygon polygon, Vec2 point)
	{
		float minDist = float.MaxValue;

		for (int c = 0; c < polygon.VertexCount; c++)
			minDist = Math.Min(minDist, distToLineSegment(polygon[c], polygon[(c+1) % polygon.VertexCount], point));

		//Debug.Log(minDist);

		return minDist;
	}

	// LINE OPERATIONS
	// Lines represented as (m, b) => y = mx + b
	// Lines represented as (a, b, c) => c = -bx + ay

	public static Vec2 line(Vec2 a, Vec2 b)
	{
		float m = (a.y - b.y) / (a.x - b.x);
		float i = a.y - m * a.x;

		return new Vec2(m, i);
	}

	public static Vec2 perpendicularBisector(Vec2 a, Vec2 b)
	{
		Vec2 mid = (a + b) * 0.5f;
		Vec2 l = line(a, b);
		
		float m = -1/l.x;
		float i = mid.y - m * mid.x;
		return new Vec2(m, i);
	}

	public static Vec2 intersection(Vec2 l1, Vec2 l2)
	{
		if (l1.x == l2.x)
			return new Vec2(float.MaxValue, float.MaxValue);

		float x = (l2.y-l1.y)/(l1.x-l2.x);
		float y = l1.x * x + l1.y;

		return new Vec2(x, y);
	}

	public static bool onLine(Vec2 p, Vec2 l)
	{
		return (l.x * p.x + l.y == p.y);
	}

	// Polygon operations

	public static Vec2 circumcenter(Vec2 a, Vec2 b, Vec2 c)
	{
		Vec2 b_ab = perpendicularBisector(a, b), b_bc = perpendicularBisector(b, c);

		return intersection(b_ab, b_bc);
	}
}
