using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Space(20);

        if (GUILayout.Button("Open Editor"))
        {
            PlayerControllerEditorWindow.Open((PlayerController)target);
        }

        GUILayout.Space(20);

        base.OnInspectorGUI();
    }
}
