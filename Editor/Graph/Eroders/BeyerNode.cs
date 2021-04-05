using XNode;

public class BeyerErosionNode : BaseNode { 
    [Input] public BaseNode nodeIn;
    public int erosions = 1000;
    public float inertia = 0.3f;
    public float gravity = -9.81f;
    public float minSlope = 0.01f;
    public float capacity = 8;
    public float maxSteps = 500;
    public float evaporation = 0.02f;
    public float erosion = 0.7f;
    public float deposition = 0.2f;
    public int radius = 2;
    public float minSedimentCapacity = 0.01f;
    public float smoothFactor = 2;

    HeightMap map = null;

    public override HeightMap GetResult() {
        if (map == null)
            map = BeyerErosion.Erode(GetInputValue<BaseNode>("nodeIn").GetResult(), erosions, inertia, gravity, minSlope, 
                                        capacity, maxSteps, evaporation, erosion, deposition, 
                                        radius, minSedimentCapacity, smoothFactor);

        return map;
    }
}