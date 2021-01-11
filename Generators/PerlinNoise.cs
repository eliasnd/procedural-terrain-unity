using UnityEngine;

public class PerlinNoise
{
    public static HeightMap Generate(int size, float scale, int octaves, float persistence, float lacunarity) {
        scale = size / scale;

        int loc = Random.Range(0, 10000);

        HeightMap noiseMap = new HeightMap(size);

        if (scale <= 0)
            scale = 0.0001f;

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++) {
                float amplitude = 1;
                float frequency = 1;
                float noiseVal = 0;

                for (int o = 0; o < octaves; o++) {
                    float xCoord = x / scale * frequency;
                    float yCoord = y / scale * frequency;
                    float sample = Mathf.PerlinNoise(loc + xCoord, loc + yCoord);
                    noiseVal += sample * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                if (noiseVal > maxNoiseHeight)
                    maxNoiseHeight = noiseVal;
                else if (noiseVal < minNoiseHeight)
                    minNoiseHeight = noiseVal;

                noiseMap[x, y] = noiseVal;
            }

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++) 
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);

        return noiseMap;
    }
    
}
