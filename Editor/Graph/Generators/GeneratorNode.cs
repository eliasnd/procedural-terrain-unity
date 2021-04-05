using XNode;

public abstract class GeneratorNode : Node { 
    [Output] public GeneratorNode result;

    public override object GetValue(NodePort port) {
        return this;
    }

    public abstract HeightMap GetResult();
}