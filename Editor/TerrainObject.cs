﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Terrain Object", menuName = "Terrain Object")]
[System.Serializable]
public class TerrainObject : ScriptableObject
{
    public enum Container { Texture, Mesh, Terrain }

    public Container container;
   
    
    public HeightMap map = null;
    // Parameters for Mesh generation

    public int tileSize = 64;
    public int meshSize = 64;
    public AnimationCurve heightCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float height=20;

    GameObject obj = null;

    public void Generate() {

        if (container == Container.Texture) {
            Texture2D tex = HeightMap2Texture(map);
            tex.Apply();
            tex.name = "Texture";
            AssetDatabase.AddObjectToAsset(tex, this);
            AssetDatabase.SetMainObject(this, AssetDatabase.GetAssetPath(this)); // This is disgusting and it is the only way I can get the asset to reload and show the texture
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this));

        } else if (container == Container.Mesh) {

            if (obj != null)
                DestroyImmediate(obj);
            /* GameObject obj = new GameObject();
            Mesh mesh = HeightMap2Mesh(map);

            MeshFilter filter = obj.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;

            MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = new Material(Shader.Find("Specular"));

            AssetDatabase.AddObjectToAsset(obj, this);
            AssetDatabase.AddObjectToAsset(mesh, this); */

            obj = HeightMap2Mesh(map);
            obj.name = "Terrain";

            // PrefabUtility.SaveAsPrefabAsset(obj, "Assets/terrain.prefab");
            // AssetDatabase.AddObjectToAsset(obj, this);

            // AssetDatabase.SetMainObject(obj, AssetDatabase.GetAssetPath(this));
        }
    }

    Texture2D HeightMap2Texture(HeightMap map) {
        return map.ToTexture2D(); 
    }

    GameObject HeightMap2Mesh(HeightMap map) {

        Debug.Assert((map.size - 1) % tileSize == 0);

        GameObject terrain = new GameObject();
        terrain.name = "Terrain Mesh";

        int dimension = map.size / tileSize;
    	float scale = (float)meshSize / map.size;

        for (int ty = 0; ty < map.size-1; ty += tileSize)
            for (int tx = 0; tx < map.size-1; tx += tileSize) {

                int r = ty / tileSize;
                int c = tx / tileSize;

                GameObject tile = new GameObject();
                tile.name = "Tile" + r + "" + c;
                tile.transform.parent = terrain.transform;
                tile.transform.position = new Vector3((c - dimension/2) * tileSize * scale, 0, (r - dimension/2) * tileSize * scale);
                tile.transform.localScale = new Vector3(scale, scale, scale);
                MeshFilter meshFilter = tile.AddComponent<MeshFilter>();
                MeshRenderer renderer = tile.AddComponent<MeshRenderer>();
                renderer.material = new Material(Shader.Find("Standard"));

                Mesh mesh = new Mesh();
                meshFilter.sharedMesh = mesh;

		        List<Vector3> vertices = new List<Vector3>();
		        List<Vector2> uvs = new List<Vector2>();

                for (int y = 0; y <= tileSize; y++)
                    for (int x = 0; x <= tileSize; x++) {
                        float val = heightCurve.Evaluate(map[tx + x, ty + y]) * height;

                        vertices.Add(new Vector3(x, val, y));
                        uvs.Add(new Vector2(x / (float)tileSize, y / (float)tileSize));
                    }

                mesh.vertices = vertices.ToArray();
                mesh.uv = uvs.ToArray();

                List<int> triangles = new List<int>();

                // Tiles actually have size tileSize+1 to cover gaps
                for (int y = 0; y < tileSize; y++)
                    for (int x = 0; x < tileSize; x++) {
                        triangles.Add(y * (tileSize+1) + x);
                        triangles.Add((y+1) * (tileSize+1) + x);
                        triangles.Add((y+1) * (tileSize+1) + (x+1));

                        triangles.Add(y * (tileSize+1) + x);
                        triangles.Add((y+1) * (tileSize+1) + (x+1));
                        triangles.Add(y * (tileSize+1) + (x+1));
                    }

                mesh.triangles = triangles.ToArray();
                mesh.RecalculateNormals();
            }
        
        return terrain;
    }

    Terrain HeightMap2Terrain(HeightMap map) {
        return new Terrain();
    }
}

