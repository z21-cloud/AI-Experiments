using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StraightLine))]
public class StraightLineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        StraightLine straightLine = (StraightLine)target;

        if (GUILayout.Button("Создать путь"))
        {
            straightLine.Search();
        }
    }
}
