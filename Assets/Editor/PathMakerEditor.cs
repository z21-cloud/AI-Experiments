using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathMaker))]
public class PathMakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PathMaker pathMaker = (PathMaker)target;

        if(GUILayout.Button("Создать путь"))
        {
            pathMaker.CreatePath();
        }
    }
}
