using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Terrain", menuName = "Terrain")]
public class TerrainObject : ScriptableObject
{
    public enum Generator { PerlinNoise, ExponentiallyDistributedNoise, Geneveaux }
    public enum Modifier { HydraulicErosion, WindErosion }
    public enum Container { Texture, Mesh, Terrain }

    public Generator generator = Generator.Geneveaux;
    public List<Modifier> modifiers;
    public Container container;

    // Parameters for Perlin Noise

    public int size = 256;

    public float scale = 2;
    public int octaves = 8;
    public float persistence = 0.5f;
    public float lacunarity = 2.0f;

    // Parameters for Geneveaux

    public int riverCount = 5;
   

    bool generated = false;
    HeightMap map = null;

    public void Generate() {
        if (generator == Generator.PerlinNoise)
            map = PerlinNoise.Generate(size, scale, octaves, persistence, lacunarity);
        else if (generator == Generator.ExponentiallyDistributedNoise)
            map = ExponentiallyDistributedNoise.Generate(size, scale, octaves, persistence, lacunarity);
        else if (generator == Generator.Geneveaux)
            map = GeneveauxTerrain.Generate(size, riverCount);  
    }
}

