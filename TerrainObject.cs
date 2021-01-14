using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Terrain", menuName = "Terrain")]
[System.Serializable]
public class TerrainObject : ScriptableObject
{ public enum Generator { PerlinNoise, ExponentiallyDistributedNoise, Geneveaux }
    public enum Modifier { BeyerHydraulicErosion, WindErosion }
    public enum Container { Texture, Mesh, Terrain }

    public Generator generator = Generator.PerlinNoise;
    // public List<Modifier> modifiers;
    public Container container;

    // Parameters for Perlin Noise

    public int size = 257;

    public float scale = 2;
    public int octaves = 8;
    public float persistence = 0.5f;
    public float lacunarity = 2.0f;

    // Parameters for Geneveaux

    public int riverCount = 5;

    // Parameters for Beyer Erosion
    public int erosions = 1000;
   
    HeightMap map = null;
    
    // Parameters for Mesh generation

    public int tileSize = 64;
    public int meshSize = 64;
    public AnimationCurve heightCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float height=20;

    GameObject obj = null;

    public void Randomize() {
        Random.InitState((int)System.DateTime.Now.Ticks);
    }

    public void Generate() {

        if (generator == Generator.PerlinNoise)
            map = PerlinNoise.Generate(size, scale, octaves, persistence, lacunarity);
        else if (generator == Generator.ExponentiallyDistributedNoise)
            map = ExponentiallyDistributedNoise.Generate(size, scale, octaves, persistence, lacunarity);
        else if (generator == Generator.Geneveaux)
            map = GeneveauxTerrain.Generate(size, riverCount);  

        foreach(Modifier mod in modifiers) {
            if (mod == Modifier.BeyerHydraulicErosion)
                map = BeyerErosion.Erode(map, erosions);
        }
        
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

    public void UpdateMesh() {
        if (obj == null)
            return;

        int tileCount = obj.transform.childCount;
        int dimension = (int)Mathf.Sqrt(tileCount);


        int oldTileSize = (int)Mathf.Sqrt(obj.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh.vertexCount)-1;

        for (int i = 0; i < tileCount; i++) {
            int tx = (i % dimension) * oldTileSize;
            int ty = (i - (i % dimension)) / dimension * oldTileSize;

            List<Vector3> vertices = new List<Vector3>();

            for (int y = 0; y <= oldTileSize; y++)
                for (int x = 0; x <= oldTileSize; x++) {
                    float val = heightCurve.Evaluate(map[tx + x, ty + y]) * height;
                    vertices.Add(new Vector3(x, val, y));
                }

            obj.transform.GetChild(i).GetComponent<MeshFilter>().sharedMesh.vertices = vertices.ToArray();
        }
    }

    Terrain HeightMap2Terrain(HeightMap map) {
        return new Terrain();
    }

    void OnValidate() {
        // Ensure size is power of 2 + 1
        if (Mathf.Log(size-1, 2) % 1.0 != 0) {
            float pow = Mathf.Log(size-1, 2);
            int low = (int)Mathf.Pow(2, Mathf.FloorToInt(pow));
            int high = (int)Mathf.Pow(2, Mathf.CeilToInt(pow));
            if (size-low < high-size)   // Size closer to low -- choose low
                size = low+1;
            else
                size = high+1;
        }

        // Update Mesh, if present
        if (container == Container.Mesh && obj != null)
            UpdateMesh();
    }
}

