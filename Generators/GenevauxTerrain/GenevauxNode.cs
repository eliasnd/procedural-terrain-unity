using System;
using System.Collections;
using System.Collections.Generic;

public class GenevauxNode
{
	// Position Data
	public Vec2 Pos {get;}
	public float Elevation {get; private set;}
	// public List<Vec3> Boundary {get; private set;}
	public Polygon Boundary {get; private set;}
	public List<float> BoundaryHeights {get; private set;}

	//River Data
	public int Priority {get; set;}
	public int RiverType {get; set;}
	public float Flow {get; set;}

	// Terrain Generation Data
	public Graph<Vec3> Skeleton {get; set;}
	// public List<Vector4> Primitives {get;}

	public GenevauxNode Parent {get;}
	public List<GenevauxNode> Children {get;}

	public GenevauxNode(Vec2 pos, GenevauxNode parent=null, int priority=0, float elevation=0)
	{
		Pos = pos;
		Elevation = elevation;
		Parent = parent;
		Priority = priority;
		Children = new List<GenevauxNode>();
	}

	public void SetElevation(float elevation)
	{
		Elevation = elevation;
	}

	public void AddChild(GenevauxNode node)
	{
		Children.Add(node);
	}

	public void SetBoundary(Polygon boundary)
	{
		List<float> heights = new List<float>();
		for (int i = 0; i < boundary.VertexCount; i++)
			heights.Add(0);
		
		Boundary = boundary;
		BoundaryHeights = heights;
	}

	public void SetBoundaryHeight(int index, float height)
	{
		BoundaryHeights[index] = height;
	}
}