#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.IO;

public class ActionEditor : EditorWindow {
    public GameObject objMar;
    public GameObject targetM;
    public AnimationClip idleAnim;
    public AnimationClip walkFore;
    public AnimationClip runFore;
    public AnimationClip walkBack;
    public AnimationClip runBack;
    public AnimationClip jumpAnim;
    public AnimationClip moveAnim;
    public AnimatorController controller;
    public string moveName = "Move Name";
    public string movePre = "Precondition";
    public bool preTrue = false;

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
        moveName = EditorGUILayout.TextField("Action ID", moveName);
        moveAnim = EditorGUILayout.ObjectField("Action Animation", moveAnim, typeof(AnimationClip), true) as AnimationClip;
        movePre = EditorGUILayout.TextField("Action Precondition", movePre);
        preTrue = EditorGUILayout.Toggle("Precondition State", preTrue);

        if (GUILayout.Button("Add", GUILayout.MaxWidth(150f))) {
            if (moveName.Equals("Move Name")) {
                Debug.LogError("Please add an appropriate name");
                return;
            } else if (movePre.Equals("Precondition")) {
                Debug.LogError("Please add an appropriate precondition");
                return;
            }
            Add();
            Debug.Log("Move Made");
        }
    }

    void Build()
    {
        controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Scripts/Controllers/" + targetM.name + ".controller");

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
    
    void Add() {
        var rootMachine = controller.layers[0].stateMachine;
        AnimatorState actionState = rootMachine.AddState(moveName);
        actionState.motion = moveAnim;
        controller.AddParameter(moveName + "Trigger", AnimatorControllerParameterType.Trigger);
        AnimatorStateTransition leaveId = rootMachine.defaultState.AddTransition(actionState);
        AnimatorStateTransition toId = actionState.AddTransition(rootMachine.defaultState, true);
        leaveId.AddCondition(AnimatorConditionMode.If, 0, moveName + "Trigger");

        string smallTrue;
        if (preTrue == true) {
            smallTrue = "true";
        } else {
            smallTrue = "false";
        }
        string cName = moveName.Replace(" ", "_");
        cName = cName.Replace("-", "_");
        string copyPath = "Assets/Scripts/Actions/" + cName + ".cs";
        Debug.Log("Creating Classfile: " + copyPath);
        if (File.Exists(copyPath) == false) {
            using (StreamWriter outfile =
                new StreamWriter(copyPath)) {
                outfile.WriteLine("using UnityEngine;");
                outfile.WriteLine("using System.Collections;");
                outfile.WriteLine("using System.Collections.Generic;");
                outfile.WriteLine("");
                outfile.WriteLine("public class " + cName + " : GoapAction {");
                outfile.WriteLine(" ");
                outfile.WriteLine(" private bool punched = false;");
                outfile.WriteLine(" private BruteController bTarget;");
                outfile.WriteLine(" private float startTime = 0;");
                outfile.WriteLine(" private float workDuration = 2;");
                outfile.WriteLine(" ");
                outfile.WriteLine(" public " + cName + "() {");
                outfile.WriteLine("     addPrecondition(\"" + movePre + "\", " + smallTrue + ");");
                outfile.WriteLine("     addEffect(\"punchMan\", true);");
                outfile.WriteLine(" }");
                outfile.WriteLine(" ");
                outfile.WriteLine(" public override void reset() {");
                outfile.WriteLine("     punched = false;");
                outfile.WriteLine("     bTarget = null;");
                outfile.WriteLine("     startTime = 0;");
                outfile.WriteLine(" }");
                outfile.WriteLine(" ");
                outfile.WriteLine(" public override bool isDone() {");
                outfile.WriteLine("     return punched;");
                outfile.WriteLine(" }");
                outfile.WriteLine(" ");
                outfile.WriteLine(" public override bool requiresInRange() {");
                outfile.WriteLine("     return true;");
                outfile.WriteLine(" }");
                outfile.WriteLine(" ");
                outfile.WriteLine(" public override bool checkProceduralPrecondition(GameObject agent) {");
                outfile.WriteLine("     BruteController[] bs = (BruteController[])UnityEngine.GameObject.FindObjectsOfType(typeof(BruteController));");
                outfile.WriteLine("     BruteController closest = null;");
                outfile.WriteLine("     float closestDist = 0;");
                outfile.WriteLine("     ");
                outfile.WriteLine("     foreach (BruteController b in bs) {");
                outfile.WriteLine("         if (closest == null) {");
                outfile.WriteLine("             closest = b;");
                outfile.WriteLine("             closestDist = (b.gameObject.transform.position - agent.transform.position).magnitude;");
                outfile.WriteLine("         } else {");
                outfile.WriteLine("             float dist = (b.gameObject.transform.position - agent.transform.position).magnitude;");
                outfile.WriteLine("             if (dist < closestDist) {");
                outfile.WriteLine("                 closest = b;");
                outfile.WriteLine("                 closestDist = dist;");
                outfile.WriteLine("             }");
                outfile.WriteLine("         }");
                outfile.WriteLine("     }");
                outfile.WriteLine("     if (closest == null) {");
                outfile.WriteLine("         return false;");
                outfile.WriteLine("     }");
                outfile.WriteLine("     ");
                outfile.WriteLine("     bTarget = closest;");
                outfile.WriteLine("     target = bTarget.gameObject;");
                outfile.WriteLine("     ");
                outfile.WriteLine("     return closest != null;");
                outfile.WriteLine("     }");
                outfile.WriteLine(" ");
                outfile.WriteLine(" public override bool perform(GameObject agent) {");
                outfile.WriteLine("     if (startTime == 0) {");
                outfile.WriteLine("         startTime = Time.time;");
                outfile.WriteLine("         GetComponent<Animator>().SetTrigger(\""+ moveName +"Trigger\");");
                outfile.WriteLine("     }");
                outfile.WriteLine("     ");
                outfile.WriteLine("     if (Time.time - startTime > workDuration) {");
                outfile.WriteLine("         punched = true;");
                outfile.WriteLine("     }");
                outfile.WriteLine("     ");
                outfile.WriteLine("     return true;");
                outfile.WriteLine(" }");
                outfile.WriteLine("}");
            }
        }
    }
    
}
#endif