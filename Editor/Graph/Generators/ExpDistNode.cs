using XNode;

public class ExponentiallyDistributedNoiseNode : BaseNode { 
    public int size = 257;
    public float scale = 2;
    public int octaves = 8;
    public float persistence = 0.5f;
    public float lacunarity = 2;
    
    HeightMap map = null;

    public override HeightMap GetResult() {
        map = ExponentiallyDistributedNoise.Generate(
            GetInputValue<int>("size", size), 
            GetInputValue<float>("scale", scale), 
            GetInputValue<int>("octaves", octaves), 
            GetInputValue<float>("persistence", persistence), 
            GetInputValue<float>("lacunarity", lacunarity));
        
        return map;
    }
}