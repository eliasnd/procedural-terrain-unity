using XNode;
using UnityEngine;

public class UnaryOpNode : BaseNode {
    [Input] public BaseNode nodeIn;
    public float num = 0;

    public enum OPERATION {ADD, SUBTRACT, MULTIPLY, DIVIDE, POWER, NEGATE}

    public OPERATION op = OPERATION.ADD;

    HeightMap map = null;

    public override HeightMap GetResult() {

        map = GetInputValue<BaseNode>("nodeIn").GetResult();
        if (op == OPERATION.NEGATE) {
            return (map * -1) + 1;
        } else {        
            if (op == OPERATION.ADD) {
                return map + num;
            } else if (op == OPERATION.SUBTRACT) {
                return map - num;
            } else if (op == OPERATION.MULTIPLY) {
                return map * num;
            } else if (op == OPERATION.DIVIDE) {
                return map / num;
            } else {
                HeightMap result = new HeightMap(map.size);

                for (int y = 0; y < map.size; y++)
                    for (int x = 0; x < map.size; x++)
                        result[x, y] = Mathf.Pow(map[x, y], num);

                return result;
            }
        }
    }
}