/*
Credit:
Génevaux et. al.
Terrain Generation Using Procedural Models Based on Hydrology
*/

using System;
using System.Collections;
using System.Collections.Generic;

public class GenevauxTerrain
{
	// Input parameters
	public float[,] riverSlopeMap;
	public float[,] terrainSlopeMap;
	public Vec2 mapSize;	//Map size in km
	public Vec2[] definedRiverMouths;

	static float scale = 0.75f;

	//World parameters -- uv (0.0 - 1.0), mc (map coords), or f (factor)
	int riverMouthCount = 4;	// How many total (defined + generated) river mouths there are
	float minContourDist = 500;	// How far new nodes need to be from contour
	float contourSideLength = 0.025f;	// How long each side of the contour is (uv)
	float elevationScale = 30;	// Scale of heights		(f)
	float elevationRange = 5;	// Range of consideration for node selection	(f)
	float edgeLength = 2000;	// Length of edges between river nodes 	(mc)
	float minEdgeDist = 1500;	// Minimum distance between all edges Note: Not * f(s)	(mc)
	float pCont = 0.3f;		// Probability of river continuation		(f)
	float pSym = 0.4f;		// Probability of symmetrical river split	(f)
	float pAsym = 0.3f;		// Probability of asymmetrical river split	(f)
	float maxSlope = 1.5f;	// Maximum slope increase relative to distance from parent node

	//Heuristics -- a vs b, 0 => only a, 1 => only b
	float riverMouthHeuristic = 0.5f;	// Concavity vs Distance

	//Thresholds
	float riverDistThreshold = 5000;	// Distance threshold for river delta types
	float riverFlowThreshold = 10;		// Flow threshold for river delta types

	// Data structures	
	public List<GenevauxNode> riverNodes;
	public List<GenevauxNode> riverMouths;
	public List<GenevauxNode> candidateNodes;
	//Vec2[] contour;
	Polygon contour;

	//Constants
	GenevauxNode nullNode;

	// Main algorithm
	public void Generate(float[,] rMap, float[,] tMap)//, Vec2 mapSize, Vec2[] riverMouths = null)
	{
		UnityEngine.Random.InitState(1000);
		nullNode = new GenevauxNode(new Vec2(-1, -1));
		mapSize = new Vec2(35000 * scale, 35000 * scale);

		riverSlopeMap = rMap;
		terrainSlopeMap = tMap;
		riverNodes = new List<GenevauxNode>();
		riverMouths = new List<GenevauxNode>();
		candidateNodes = new List<GenevauxNode>();

		Vec2[] contourArr = ExtractContour(riverSlopeMap, contourSideLength);
		if (contourArr == null)
			return;

		for (int i = 0; i < contourArr.Length; i++)
			contourArr[i] *= mapSize;

		contour = new Polygon(contourArr);

		//ProcessUserDefinedRivers();
		InitCandidateNodes();

		bool completed = false;
		while (!completed)
		{
			GenevauxNode node = SelectNode();

			if (node.Equals(nullNode))	// All nodes expanded
				completed = true;
			else
				ExpandNode(node);
		}

		// Generate Voronoi Diagram

		List<Vec2> positions = new List<Vec2>();
		for (int i = 0; i < riverNodes.Count; i++)
			positions.Add(riverNodes[i].Pos);

		List<Polygon> voronoiCells = VoronoiDiagram.Generate(positions, new Polygon(new Vec2[] { new Vec2(0, 0), new Vec2(0, mapSize.y-1), new Vec2(mapSize.x-1, 0), new Vec2(mapSize.x-1, mapSize.y-1) }));
		for (int i = 0; i < riverNodes.Count; i++)
			riverNodes[i].SetBoundary(Polygon.Intersection(contour, voronoiCells[i]));
		
		/*Debug.Log("Boundary of " + riverNodes[0].Pos.x + ", " + riverNodes[0].Pos.y);
			for(int j = 0; j < voronoiCells[0].Count; j++)
				Debug.Log("Edge from " + voronoiCells[0][j].x + ", " + voronoiCells[0][j].y + " -- " + voronoiCells[0][(j+1)%voronoiCells[0].Count].x + ", " + voronoiCells[0][(j+1)%voronoiCells[0].Count].y);
		foreach (Node child in riverNodes[0].Children)
		{
			Debug.Log("Boundary of " + child.Pos.x + ", " + child.Pos.y);
			for(int j = 0; j < child.Boundary.Count; j++)
				Debug.Log("Edge from " + child.Boundary[j].x + ", " + child.Boundary[j].z + " -- " + child.Boundary[(j+1)%child.Boundary.Count].x + ", " + child.Boundary[(j+1)%child.Boundary.Count].z);
		}*/

		//CalculateCrests();
		// ClassifyRivers();
		// GeneratePrimitives();
	}

