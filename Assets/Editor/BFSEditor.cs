using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BFS))]
public class BFSEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BFS bfs = (BFS)target;

        if (GUILayout.Button("Start Search"))
        {
            bfs.BFSAlgStart();
        }
    }
}
