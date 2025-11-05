using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeView : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Node node;
    private Color originalColor;
    private Color currentColor;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Init(Node node) => this.node = node;

    public void SetColor(Color color)
    {
        originalColor = color;
        currentColor = color;

        meshRenderer.material.color = color;
    }

    public void Highlight(Color highlightColor)
    {
        currentColor = highlightColor;
        meshRenderer.material.color = highlightColor;
    }

    public void Unhighlight()
    {
        currentColor = originalColor;
        meshRenderer.material.color = originalColor;
    }

    public Node GetNode() => node;
}
