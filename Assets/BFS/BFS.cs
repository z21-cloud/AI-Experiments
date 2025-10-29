using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.VFX;

public class BFS : MonoBehaviour
{
    public GridBase gridBase;
    public float delay = 1f;

    public void BFSAlgStart()
    {
        StartCoroutine(BFSAlg());
    }

    public IEnumerator BFSAlg()
    {
        List<NodeBase> currentWave = new List<NodeBase>();
        List<NodeBase> nextWave = new List<NodeBase>();
        HashSet<NodeBase> visited = new HashSet<NodeBase>();

        NodeBase startNode = gridBase.startNode;
        NodeBase goalNode = gridBase.goalNode;

    currentWave.Add(startNode);
        visited.Add(startNode);

        bool found = false;

        while (currentWave.Count > 0 && !found)
        {
            foreach (NodeBase node in currentWave)
            {
                node.view.SetColor(Color.yellow);

                foreach (var neighbor in node.neighbors)
                {
                    if (neighbor == null || neighbor.isObstacle) continue;
                    if (visited.Contains(neighbor)) continue;

                    neighbor.parent = node;
                    visited.Add(neighbor);
                    nextWave.Add(neighbor);

                    neighbor.view.SetColor(Color.cyan);

                    if (neighbor == goalNode)
                    {
                        found = true;
                        break;
                    }
                }

                if (found) break;
            }

            yield return new WaitForSeconds(delay);
            currentWave = new List<NodeBase>(nextWave);
            nextWave.Clear();
        }

        if(found)
        {
            NodeBase current = goalNode;
            while(current != null)
            {
                current.view.SetColor(Color.green);
                current = current.parent;
            }
        }
    }
}
