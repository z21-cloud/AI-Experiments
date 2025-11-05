using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    private NodeView previousNodeView;
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            NodeView hitNode = hit.collider.GetComponent<NodeView>();
            
            if (hitNode != null && hitNode != previousNodeView)
            {
                if(previousNodeView != null) 
                    previousNodeView.Unhighlight();

                hitNode.Highlight(Color.yellow);
                previousNodeView = hitNode;
            }
        }
        else
        {
            if (previousNodeView != null)
            {
                previousNodeView.Unhighlight();
                previousNodeView = null;
            }
        }
    }
}
