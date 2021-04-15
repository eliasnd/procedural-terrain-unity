using System.Collections;
using System.Collections.Generic;

public class Graph<T>
{
	List<T> nodes;
	List<List<int>> adjacencies;

	public int NodeCount { get { return nodes.Count; } }

	public Graph()
	{
		nodes = new List<T>();
		adjacencies = new List<List<int>>();
	}

	public bool Add(T node)
	{
		if (nodes.Contains(node))
			return false;
			
		nodes.Add(node);
		adjacencies.Add(new List<int>());

		return true;
	}

	public T Get(int node)
	{
		return nodes[node];
	}

	public bool Remove(T node)
	{
		int i = IndexOf(node);
		
		if (i == -1)
			return false;

		RemoveAt(i);

		return true;
	}

	public bool RemoveAt(int node)
	{
		if (node < 0 || node > nodes.Count)
			return false;

		nodes.RemoveAt(node);

		adjacencies.RemoveAt(node);
		foreach (List<int> adjacency in adjacencies)
			if (adjacency.Contains(node))
				adjacency.Remove(node);

		return true;
	}

	public bool Connect(T node1, T node2)
	{
		int i1 = IndexOf(node1);
		int i2 = IndexOf(node2);

		if (i1 == -1 || i2 == -1)
			return false;

		return Connect(i1, i2);
	}

	public bool Connect(int node1, int node2)
	{
		if (node1 < 0 || node1 > nodes.Count || node2 < 0 || node2 > nodes.Count)
			return false;

		if (adjacencies[node1].Contains(node2))
			return false;

		adjacencies[node1].Add(node2);
		adjacencies[node2].Add(node1);

		return true;
	}

	public bool AreConnected(T node1, T node2)
	{
		int i1 = IndexOf(node1);
		int i2 = IndexOf(node2);

		if (i1 == -1 || i2 == -1)
			return false;
		return adjacencies[i1].Contains(i2);
	}

	public bool AreConnected(int node1, int node2)
	{
		if (node1 < 0 || node1 > nodes.Count || node2 < 0 || node2 > nodes.Count)
			return false;

		return adjacencies[node1].Contains(node2);
	}

	public List<T> GetAdjacencies(int node)
	{
		List<T> result = new List<T>();
		foreach (int nodeIndex in adjacencies[node])
			result.Add(nodes[nodeIndex]);
		return result;
	}

	public int IndexOf(T node)
	{
		for (int i = 0; i < nodes.Count; i++)
			if (nodes[i].Equals(node))
				return i;
		return -1;
	}
}