	public static Vec2[] ExtractContour(float[,] tex, float sideLength)
	{
		bool OnContour(Vec2 point)
		{
			if (point.x < 0 || point.x >= tex.GetLength(1) || point.y < 0 || point.y >= tex.GetLength(0))
				return false;

			if (tex[(int)point.y, (int)point.x] == 0)
				return false;

			for (int o = -1; o <= 1; o++)
				if (tex[(int)point.y+o, (int)point.x] == 0 || tex[(int)point.y, (int)point.x+o] == 0)
					return true;

			return false;
		}

		List<Vec2> fullContour = new List<Vec2>();

		Vec2 dim = new Vec2(tex.GetLength(0), tex.GetLength(1));
		Vec2[] directions = new Vec2[] { new Vec2(1, 0), new Vec2(0, -1), new Vec2(0, 1), new Vec2(-1, 0), new Vec2(1, -1), new Vec2(1, 1), new Vec2(-1, -1), new Vec2(-1, 1) };

		Vec2 start = Vec2.zero;
		bool startFound = false;
		for (int y = 1; y < tex.GetLength(1)-1 && !startFound; y++)			//Choose bounds to not have to deal with contour on edge
			for (int x = 1; x < tex.GetLength(1)-1 && !startFound; x++)
				if (tex[(int)y, (int)x] != 0)
				{
					start = new Vec2(x, y); 
					startFound = true;
				}

		if (!startFound)		//In case no contour
			return null;

		fullContour.Add(start / dim);

		Vec2 curr = start;

		foreach (Vec2 dir in directions)
			if (OnContour(start + dir))
			{
				curr = start + dir;
				break;
			}

		while (curr != start && curr != Vec2.zero)
		{
			fullContour.Add(curr / dim);	//Contour in uv coords
			Vec2 next = Vec2.zero;

			for (int i = 1; i < 5 && next == Vec2.zero; i++)	// Iteratively widen search if no next point is found
			{
				foreach (Vec2 dir in directions)
					if (OnContour(curr + dir * i) && !fullContour.Contains((curr + dir * i) / dim))
					{
						next = curr + dir * i;
						break;
					}
			}

			curr = next;
		}

		List<Vec2> selectContour = new List<Vec2>();

		Vec2 last = start;

		for (int i = 1; i < fullContour.Count; i++)
			if (Vec2.Distance(fullContour[i], last) >= sideLength)
			{
				selectContour.Add(fullContour[i]);
				last = fullContour[i];
			}

		return selectContour.ToArray();
	}

	void InitCandidateNodes() 
	{
		// Cannot have more river mouths than vertices on contour
		riverMouthCount = Math.Min(riverMouthCount, contour.VertexCount);	

		float MinNodeDist(Vec2 pos)
		{ 
			if (riverNodes.Count == 0)
				return -1;

			float min = float.MaxValue;
			//Debug.Log("pos is " + (pos / mapSize).x + ", " + (pos / mapSize).y);
			for (int i = 0; i < riverNodes.Count; i++) { min = Math.Min(Vec2.Distance(riverNodes[i].Pos / mapSize, pos / mapSize), min); }
			return min;
		};

		bool[] concavePoints = new bool[contour.VertexCount];	// 0 if point is convex, 1 if concave
		int[] concaveScore = new int[contour.VertexCount];		// How concave points are
		float[] candidateScore = new float[contour.VertexCount];	// Total heuristic score

		// Calculate concavity

		for (int i = 0; i < contour.VertexCount; i++)
		{
			Vec2 prev = contour[MathOps.mod(i-1, contour.VertexCount)] - contour[i];
			Vec2 next = contour[(i+1)%(contour.VertexCount)] - contour[i];
			concavePoints[i] = (MathOps.angle(next, prev) < 180);
		}

		for (int i = 0; i < contour.VertexCount; i++)
		{
			int c = 0;
			while (i-c > 0 && i+c < contour.VertexCount && concavePoints[(i+c)%(contour.VertexCount)] && concavePoints[MathOps.mod(i-c, contour.VertexCount)]) { c++; }
			concaveScore[i] = c;
		}

		// Calculate heuristic and get nodes

		List<int> used = new List<int>();

		for (int n = 0; n < riverMouthCount; n++)
		{
			if (riverNodes.Count > 0)
				for (int i = 0; i < contour.VertexCount; i++)
					candidateScore[i] = concaveScore[i] * (1-riverMouthHeuristic) + MinNodeDist(contour[i]) * riverMouthHeuristic;
			else
				for (int i = 0; i < contour.VertexCount; i++)
					candidateScore[i] = (float)concaveScore[i];

			int max = 0;

			while (used.Contains(max)) { max++; }	//Since riverMouthCount < contour.VertexCount, always unused points

			for (int i = max+1; i < contour.VertexCount; i++)
				if (candidateScore[i] > candidateScore[max] && !used.Contains(i))
					max = i;

			GenevauxNode mouth = new GenevauxNode(contour[max], null, 4);
			riverNodes.Add(mouth);
			riverMouths.Add(mouth);
			candidateNodes.Add(mouth);

			//Debug.Log(contour[max]);

			used.Add(max);
		}
	}

