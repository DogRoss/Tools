using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerControllerEditorWindow : ExtendedEditorWindow
{
    int selectedToolbarIndex = 0;
    string[] toolbarOptions = new string[] { "Player Controller", "Modules" };

    //PlayerController
    bool generalValues;
    bool groundValues;
    bool airValues;
    bool camValues;

    public static void Open(PlayerController playerController)
    {
        PlayerControllerEditorWindow window = GetWindow<PlayerControllerEditorWindow>("Player Editor");
        window.serializedObject = new SerializedObject(playerController);
    }

    private void OnGUI()
    {
        DrawToolBar();
        DrawLeftBar();
        DrawRightBar();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawToolBar()
    {
        EditorGUILayout.BeginHorizontal(); // each begin begins a new line
        selectedToolbarIndex = GUILayout.Toolbar(selectedToolbarIndex, toolbarOptions);
        EditorGUILayout.EndHorizontal();
    }
    private void DrawLeftBar()
    {
        if(selectedToolbarIndex == 0)
        {
            //Player Controller tab selected
            EditorGUILayout.BeginVertical();

            generalValues = EditorGUILayout.Foldout(generalValues, "Main Values");
            if (generalValues)
            {
                EditorGUI.indentLevel++;

                currentProperty = serializedObject.FindProperty("input");
                EditorGUILayout.PropertyField(currentProperty);

                currentProperty = serializedObject.FindProperty("health");
                EditorGUILayout.PropertyField(currentProperty);

                currentProperty = serializedObject.FindProperty("playerMass");
                EditorGUILayout.PropertyField(currentProperty);

                currentProperty = serializedObject.FindProperty("acceleration");
                EditorGUILayout.PropertyField(currentProperty);

                currentProperty = serializedObject.FindProperty("gravity");
                EditorGUILayout.PropertyField(currentProperty);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            groundValues = EditorGUILayout.Foldout(groundValues, "Ground Values");
            if (groundValues)
            {
                EditorGUI.indentLevel++;

                currentProperty = serializedObject.FindProperty("groundAccelerationCoefficient");
                EditorGUILayout.PropertyField(currentProperty);

                currentProperty = serializedObject.FindProperty("groundFrictionCoefficient");
                EditorGUILayout.PropertyField(currentProperty);

                currentProperty = serializedObject.FindProperty("groundMask");
                EditorGUILayout.PropertyField(currentProperty);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            airValues = EditorGUILayout.Foldout(airValues, "Air Values");
            if (airValues)
            {
                EditorGUI.indentLevel++;

                currentProperty = serializedObject.FindProperty("airAccelerationCoefficient");
                EditorGUILayout.PropertyField(currentProperty);

                currentProperty = serializedObject.FindProperty("verticalDragCoefficient");
                EditorGUILayout.PropertyField(currentProperty);

                currentProperty = serializedObject.FindProperty("horizontalDragCoefficient");
                EditorGUILayout.PropertyField(currentProperty);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            camValues = EditorGUILayout.Foldout(camValues, "Camera Values");
            if (camValues)
            {
                EditorGUI.indentLevel++;

                currentProperty = serializedObject.FindProperty("sensitivity");
                EditorGUILayout.PropertyField(currentProperty);

                currentProperty = serializedObject.FindProperty("minLookAngle");
                EditorGUILayout.PropertyField(currentProperty);

                currentProperty = serializedObject.FindProperty("maxLookAngle");
                EditorGUILayout.PropertyField(currentProperty);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }
        else if(selectedToolbarIndex == 1)
        {
            //Modules tab selected

        }   
    }

    private void DrawRightBar()
    {

    }

}
