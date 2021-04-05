using UnityEngine;
using UnityEditor;
using XNodeEditor;

[CustomEditor(typeof(TerrainGraph))]
public class TerrainGraphEditor : Editor
{
    public override void OnInspectorGUI() {

        TerrainGraph script = (TerrainGraph)target;

        serializedObject.Update();

        SerializedProperty map = serializedObject.FindProperty("heightmap");
        SerializedProperty generated = serializedObject.FindProperty("generated");

        if (GUILayout.Button("Edit graph", GUILayout.Height(25))) {
            XNodeEditor.NodeEditorWindow.Open(serializedObject.targetObject as XNode.NodeGraph);
        }


        EditorGUILayout.Space();

        if (!generated.boolValue) {
            if (GUILayout.Button("Generate", GUILayout.Height(25))) {
                ((TerrainGraph)target).Generate();
            }
        } else {
            if (script.heightmap == null)
            {
                int i = 0;
            }
            EditorGUILayout.ObjectField("HeightMap", script.texture, typeof(Texture2D), false);
            if (GUILayout.Button("Re-generate", GUILayout.Height(25))) {
                ((TerrainGraph)target).Generate();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