	GenevauxNode SelectNode()
	{
		float minElevation = float.MaxValue;

		for (int i = 0; i < candidateNodes.Count; i++)
		{
			GenevauxNode node = candidateNodes[i];
			if (node.Elevation < minElevation)
				minElevation = node.Elevation;
		}

		if (minElevation == float.MaxValue)		// No candidates
			return nullNode;

		List<GenevauxNode> inRange = new List<GenevauxNode>();

		for (int i = 0; i < candidateNodes.Count; i++)
		{
			GenevauxNode node = candidateNodes[i];
			if (node.Elevation < minElevation + elevationRange)
				inRange.Add(node);
		}

		int maxPriority = int.MinValue;

		for (int i = 0; i < inRange.Count; i++)
			maxPriority = Math.Max(maxPriority, inRange[i].Priority);

		for (int i = 0; i < inRange.Count; i++)
			if (inRange[i].Priority < maxPriority)
			{
				inRange.RemoveAt(i);
				i--;
			}

		GenevauxNode selected = inRange[0];

		for (int i = 1; i < inRange.Count; i++)
		{
			if (inRange[i].Elevation < selected.Elevation)
				selected = inRange[i];
		}

		return selected;
	}

	void ExpandNode(GenevauxNode node)		// alpha in rule table
	{
		if (node.Priority == 1)
		{
			int num = (int)UnityEngine.Random.Range(1, 6);
			for (int i = 0; i < num; i++)
				InstantiateNode(node, 1);
		}
		else
		{
			float rand = UnityEngine.Random.Range(0.0f, 1.0f);
			int rule;
			if (rand < pCont)
				rule = 0;
			else if (rand < pCont + pSym)
				rule = 1;
			else
				rule = 2;

			switch (rule) {
				case 0:
					InstantiateNode(node, node.Priority);
					break;
				case 1:
					InstantiateNode(node, node.Priority-1);
					InstantiateNode(node, node.Priority-1);
					break;
				case 2:
					int m = (int)UnityEngine.Random.Range(1, node.Priority);
					InstantiateNode(node, node.Priority);
					InstantiateNode(node, m);
					break;
			}
		}

		candidateNodes.Remove(node);
	}

