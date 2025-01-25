
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Gun))]
public class GunEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Gun gun = (Gun) target;

        

        if (GUILayout.Button("Reload"))
            gun.Reload();
        if (GUILayout.Button("Fire"))
            gun.Fire();
        if (GUILayout.Button("TriggerDown"))
            gun.TriggerDown();
        if (GUILayout.Button("TriggerUp"))
            gun.TriggerUp();

    }
}
