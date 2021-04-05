using System;
using UnityEditor;
using UnityEngine;
using XNode;

[Serializable, CreateAssetMenu(fileName = "New Terrain Settings", menuName = "Terrain Settings")]
public class TerrainGraph : NodeGraph {
    public HeightMap heightmap;
    public Texture2D texture;
    [SerializeField] bool generated = false;

    public void Generate() {
        texture = new Texture2D(128, 128);
        
        foreach (Node n in nodes) {
            if (n is OutputNode) {
                heightmap = ((OutputNode)n).GetResult();
                generated = true;
                return;
            }
        }

        throw new Exception("Graph must contain an OutputNode");
    }
}
