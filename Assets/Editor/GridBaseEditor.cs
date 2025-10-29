using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridBase))]
public class GridBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridBase gridBase = (GridBase)target;

        if (GUILayout.Button("Generate Nodes"))
        {
            gridBase.GenerateNode();
        }
    }
}
