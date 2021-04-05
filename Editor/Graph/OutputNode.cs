using XNode;

[CreateNodeMenu("Output")]
public class OutputNode : Node { 
    [Input] public BaseNode node;
    public override object GetValue(NodePort port) {
        return this;
    }

    public HeightMap GetResult() {
        return node.GetResult();
    }
}