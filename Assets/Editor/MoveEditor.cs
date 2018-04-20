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
    public int moveWeight = 5;

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
        
        if (GUILayout.Button("Build", GUILayout.MaxWidth(150f))) {
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
        moveWeight = EditorGUILayout.IntSlider("Action Weight", moveWeight, 0, 10);

        if (GUILayout.Button("Add", GUILayout.MaxWidth(150f))) {
            if (moveName.Equals("Move Name")) {
                Debug.LogError("Please add an appropriate name");
                return;
            }
            Debug.Log("Move Made");
        }

        GUILayout.Label("Delete Move", EditorStyles.boldLabel);
    }

    void Build()
    {
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Scripts/" + targetM.name + ".controller");

        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("IsJump", AnimatorControllerParameterType.Trigger);

        var rootMachine = controller.layers[0].stateMachine;
        BlendTree blendTree1;
        BlendTree blendTree2;

        AnimatorState idleState = rootMachine.AddState("Idle");
        AnimatorState jumpState = rootMachine.AddState("Jump");
        AnimatorState moveState1 = controller.CreateBlendTreeInController("Move Forward", out blendTree1);
        AnimatorState moveState2 = controller.CreateBlendTreeInController("Move Backward", out blendTree2);
        idleState.motion = idleAnim;
        jumpState.motion = jumpAnim;

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
        AnimatorStateTransition jumpUp = idleState.AddTransition(jumpState);
        AnimatorStateTransition jumpDown = jumpState.AddTransition(idleState, true);

        leaveIFore.AddCondition(AnimatorConditionMode.Greater, 0.01f, "Speed");
        leaveMFore.AddCondition(AnimatorConditionMode.Less, 0.01f, "Speed");
        leaveIBack.AddCondition(AnimatorConditionMode.Less, -0.01f, "Speed");
        leaveMBack.AddCondition(AnimatorConditionMode.Greater, -0.01f, "Speed");
        jumpUp.AddCondition(AnimatorConditionMode.If, 0, "IsJump");

        targetM.GetComponent<Animator>().runtimeAnimatorController = controller;
    }
    /**
    void add {
        foo;
    }
    **/
}
#endif