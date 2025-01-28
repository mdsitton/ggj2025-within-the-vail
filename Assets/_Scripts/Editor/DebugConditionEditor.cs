using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EventCondition_Debug))]
public class DebugConditionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EventCondition_Debug condition = (EventCondition_Debug)target;

        if (GUILayout.Button("Complete"))
            condition.DebugComplete();
        if (GUILayout.Button("Release"))
            condition.DebugRelease();
    }
}