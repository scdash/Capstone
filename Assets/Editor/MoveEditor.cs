#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class ActionEditor : EditorWindow {
    public GameObject targetM;
    public AnimationClip idleAnim;
    public AnimationClip walkFore;
    public AnimationClip runFore;
    public AnimationClip walkBack;
    public AnimationClip runBack;
    public AnimationClip jumpAnim;
    public AnimationClip moveAnim;
    public string moveName;

    [MenuItem("Window/Action Editor")]
    static void OpenWindow() {
        Debug.Log("ActionEditor is up and running");
        GetWindow<ActionEditor> ();
    }

    void OnGUI() {
        GUILayout.Label("Animation Controller Builder", EditorStyles.boldLabel);
        targetM = EditorGUILayout.ObjectField("Character Base", targetM, typeof(GameObject), true) as GameObject;
        idleAnim = EditorGUILayout.ObjectField("Idle Pose", idleAnim, typeof(AnimationClip), false) as AnimationClip;
        walkFore = EditorGUILayout.ObjectField("Forward Walk", walkFore, typeof(AnimationClip), false) as AnimationClip;
        runFore = EditorGUILayout.ObjectField("Forward Run", runFore, typeof(AnimationClip), false) as AnimationClip;
        walkBack = EditorGUILayout.ObjectField("Backward Walk", walkBack, typeof(AnimationClip), false) as AnimationClip;
        runBack = EditorGUILayout.ObjectField("Backward Run", runBack, typeof(AnimationClip), false) as AnimationClip;
        jumpAnim = EditorGUILayout.ObjectField("Vertical Jump", jumpAnim, typeof(AnimationClip), false) as AnimationClip;
        
        if (GUILayout.Button("Build")) {
            if (targetM == null){
                Debug.LogError("Needs a base to animate");
                return;
            }
            Debug.Log("Target found");
            Build();
        }

        GUILayout.Label("Add Move", EditorStyles.boldLabel);
        moveName = EditorGUILayout.TextField("Animator ID", "Move Name");
        moveAnim = EditorGUILayout.ObjectField("Move Animation", moveAnim, typeof(AnimationClip), true) as AnimationClip;

        GUILayout.Label("Delete Move", EditorStyles.boldLabel);
    }

    void Build()
    {
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Scripts/" + targetM.name + ".controller");
        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        AnimatorState idleState = controller.layers[0].stateMachine.AddState("Idle");
        idleState.motion = idleAnim;
    }
}
#endif