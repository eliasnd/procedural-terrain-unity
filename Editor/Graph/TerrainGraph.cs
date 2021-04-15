using System;
using UnityEditor;
using UnityEngine;
using XNode;

[Serializable, CreateAssetMenu(fileName = "New Terrain Generator", menuName = "Terrain Generator")]
public class TerrainGraph : NodeGraph {
    HeightMap heightmap;
    [SerializeField] bool generated = false;

    public void Generate() {

        foreach (Node n in nodes) {
            if (n is OutputNode) {
                heightmap = ((OutputNode)n).GetResult();
                TerrainObject obj = ScriptableObject.CreateInstance<TerrainObject>();
                obj.map = heightmap;
                AssetDatabase.CreateAsset(obj, "Assets/New Terrain.asset");
                AssetDatabase.SaveAssets();
                generated = true;
                return;
            }
        }

        throw new Exception("Graph must contain an OutputNode");
    }
}
