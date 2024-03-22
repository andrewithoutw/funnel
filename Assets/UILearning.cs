using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager.UI;


[CustomEditor(typeof(Example))]
public class CustomExample : Editor
{
    SerializedProperty boolProperty;
    SerializedProperty floatProperty;
    SerializedProperty intProperty;


    void OnEnable()
    {
        boolProperty = serializedObject.FindProperty("boolVariable");
        floatProperty = serializedObject.FindProperty("floatVariable");
        intProperty = serializedObject.FindProperty("intVariable");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();

        boolProperty.boolValue = EditorGUILayout.Toggle(boolProperty.displayName, boolProperty.boolValue);

        if (boolProperty.boolValue)
        {
            OnBoolPropertyTrue();
        }

        floatProperty.floatValue = EditorGUILayout.FloatField(floatProperty.displayName, floatProperty.floatValue);
        intProperty.intValue = EditorGUILayout.IntField(intProperty.displayName, intProperty.intValue);

        serializedObject.ApplyModifiedProperties();
    }


    void OnBoolPropertyTrue()
    {
        EditorGUILayout.LabelField("I AM BELOW THE BOOL PROPERTY NOW");
    }
}