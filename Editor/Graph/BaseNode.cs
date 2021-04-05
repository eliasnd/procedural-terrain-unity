using XNode;

public abstract class BaseNode : Node { 
    [Output] public BaseNode result;
    public override object GetValue(NodePort port) {
        return this;
    }

    public abstract HeightMap GetResult();
}