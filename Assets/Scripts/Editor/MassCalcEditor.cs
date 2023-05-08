using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MassCalculator))]
public class MassCalcEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MassCalculator calc = (MassCalculator)target;
        if (GUILayout.Button("Calculate Mass"))
        {
            calc.CalculateMass();
        }
    }
}
