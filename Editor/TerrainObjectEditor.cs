using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainObject))]
public class TerrainObjectEditor : Editor
{
    public override void OnInspectorGUI() {
        serializedObject.Update();

        // This sucks, there must be a better way, I know it
        SerializedProperty generator = serializedObject.FindProperty("generator");
        SerializedProperty container = serializedObject.FindProperty("container");
        SerializedProperty size = serializedObject.FindProperty("size");
        SerializedProperty scale = serializedObject.FindProperty("scale");
        SerializedProperty octaves = serializedObject.FindProperty("octaves");
        SerializedProperty persistence = serializedObject.FindProperty("persistence");
        SerializedProperty lacunarity = serializedObject.FindProperty("lacunarity");
        SerializedProperty riverCount = serializedObject.FindProperty("riverCount");
        SerializedProperty tileSize = serializedObject.FindProperty("tileSize");
        SerializedProperty meshSize = serializedObject.FindProperty("meshSize");
        SerializedProperty heightCurve = serializedObject.FindProperty("heightCurve");
        SerializedProperty height = serializedObject.FindProperty("height");
        SerializedProperty modifiers = serializedObject.FindProperty("modifiers");
        SerializedProperty erosions = serializedObject.FindProperty("erosions");

        if (GUILayout.Button("Randomize")) {
            ((TerrainObject)target).Randomize();
        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(generator, new GUIContent("Generator"));
        EditorGUILayout.Space();

        if (generator.enumValueIndex < 2) {     // Handle Perlin and Exponentialy Distributed Noise
            EditorGUILayout.PropertyField(size, new GUIContent("Size"));
            EditorGUILayout.PropertyField(scale, new GUIContent("Scale"));
            EditorGUILayout.PropertyField(octaves, new GUIContent("Octaves"));
            EditorGUILayout.PropertyField(persistence, new GUIContent("Persistence"));
            EditorGUILayout.PropertyField(lacunarity, new GUIContent("Lacunarity"));
        } else {    // Geneveaux Terrain
            EditorGUILayout.PropertyField(riverCount, new GUIContent("River Count"));
        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(modifiers, new GUIContent("Map Modifiers"));

        EditorGUILayout.Space();

        if (modifiers.arraySize > 0) {
            EditorGUILayout.LabelField("Modifier Options");

            EditorGUILayout.LabelField("Beyer Erosion Options");
            EditorGUILayout.PropertyField(erosions, new GUIContent("Erosion"));
        }

        

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(container, new GUIContent("Container"));

        if (container.enumValueIndex == 1)  { // Handle Mesh
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(tileSize, new GUIContent("Tile Size"));
            EditorGUILayout.PropertyField(meshSize, new GUIContent("Mesh Size"));
            EditorGUILayout.PropertyField(heightCurve, new GUIContent("Height Curve"));
            EditorGUILayout.PropertyField(height, new GUIContent("Height"));
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate")) {
            ((TerrainObject)target).Generate();
        }

        // Warning -- Old way Below -- bleh!
        /* 
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
        
        if (script.container == TerrainObject.Container.Mesh) {
            EditorGUILayout.Space();
            script.tileSize = EditorGUILayout.IntField("Tile Size", script.tileSize);
            script.meshSize = EditorGUILayout.IntField("Mesh Size", script.meshSize);
            script.heightCurve = EditorGUILayout.CurveField("Height Curve", script.heightCurve);
            script.height = EditorGUILayout.FloatField("Height Scale", script.height);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate")) {
            script.Generate();
        } */

        serializedObject.ApplyModifiedProperties();
    }
}
