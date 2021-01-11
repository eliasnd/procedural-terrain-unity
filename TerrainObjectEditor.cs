using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainObject))]
public class TerrainObjectEditor : Editor
{
    public override void OnInspectorGUI() {
        TerrainObject script = (TerrainObject)target;

        script.generator = (TerrainObject.Generator)EditorGUILayout.EnumPopup("Generator", script.generator);
        
        EditorGUILayout.Space();

        if (script.generator == TerrainObject.Generator.PerlinNoise || script.generator == TerrainObject.Generator.ExponentiallyDistributedNoise) {
            script.size = EditorGUILayout.IntField("Size", script.size);
            script.scale = EditorGUILayout.FloatField("Scale", script.scale);
            script.octaves = EditorGUILayout.IntField("Octaves", script.octaves);
            script.persistence = EditorGUILayout.FloatField("Persistence", script.persistence);
            script.lacunarity = EditorGUILayout.FloatField("Lacunarity", script.lacunarity);
            
        } else if (script.generator == TerrainObject.Generator.Geneveaux) {
            script.riverCount = EditorGUILayout.IntField("River Count", script.riverCount);
        }
        
        EditorGUILayout.Space();

        script.container = (TerrainObject.Container)EditorGUILayout.EnumPopup("Container Type", script.container);
        
        EditorGUILayout.Space();

        if (GUILayout.Button("Generate")) {
            script.Generate();
        }
    }
}
