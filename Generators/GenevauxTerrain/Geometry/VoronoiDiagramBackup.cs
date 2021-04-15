using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple Voronoi Diagram implementation using Delaunay triangulation
// Diagram represented as vertices of cell of each passed point


/* public static class VoronoiDiagram
{
	public static List<Vector2>[] Generate(Vector2[] points, Vector2? nBounds=null)
	{
		List<Vector2>[] diagram = new List<Vector2>[points.Length];

		for (int i = 0; i < points.Length; i++)
			diagram[i] = new List<Vector2>();

		//Calculate Delauney triangulation
		List<Vector2> voronoiVertices = new List<Vector2>();

		// Delaunay algorithm
		for (int a = 0; a < points.Length; a++)
			for (int b = a+1; b < points.Length; b++)
				for (int c = b+1; c < points.Length; c++)
				{
					bool valid = true;

					Vector2 pointA = points[a], pointB = points[b], pointC = points[c]; 
					float distAB = Vector2.Distance(pointA, pointB), distBC = Vector2.Distance(pointB, pointC), distCA = Vector2.Distance(pointC, pointA);

					float radius = (distAB * distBC * distCA) / Mathf.Sqrt((distAB+distBC+distCA) * (distBC+distCA-distAB) * (distCA+distAB-distBC) * (distAB+distBC-distCA));	// Radius of circumcircle
					Vector2 circumcenter = MathOps.circumcenter(pointA, pointB, pointC);

					if (circumcenter == new Vector2(float.MaxValue, float.MaxValue))	// Circumenter calculation failed
						valid = false;
					else
					{
						for (int i = 0; i < points.Length && valid; i++)
							if (i != a && i != b && i != c && Vector2.Distance(points[i], circumcenter) < radius)	// Fail if any other point in radius of circumcenter
								valid = false;
					}

					// Add if valid
					// Note: Not sure how to handle points outside of texture -- since not needed for crests, just not including
					if (valid)
					{
						if (nBounds != null)
						{
							Vector2 bounds = (Vector2)nBounds;
							if (circumcenter.x > 0 && circumcenter.x < bounds.x && circumcenter.y > 0 && circumcenter.y < bounds.y)
							{
								diagram[a].Add(circumcenter);
								diagram[b].Add(circumcenter);
								diagram[c].Add(circumcenter);
							}
						}
						else
						{
							diagram[a].Add(circumcenter);
							diagram[b].Add(circumcenter);
							diagram[c].Add(circumcenter);
						}
					}
				}

		for (int i = 0; i < points.Length; i++)
			for (int j = 0; j < diagram[i].Count-1; j++)	// Sort vertices clockwise order -- selection sort ok for small sizes
			{
				Vector2 direction = diagram[i][j] - points[i];
				int min = j+1;
				for (int k = j+2; k < diagram[i].Count; k++)
				{
					Vector2 minDir = diagram[i][min] - points[i];
					Vector2 targetDir = diagram[i][k] - points[i];
					if (MathOps.angle(targetDir, direction) < MathOps.angle(minDir, direction))
						min = k;
				}

				Vector2 temp = diagram[i][j+1];
				diagram[i][j+1] = diagram[i][min];
				diagram[i][min] = temp;
			}

		return diagram;
	}
}*/