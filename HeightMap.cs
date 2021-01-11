using System;

public class HeightMap 
{
    float[] map;
    public int size { get; private set; }

    public HeightMap(int size) {
        map = new float[size * size];
    }
    
    public float this[int x, int y] {
        get => map[y * size + x];
        set => map[y * size + x] = value;
    }

    public float Max() {
        float max = map[0];

        for (int i = 1; i < size * size; i++)
            if (map[i] > max)
                max = map[i];

        return max;
    }

    public float Min() {
        float min = map[0];

        for (int i = 1; i < size * size; i++)
            if (map[i] < min)
                min = map[i];

        return min;
    }

    public HeightMap Clone() {
        HeightMap clone = new HeightMap(size);

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                clone[x, y] = this[x, y];

        return clone;
    }

    public static HeightMap operator +(HeightMap a, HeightMap b) {
        if (a.size != b.size)
            throw new InvalidOperationException("Heightmap sizes must match");

        HeightMap result = new HeightMap(a.size);

        for (int y = 0; y < a.size; y++)
            for (int x = 0; x < a.size; x++)
                result[x, y] = a[x, y] + b[x, y];

        return result;
    }


    public static HeightMap operator -(HeightMap a, HeightMap b) {
        if (a.size != b.size)
            throw new InvalidOperationException("Heightmap sizes must match");

        HeightMap result = new HeightMap(a.size);

        for (int y = 0; y < a.size; y++)
            for (int x = 0; x < a.size; x++)
                result[x, y] = a[x, y] - b[x, y];

        return result;
    }

    public static HeightMap operator *(HeightMap h, float f) {
        HeightMap result = new HeightMap(h.size);

        for (int y = 0; y < h.size; y++)
            for (int x = 0; x < h.size; x++)
                result[x, y] = h[x, y] * f;

        return result;
    }


    public static HeightMap operator /(HeightMap h, float f) {
        HeightMap result = new HeightMap(h.size);

        for (int y = 0; y < h.size; y++)
            for (int x = 0; x < h.size; x++)
                result[x, y] = h[x, y] / f;

        return result;
    }
}
