public class Perlin 
{
    public static HeightMap Generate(int size, float scale, int octaves, float persistence, float lacunarity) {
        return new HeightMap(size);
    }
    
    public static HeightMap GenerateExponential(int size, float scale, int octaves, float persistence, float lacunarity, float exponent) {
        return new HeightMap(size);
    }
}
