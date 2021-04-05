using System;
using UnityEditor;
using UnityEngine;
using XNode;

[Serializable, CreateAssetMenu(fileName = "New Terrain", menuName = "Terrain")]
public class TerrainGraph : NodeGraph {
    public HeightMap Output() {
        foreach (Node n in nodes) {
            if (n is OutputNode) {
                return ((OutputNode)n).GetResult();
            }
        }

        throw new Exception("Graph must contain an OutputNode");
    }
}
