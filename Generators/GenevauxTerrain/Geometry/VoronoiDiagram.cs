using System;
using System.Collections;
using System.Collections.Generic;

using csDelaunay;

// Simple Voronoi Diagram implementation using Delaunay triangulation
// Diagram represented as vertices of cell of each passed point


public static class VoronoiDiagram
{
	public static List<Polygon> Generate(List<Vec2> points, Polygon bounds)
	{
		List<Vector2f> pointsf = new List<Vector2f>();
		for (int i = 0; i < points.Count; i++)
			pointsf.Add(new Vector2f(points[i].x, points[i].y));

		float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
		for (int i = 0; i < bounds.VertexCount; i++)
		{
			minX = Math.Min(minX, bounds[i].x );
			maxX = Math.Max(maxX, bounds[i].x);
			minY = Math.Min(minY, bounds[i].y);
			maxY = Math.Max(maxY, bounds[i].y);
		}

		Rectf rect = new Rectf(minX, minY, maxX-minX, maxY-minY);
		Voronoi voronoi = new Voronoi(pointsf, rect);

		List<Polygon> generated = new List<Polygon>();
		for (int i = 0; i < points.Count; i++)
		{
			List<Vec2> region = new List<Vec2>();
			foreach (Vector2f point in voronoi.Region(pointsf[i]))
				region.Add(new Vec2(point.x, point.y));
			// generated.Add(Polygon.Intersection(new Polygon(region.ToArray()), bounds));
			generated.Add(new Polygon(region.ToArray()));
		}
		return generated;
	}
}