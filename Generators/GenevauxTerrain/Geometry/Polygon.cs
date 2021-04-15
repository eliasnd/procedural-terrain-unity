using System.Collections.Generic;
using System.Linq;
using System;
public class Polygon : object
{
	public enum Direction { CW, CCW }

	Vec2[] vertices;

	public float Area { 
		get {
			float area = 0;
			int j = vertices.Length-1;

			for (int i = 0; i < vertices.Length; i++)
			{
				area += (vertices[j].x + vertices[i].x) * (vertices[j].y - vertices[i].y);
				j = i;
			}

			return Math.Abs(area/2);
		}}

	public int VertexCount { get { return vertices.Length; } }

	public Vec2 this[int index] {
		get { return vertices[index]; }
	}

	public Polygon(Vec2[] boundary)
	{
		vertices = boundary;
	}

	public bool Contains(Vec2 point)		// Polygon strictly contains point -- not on boundary
	{
		if (OnBoundary(point))
			return false;

		int intersectionCount = 0;
		for (int i = 0; i < vertices.Length; i++)
		{
			Vec2 l1 = vertices[i];
			Vec2 l2 = vertices[(i+1) % vertices.Length];

			if (l1.y > point.y && l2.y <= point.y)			// Line intersects ray, l1 above l2
			{			
				Vec2 diff = l1 - l2;
				float factor = MathOps.inverseLerp(l2.y, l1.y, point.y);		// Proportion between l1 and l2 where y is
				float x0 = l2.x + diff.x * factor;
				if (x0 > point.x)
					intersectionCount++;
			}
			else if (l1.y < point.y && l2.y >= point.y)	// Line intersects ray, l2 above l1
			{
				Vec2 diff = l2 - l1;
				float factor = MathOps.inverseLerp(l1.y, l2.y, point.y);		// Proportion between l1 and l2 where y is
				float x0 = l1.x + diff.x * factor;
				if (x0 > point.x)
					intersectionCount++;
			}
		}

		return intersectionCount % 2 == 1;
	}

	public bool OnBoundary(Vec2 point)
	{
		for (int i = 0; i < vertices.Length; i++)
		{
			Vec2 l1 = vertices[i], l2 = vertices[(i+1) % vertices.Length];
			if (MathOps.onLineSegment(l1, l2, point))
				return true;
		}

		return false;
	}

	public static Polygon Intersection(Polygon a, Polygon b)
	{
		List<Vec2> aInB = new List<Vec2>();	// Vertices of a in b
		List<Vec2> bInA = new List<Vec2>();	// Vertices of b in a
		List<Vec2> intersections = new List<Vec2>();	// Intersection points

		List<Vec2> aBoundary = new List<Vec2>();
		List<Vec2> bBoundary = new List<Vec2>();

		// Find all vertices and fill lists

		for (int v1 = 0; v1 < a.VertexCount; v1++)	// Find aInB
			if (b.Contains(a[v1]))
				aInB.Add(a[v1]);

		for (int v2 = 0; v2 < b.VertexCount; v2++)	// Find bInA
			if (a.Contains(b[v2]))
				bInA.Add(b[v2]);

		for (int v1 = 0; v1 < a.VertexCount; v1++)	// Find intersections
		{
			Vec2 a1 = a[v1], a2 = a[(v1+1)%a.VertexCount];
			for (int v2 = 0; v2 < b.VertexCount; v2++)
			{
				Vec2 b1 = b[v2], b2 = b[(v2+1)%b.VertexCount];
				Vec2 intersection = MathOps.intersection(MathOps.line(a1, a2), MathOps.line(b1, b2));		// Test intersection of every pair of edges
				if (MathOps.onLineSegment(a1, a2, intersection) && MathOps.onLineSegment(b1, b2, intersection))	// Line segments intersect
					intersections.Add(intersection);

			}
		}

		if (intersections.Count == 0)
			if (aInB.Count == 0)
				return b;
			else if (bInA.Count == 0)
				return a;
		
		// Construct complete A and B

		List<Vec2> aComplete = new List<Vec2>();
		List<Vec2> bComplete = new List<Vec2>();

		for (int v1 = 0; v1 < a.VertexCount; v1++)
		{
			Vec2 a1 = a[v1], a2 = a[(v1+1)%a.VertexCount];
			Vec2 line = MathOps.line(a1, a2);
			List<Vec2> edgeIntersections = new List<Vec2>();

			for (int i = 0; i < intersections.Count; i++)
				if (MathOps.onLineSegment(a1, a2, intersections[i]))
					edgeIntersections.Add(intersections[i]);

			int DistToEdge(Vec2 i1, Vec2 i2)
			{
				if (Vec2.Distance(a1, i1) < Vec2.Distance(a1, i2))
					return -1;
				else if (Vec2.Distance(a1, i1) > Vec2.Distance(a1, i2))
					return 1;
				return 0;
			}

			edgeIntersections.Sort(DistToEdge);

			if (aInB.Contains(a1))
				aComplete.Add(a1);
			aComplete.AddRange(edgeIntersections);
		}

		for (int v2 = 0; v2 < b.VertexCount; v2++)
		{
			Vec2 b1 = b[v2], b2 = b[(v2+1)%b.VertexCount];
			Vec2 line = MathOps.line(b1, b2);
			List<Vec2> edgeIntersections = new List<Vec2>();

			for (int i = 0; i < intersections.Count; i++)
				if (MathOps.onLineSegment(b1, b2, intersections[i]))
					edgeIntersections.Add(intersections[i]);

			int DistToEdge(Vec2 i1, Vec2 i2)
			{
				if (Vec2.Distance(b1, i1) < Vec2.Distance(b1, i2))
					return -1;
				else if (Vec2.Distance(b1, i1) > Vec2.Distance(b1, i2))
					return 1;
				return 0;
			}

			edgeIntersections.Sort(DistToEdge);

			if (bInA.Contains(b1))
				bComplete.Add(b1);
			bComplete.AddRange(edgeIntersections);
		}

		List<Vec2> result = new List<Vec2>();
		List<Vec2> currList = aComplete;
		int index;

		void BuildResults()
		{
			while (!result.Contains(currList[index]))		// Loop until return to added point
			{
				result.Add(currList[index]);

				if (intersections.Contains(currList[index]))
				{
					Vec2 intersection = currList[index];
					currList = currList == aComplete ? bComplete : aComplete;
					index = currList.IndexOf(intersection);
				}
				
				if (result.Contains(currList[(index+1)%currList.Count]) && !result.Contains(currList[MathOps.mod(index-1, currList.Count)]))	// To figure out order, check to see which hasn't been added
					index = MathOps.mod(index-1, currList.Count);
				else
					index = (index+1)%currList.Count;
			}
		}

		bool AllAdded()				// Test if all have been added
		{							 
			for (int v1 = 0; v1 < aComplete.Count; v1++)
				if (!result.Contains(aComplete[v1]))
					return false;
			for (int v2 = 0; v2 < bComplete.Count; v2++)
				if (!result.Contains(bComplete[v2]))
					return false;
			return true;
		}

		for (int i = 0; !AllAdded() && i < aComplete.Count; i++)	// Test different start positions for a
		{
			result = new List<Vec2>();
			currList = aComplete;
			index = i;
			BuildResults();
		}

		for (int i = 0; !AllAdded() && i < bComplete.Count; i++)	// Same for b
		{
			result = new List<Vec2>();
			currList = bComplete;
			index = i;
			BuildResults();
		}

		return new Polygon(result.ToArray());
	}
}