	void InstantiateNode(GenevauxNode parent, int priority)		// beta in rule table
	{
		bool added = false;

		Vec2 dir = parent.Parent != null ? parent.Pos - parent.Parent.Pos : mapSize / 2 - parent.Pos;
		float angle = MathOps.angle(new Vec2(0, -1), dir) + UnityEngine.Random.Range(-45.0f, 45.0f);	// Start checking towards center jiggled

		for (float y = -edgeLength; y < edgeLength && !added; y++)
		{
			float x = (float)Math.Sqrt(Math.Pow(edgeLength, 2) - Math.Pow(y, 2));
			Vec2[] positions = new Vec2[] { parent.Pos + MathOps.rotate(new Vec2(-x, y), angle), parent.Pos + MathOps.rotate(new Vec2(x, y), angle) };
			
			foreach (Vec2 pos in positions)
			{
				if (!contour.Contains(pos))
					continue;

				if (MathOps.distToPolygon(contour, pos) < minContourDist)
					continue;

				// If in contour and valid distance, check if edges are far enough apart
				bool edgeDist = true;
				Vec2 midpoint = (pos + parent.Pos) * 0.5f;

				for (int i = 0; i < riverNodes.Count && edgeDist; i++)	// Note: Figured a visited array search would take longer that calculating each edge twice
				{
					GenevauxNode start = riverNodes[i];
					if (!start.Equals(parent) && MathOps.distToLineSegment(parent.Pos, pos, start.Pos) <= minEdgeDist)
						edgeDist = false;
					else
					{
						foreach (GenevauxNode end in start.Children)
						{
							if (!end.Equals(parent) && MathOps.distToLineSegment(parent.Pos, pos, end.Pos) <= minEdgeDist)
								edgeDist = false;
							else if (MathOps.distToLineSegment(start.Pos, end.Pos, pos) <= minEdgeDist)
								edgeDist = false;
						}
					}

					
				}

				if (!edgeDist)
					continue;

				float slope = RiverSlope(pos);
				float elevation = parent.Elevation + slope * elevationScale;

				for (int i = 0; i < riverNodes.Count; i++)
				{
					if (Math.Abs(elevation - riverNodes[i].Elevation) >= maxSlope * Vec2.Distance(pos, riverNodes[i].Pos))
					{
						if (elevation > riverNodes[i].Elevation)
							elevation = riverNodes[i].Elevation + maxSlope * Vec2.Distance(pos, riverNodes[i].Pos);
						else
							elevation = riverNodes[i].Elevation - maxSlope * Vec2.Distance(pos, riverNodes[i].Pos);
					}
				}

				GenevauxNode newNode = new GenevauxNode(pos, parent, priority, elevation);

				riverNodes.Add(newNode);
				candidateNodes.Add(newNode);
				parent.AddChild(newNode);

				added = true;
				break;
			}
		}
	}

	/*
		Calculate crest points and classify rivers
		Rivers labeled 0 (A+), 1 (A), ... 8 (G)
	*/
	void ClassifyRivers()
	{
		float watershedArea(GenevauxNode node)
		{
			float area = node.Boundary.Area;
			foreach (GenevauxNode child in node.Children)
				area += watershedArea(child);
			return area;
		}

		for (int i = 0; i < riverNodes.Count; i++)
		{
			GenevauxNode node = riverNodes[i];

			node.Flow = 0.42f * (float)Math.Pow(watershedArea(riverNodes[i]), 0.69f);

			for (int v = 0; v < node.Boundary.VertexCount; v++)
			{
				Vec2 vert = node.Boundary[v];
				if (!contour.Contains(vert) || node.BoundaryHeights[v] != 0)
					continue;

				float dist = Vec2.Distance(node.Pos, vert);
				float maxHeight = node.Elevation;

				List<GenevauxNode> adjacent = new List<GenevauxNode>();

				for (int n = 0; n < riverNodes.Count; n++)
					if (Vec2.Distance(vert, riverNodes[n].Pos) == dist)
					{
						adjacent.Add(riverNodes[n]);
						maxHeight = Math.Max(maxHeight, riverNodes[n].Elevation);
					}
				
				float tSlope = TerrainSlope(vert);

				float height = maxHeight + tSlope * dist;

				foreach (GenevauxNode adj in adjacent)
					for (int b = 0; b < adj.Boundary.VertexCount; b++)
						if (adj.Boundary[b] == node.Boundary[v])
							adj.SetBoundaryHeight(b, height);
				node.SetBoundaryHeight(v, height);
			}

			float rSlope = RiverSlope(node.Pos);
			float distToShore = MathOps.distToPolygon(contour, node.Pos);

			if (rSlope < 0.04f && (distToShore < riverDistThreshold || node.Flow > riverFlowThreshold))
				node.RiverType = rSlope > 0.01f ? 4 : 5;
			else if (rSlope > 0.1f)
				node.RiverType = 0;
			else if (rSlope > 0.04f)
				node.RiverType = 1;
			else if (node.Flow > riverFlowThreshold && rSlope > 0.005f)
				node.RiverType = 4;
			else if (rSlope > 0.02f)
				node.RiverType = UnityEngine.Random.Range(0, 2) == 0 ? 2 : 8;
			else
				node.RiverType = new int[] {3, 6, 7}[UnityEngine.Random.Range(0, 3)];
		}
	}

