using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

//TODO: explain this
public class ExtendedEditorWindow : EditorWindow
{
    protected SerializedObject serializedObject;
    protected SerializedObject selectedObject;
    protected SerializedProperty currentProperty;

    private string selectedPropertyPath;
    protected SerializedProperty selectedProperty;


    public void DrawSelectedObjectInfo(SerializedObject obj)
    {
        selectedObject = obj;
        selectedObject.Update();
        SerializedProperty prop = selectedObject.GetIterator();

        while (prop.NextVisible(true))
        {
            if (prop.isArray && prop.propertyType == SerializedPropertyType.Generic)
            {
                Debug.Log("[protected void DrawProperties(SerializedProperty prop, bool drawChildren)] \n" +
                          "1.1) array and property type are generic, foldout and draw properties (if expanded foldout).");

                EditorGUILayout.BeginHorizontal();
                prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, prop.displayName);
                EditorGUILayout.EndHorizontal();

                //TODO: explain this
                if (prop.isExpanded)
                {
                    Debug.Log("[protected void DrawProperties(SerializedProperty prop, bool drawChildren)] \n" +
                              "1.2) Expanded, draw property");
                    EditorGUI.indentLevel++;
                    DrawProperties(prop, true);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                Debug.Log("[protected void DrawProperties(SerializedProperty prop, bool drawChildren)] \n" +
                          "2.1) array and property type are generic, foldout and draw properties (if expanded foldout).");

                EditorGUILayout.PropertyField(prop, true);
            }
        }


        selectedObject.ApplyModifiedProperties();
    }

    //TODO: explain this
    protected void DrawProperties(SerializedProperty prop, bool drawChildren = true)
    {
        Debug.Log("[protected void DrawProperties(SerializedProperty prop, bool drawChildren)] \n" +
                          "0.1) Entering method.");

        Debug.Log(prop.propertyPath + " // prop path");

        string lastPropPath = string.Empty;

        foreach (SerializedProperty p in prop)
        {
            Debug.Log("Loop");
            //TODO: explain this
            if (Attribute.IsDefined(prop.serializedObject.targetObject.GetType().GetField(p.propertyPath), typeof(SerializeField)))
            {
                if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
                {
                    Debug.Log("[protected void DrawProperties(SerializedProperty prop, bool drawChildren)] \n" +
                              "1.1) array and property type are generic, foldout and draw properties (if expanded foldout).");

                    EditorGUILayout.BeginHorizontal();
                    p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                    EditorGUILayout.EndHorizontal();

                    //TODO: explain this
                    if (p.isExpanded)
                    {
                        Debug.Log("[protected void DrawProperties(SerializedProperty prop, bool drawChildren)] \n" +
                                  "1.2) Expanded, draw property");
                        EditorGUI.indentLevel++;
                        DrawProperties(p, drawChildren);
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    Debug.Log("[protected void DrawProperties(SerializedProperty prop, bool drawChildren)] \n" +
                              "2.1) array and property type are generic, foldout and draw properties (if expanded foldout).");

                    if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) { continue; }
                    lastPropPath = p.propertyPath;
                    EditorGUILayout.PropertyField(p, drawChildren);
                }
            }
        }
    }
    protected void DrawSidebar(SerializedProperty prop)
    {
        foreach (SerializedProperty p in prop)
        {
            if (GUILayout.Button(p.displayName))
                selectedPropertyPath = p.propertyPath;
        }

        if (!string.IsNullOrEmpty(selectedPropertyPath))
            selectedProperty = serializedObject.FindProperty(selectedPropertyPath);
    }

    protected void DrawField(string propName, bool relative = true)
    {
        SerializedProperty prop;
        if (relative)
            prop = currentProperty.FindPropertyRelative(propName);
        else
            prop = serializedObject.FindProperty(propName);

        if (relative && currentProperty != null)
            EditorGUILayout.PropertyField(prop, true);
        else if (serializedObject != null)
            EditorGUILayout.PropertyField(prop, true);
    }

    protected virtual void Apply()
    {
        serializedObject.ApplyModifiedProperties();
    }
}
