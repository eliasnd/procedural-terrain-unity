using System.Collections.Generic;
using UnityEngine;

public static class Utils {
    public static Texture2D HeightMap2Texture(HeightMap map) {
        return map.ToTexture2D(); 
    }

    public static GameObject HeightMap2Mesh(HeightMap map, int meshSize, float meshHeight, int tileSize, AnimationCurve heightCurve = null) {

        if (heightCurve == null)
            heightCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

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
                        float val = heightCurve.Evaluate(map[tx + x, ty + y]) * meshHeight;

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

    public static Terrain HeightMap2Terrain(HeightMap map) {
        return new Terrain();
    }
}