	void GeneratePrimitives()
	{
		/*
			Generate River Primitives
			Note: Paper describes merging n entry points to a cell with (n-1) confluences and shows a cell with 3 entries as an example. 
			However, due to the network structure, I'm pretty sure each cell can have at most two entries. 
			I've implemented the algorithm to reflect this in order to avoid storing flows of non-junction river nodes
		*/
		for (int i = 0; i < riverNodes.Count; i++)
		{
			Graph<Vec3> riverStructure = new Graph<Vec3>();

			GenevauxNode node = riverNodes[i];
			int childCount = node.Children.Count;
			node.Skeleton = riverStructure;
			Polygon boundary = node.Boundary;

			List<Vec3> entryPoints = new List<Vec3>();
			Vec3 outletPoint = Vec3.zero;

			// For each edge, test if shared with child or parent
			foreach (GenevauxNode child in node.Children)
			{
				Polygon cBoundary = child.Boundary;
				for (int c = 0; c < cBoundary.VertexCount; c++) 
				{
					Vec2 c1 = cBoundary[c], c2 = cBoundary[(c+1)%cBoundary.VertexCount];
					for (int v = 0; v < boundary.VertexCount; v++)
					{
						Vec2 v1 = boundary[v], v2 = boundary[(v+1)%boundary.VertexCount];
						if (c1 == v1 && c2 == v2 || c1 == v2 && c2 == v1)
							entryPoints.Add(new Vec3(((v1+v2)/2).x, (node.BoundaryHeights[v]+node.BoundaryHeights[(v+1)%node.Boundary.VertexCount])/2, ((v1+v2)/2).y));
					}
				}
			}

			if (node.Parent != null) 
			{
				Polygon pBoundary = node.Parent.Boundary;
				for (int p = 0; p < pBoundary.VertexCount; p++)
				{
					Vec2 p1 = pBoundary[p], p2 = pBoundary[(p+1)%pBoundary.VertexCount];
						for (int v = 0; v < boundary.VertexCount; v++)
						{
							Vec2 v1 = boundary[v], v2 = boundary[(v+1)%boundary.VertexCount];
							if (p1 == v1 && p2 == v2 || p1 == v2 && p2 == v1)
								outletPoint = new Vec3(((v1+v2)/2).x, (node.BoundaryHeights[v]+node.BoundaryHeights[(v+1)%node.Boundary.VertexCount])/2, ((v1+v2)/2).y);
						}
				}
			}

			/* for (int v = 0; v < boundary.VertexCount; v++)
			{
				Vec2 l1 = boundary[v];
				Vec2 l2 = boundary[(v+1)%boundary.VertexCount];

				foreach (Node child in node.Children)
				{
					for (int c = 0; c < child.Boundary.VertexCount; c++)
					{
						Vec2 c1 = child.Boundary[c];
						Vec2 c2 = child.Boundary[MathOps.mod(c-1, child.Boundary.VertexCount)];
						if (c1 == l1 && c2 == l2 || c1 == l2 && c2 == l1)
							entryPoints.Add(new Vec3(((l1+l2)/2).x, (node.BoundaryHeights[v]+node.BoundaryHeights[(v+1)%node.Boundary.VertexCount])/2, ((l1+l2)/2).y));
					}
				}

				// Similarly, parent will share an edge eventually

				if (node.Parent != null)
					for (int p = 0; p < node.Parent.Boundary.VertexCount; p++)
					{
						Vec2 p1 = node.Parent.Boundary[p];
						Vec2 p2 = node.Parent.Boundary[MathOps.mod(p-1, node.Parent.Boundary.VertexCount)];
						if ((p1-l1).magnitude * (p1-l1).magnitude < 1E-03 && (p2-l2).magnitude * (p2-l2).magnitude < 1E-03)
							outletPoint = new Vec3(((l1+l2)/2).x, (node.BoundaryHeights[v]+node.BoundaryHeights[(v+1)%node.Boundary.VertexCount])/2, ((l1+l2)/2).y);
					}
			} */

			if (node.Parent == null)
				outletPoint = new Vec3(node.Pos.x, node.Elevation, node.Pos.y);


			if (childCount == 0)	// If no children, just calculate outlet point and return
			{
				Vec3 pos = new Vec3(node.Pos.x, node.Elevation, node.Pos.y);
				riverStructure.Add(pos);
				riverStructure.Add(outletPoint);
				riverStructure.Connect(pos, outletPoint);
			}
			else if (childCount == 1)
			{
				Vec3 pos = new Vec3(node.Pos.x, node.Elevation, node.Pos.y);
				riverStructure.Add(entryPoints[0]);
				riverStructure.Add(pos);
				riverStructure.Add(outletPoint);
				riverStructure.Connect(entryPoints[0], pos);
				riverStructure.Connect(pos, outletPoint);
			}
			else
			{
				//Debug.Log("River splits");
				//Debug.Log(entryPoints[0].x + ", " + entryPoints[0].z);
				//Debug.Log(entryPoints[1].x + ", " + entryPoints[1].z);
				//Debug.Log(childCount);
				GenevauxNode child1 = node.Children[0];
				GenevauxNode child2 = node.Children[1];

				/*if (entryPoints[0].x == 0 && entryPoints[0].z == 0)
					Debug.Log("entry1 0");
				if (entryPoints[1].x == 0 && entryPoints[1].z == 0)
					Debug.Log("engry2 0");*/

				float junctionAngle = node.Children[0].Priority == node.Children[1].Priority ? 60 + UnityEngine.Random.Range(-10.0f, 10.0f) : 90 + UnityEngine.Random.Range(-5.0f, 10f);

				Vec3 junction = new Vec3(node.Pos.x, node.Elevation, node.Pos.y);
				Vec2 bisector = new Vec2(((entryPoints[0]+entryPoints[1])/2 - junction).x, ((entryPoints[0]+entryPoints[1])/2 - junction).z).normalized;

				//Debug.Log(junction.x + ", " + junction.z);

				Vec2 conn1_2 = MathOps.rotate(bisector, -junctionAngle/2) * Vec2.Distance(new Vec2(junction.x, junction.z), new Vec2(entryPoints[0].x, entryPoints[0].z))/2;
				Vec3 conn1_3 = new Vec3(junction.x + conn1_2.x, (node.Elevation + entryPoints[0].y)/2, junction.z + conn1_2.y);

				Vec2 conn2_2 = MathOps.rotate(bisector, junctionAngle/2) * Vec2.Distance(new Vec2(junction.x, junction.z), new Vec2(entryPoints[1].x, entryPoints[1].z))/2;
				Vec3 conn2_3 = new Vec3(junction.x + conn2_2.x, (node.Elevation + entryPoints[1].y)/2, junction.z + conn2_2.y);

				//Debug.Log(conn1_3.x + ", " + conn1_3.z);
				//Debug.Log(conn2_3.x + ", " + conn2_3.z);

				/*if (conn1_3.x == 0 && conn1_3.z == 0)
					Debug.Log("conn1 0");
				if (conn2_3.x == 0 && conn2_3.z == 0)
					Debug.Log("conn2 0");*/

				riverStructure.Add(entryPoints[0]);
				riverStructure.Add(conn1_3);
				riverStructure.Connect(entryPoints[0], conn1_3);

				riverStructure.Add(entryPoints[1]);
				riverStructure.Add(conn2_3);
				riverStructure.Connect(entryPoints[1], conn2_3);

				riverStructure.Add(junction);
				//if (junction.x == 0 && junction.z == 0)
					//Debug.Log("Junction 0");
				riverStructure.Connect(conn1_3, junction);
				riverStructure.Connect(conn2_3, junction);

				riverStructure.Add(outletPoint);
				riverStructure.Connect(junction, outletPoint);
			}

			if (childCount == 2 && riverStructure.NodeCount != 6)
				continue;
		}
	}

	/* Helper Functions */

	// Basic idea: From any point in contour, ray will intersect contour odd number of times. Ray straight right from point (x, y) = {(x0, y) | x0 > x}

	float MinLineDist(Vec2 a1, Vec2 a2, Vec2 b1, Vec2 b2)
	{
		return Math.Min(
				Math.Min(MathOps.distToLineSegment(a1, a2, b1), MathOps.distToLineSegment(a1, a2, b2)),
				Math.Min(MathOps.distToLineSegment(b1, b2, a1), MathOps.distToLineSegment(b1, b2, a2))
			);
	}

	float RiverSlope(Vec2 pos)
	{
		Vec2 uv = pos / mapSize; 
		return riverSlopeMap[(int)(uv.y * riverSlopeMap.GetLength(0)), (int)(uv.x * riverSlopeMap.GetLength(1))];
	}

	float TerrainSlope(Vec2 pos)
	{
		Vec2 uv = pos / mapSize;
		return terrainSlopeMap[(int)(uv.y * riverSlopeMap.GetLength(0)), (int)(uv.x * riverSlopeMap.GetLength(1))];
	}
}