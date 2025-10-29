using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Dijkstra))]
public class DiijkstraEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Dijkstra dijkstra = (Dijkstra)target;

        if (GUILayout.Button("Start Search"))
        {
            dijkstra.StarSearch();
        }
    }
}
