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
    public float moveWeight = 5.00f;

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
        moveName = EditorGUILayout.TextField("Action ID", "Move Name");
        moveAnim = EditorGUILayout.ObjectField("Action Animation", moveAnim, typeof(AnimationClip), true) as AnimationClip;
        moveWeight = EditorGUILayout.Slider("Action Weight", moveWeight, 0, 10);

        GUILayout.Label("Delete Move", EditorStyles.boldLabel);
    }

    void Build()
    {
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Scripts/" + targetM.name + ".controller");
        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);

        AnimatorState idleState = controller.layers[0].stateMachine.AddState("Idle");
        idleState.motion = idleAnim;
        //AnimatorState jumpState = controller.layers[1].stateMachine.AddState("Jump");
        //jumpState.motion = jumpAnim;

        BlendTree blendTree1;
        BlendTree blendTree2;
        AnimatorState moveState1 = controller.CreateBlendTreeInController("Move Forward", out blendTree1);
        AnimatorState moveState2 = controller.CreateBlendTreeInController("Move Backward", out blendTree2);

        blendTree1.blendType = BlendTreeType.Simple1D;
        blendTree2.blendType = BlendTreeType.Simple1D;
        blendTree1.blendParameter = "Speed";
        blendTree2.blendParameter = "Speed";
        blendTree1.AddChild(walkFore);
        blendTree1.AddChild(runFore);
        blendTree2.AddChild(walkBack);
        blendTree2.AddChild(runBack);

        AnimatorStateTransition leaveIFore = idleState.AddTransition(moveState1);
        AnimatorStateTransition leaveMFore = moveState1.AddTransition(idleState);
        AnimatorStateTransition leaveIBack = idleState.AddTransition(moveState2);
        AnimatorStateTransition leaveMBack = moveState2.AddTransition(idleState);
        //AnimatorStateTransition jumpUp = idleState.AddTransition(jumpState);
        //AnimatorStateTransition jumpDown = jumpState.AddTransition(idleState);

        leaveIFore.AddCondition(AnimatorConditionMode.Greater, 0.01f, "Speed");
        leaveMFore.AddCondition(AnimatorConditionMode.Less, 0.01f, "Speed");
        leaveIBack.AddCondition(AnimatorConditionMode.Less, -0.01f, "Speed");
        leaveMBack.AddCondition(AnimatorConditionMode.Greater, -0.01f, "Speed");

        targetM.GetComponent<Animator>().runtimeAnimatorController = controller;
    }
}
#endif