using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExponentiallyDistributedNoise
{
    const float dec = 1.02f;
    static float[] m = new float[256];

    static Vector2[] gradientVectors = new Vector2[] {
        new Vector2(1, 1),
        new Vector2(1, -1),
        new Vector2(-1, 1),
        new Vector2(-1, -1)
    };

    static int[] p = new int[] {
            151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7,
            225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247,
            120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
            88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134,
            139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220,
            105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80,
            73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86,
            164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38,
            147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189,
            28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101,
            155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232,
            178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12,
            191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181,
            199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236,
            205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,   
            151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7,
            225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247,
            120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
            88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134,
            139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220,
            105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80,
            73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86,
            164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38,
            147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189,
            28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101,
            155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232,
            178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12,
            191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181,
            199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236,
            205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
        };

    static float fade(float n)
    { return 6 * Mathf.Pow(n, 5) - 15 * Mathf.Pow(n, 4) + 10 * Mathf.Pow(n, 3); }

    static float grad(int hash, float x, float y)
    {
        Vector2 grad = gradientVectors[hash & 3];
        return x * grad.x + y * grad.y;
    }

    static float noise(float x, float y)
    {
        int unitX = Mathf.FloorToInt(x) & 255;    //x and y as ints and in range [0, 255]
        int unitY = Mathf.FloorToInt(y) & 255;

        float xOffset = x - Mathf.Floor(x);    //Fractional components of x and y
        float yOffset = y - Mathf.Floor(y);

        int[] hashes = new int[] {          //Hash values to get pseudorandom gradients:
            p[p[unitX] + unitY],    //  By hashing unitX term, adding unitY term, and hashing again, gradients
            p[p[unitX+1] + unitY],  //  are replicable but seemingly random.
            p[p[unitX] + unitY+1],
            p[p[unitX+1] + unitY+1]
        };

        float[] dotProducts = new float[] {                             //Runs grad function on respective gradient vectors and four corners
            m[hashes[0]] * grad(hashes[0], xOffset, yOffset),       //of unit cube
            m[hashes[1]] * grad(hashes[1], xOffset-1, yOffset),
            m[hashes[2]] * grad(hashes[2], xOffset, yOffset-1),
            m[hashes[3]] * grad(hashes[3], xOffset-1, yOffset-1)
        };

        float fadedxOffset = fade(xOffset);   //Fades the offsets to get smoother transitions in following step
        float fadedyOffset = fade(yOffset);

        float top = Mathf.Lerp(dotProducts[0], dotProducts[1], fadedxOffset); //Bilinear interpolation with four dot products
        float bottom = Mathf.Lerp(dotProducts[2], dotProducts[3], fadedxOffset);;
        float value = Mathf.Lerp(top, bottom, fadedyOffset);

        value = (value + 1) / 2; //Bind to [0, 1] instead of [-1, 1]

        return value;
    }

    public static HeightMap Generate(int size, float scale, int octaves, float persistence, float lacunarity)
    {
        scale = size / scale;

        float mu = 1;

        for (int i = 0; i < 256; i++)
            m[i] = mu /= dec;

        Random.InitState((int)(Time.time * 1000));
        int loc = Random.Range(0, 10000);

        HeightMap noiseMap = new HeightMap(size);

        if (scale <= 0)
            scale = 0.0001f;

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseVal = 0;

                for (int o = 0; o < octaves; o++)
                {
                    float xCoord = x / scale * frequency;
                    float yCoord = y / scale * frequency;
                    float sample = noise(loc + xCoord, loc + yCoord);
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

