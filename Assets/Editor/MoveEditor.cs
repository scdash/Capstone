using UnityEditor;
using UnityEngine;

//[InitializeOnLoad]
public class ActionEditor : EditorWindow {
    string string1 = "Insert Here";
    bool groupEnabled;
    bool move = true;
    float weight = 4.2f;

    static ActionEditor() {
        Debug.Log("ActionEditor is up and running");
        ShowWindow();
    }

    [MenuItem("Window/Action Editor")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(ActionEditor));
    }

    void OnGUI() {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        string1 = EditorGUILayout.TextField("Rule", string1);
        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
            move = EditorGUILayout.Toggle("Movement", move);
            weight = EditorGUILayout.Slider("Action Weight", weight, 0, 10);
        EditorGUILayout.EndToggleGroup();
        //add = GUILayout.Button("Add Action");
    }
}
