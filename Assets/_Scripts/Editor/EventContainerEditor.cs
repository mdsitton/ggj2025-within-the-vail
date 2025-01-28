using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EventContainer))]
public class EventContainerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EventContainer container = (EventContainer)target;

        if (GUILayout.Button("Complete"))
            container.ExecuteActions();
        if (GUILayout.Button("Release"))
            container.ReleaseActions();
    }
}