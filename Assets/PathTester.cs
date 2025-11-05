using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathTester : MonoBehaviour
{
    private NodeView startNodeView;
    private NodeView goalNodeView;
    private BFSPath bfsPath;

    private List<Node> currentPath = new List<Node>();

    private void Start()
    {
        bfsPath = GetComponent<BFSPath>();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                NodeView hitNode = hit.collider.GetComponent<NodeView>();
                if (hitNode == null) return;

                if(startNodeView == null)
                {
                    startNodeView = hitNode;
                    startNodeView.Highlight(Color.white);
                    return;
                }

                if (goalNodeView == null)
                {
                    goalNodeView = hitNode;
                    goalNodeView.Highlight(Color.red);

                    Node start = startNodeView.GetNode();
                    Node goal = goalNodeView.GetNode();

                    List<Node> path = bfsPath.SearchPath(start, goal);
                    if(path != null)
                    {
                        currentPath = path;
                        foreach (Node node in currentPath)
                        {
                            node.worldPosition.y += .1f;
                            node.view.Highlight(Color.blue);
                        }
                    }
                    return;
                }

                ClearPath();
            }
    }
}

    private void ClearPath()
    {
        Debug.Log("Путь очищен");
        if (startNodeView != null) startNodeView.Unhighlight();
        if (goalNodeView != null) goalNodeView.Unhighlight();

        foreach (Node node in currentPath)
        {
            node.view.Unhighlight();
        }

        startNodeView = null;
        goalNodeView = null;
        currentPath.Clear();
    }
}
