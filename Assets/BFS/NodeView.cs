using UnityEngine;

public class NodeView : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private NodeBase nodeData;

    public void Init(NodeBase node, Color color)
    {
        nodeData = node;
        nodeData.view = this;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
        transform.position = new Vector3(node.nodePosition.x, node.nodePosition.y, 0);
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }
}
