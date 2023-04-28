using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IK_Leg))]
public class IK_LegEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        IK_Leg legScript = (IK_Leg)target;
        if (GUILayout.Button("Calculate angles"))
        {
            legScript.CalculateAngles();
        }
    }
}
