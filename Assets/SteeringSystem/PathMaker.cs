using UnityEngine;
using System.Collections.Generic;

public class PathMaker : MonoBehaviour
{
    public List<Transform> Waypoints;

    public void CreatePath()
    {
        Waypoints = new List<Transform>();
        Waypoints.AddRange(GetComponentsInChildren<Transform>());
        Waypoints.Remove(this.transform);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if(Waypoints != null && Waypoints.Count > 0)
        {
            for(int i = 0; i < Waypoints.Count - 1; i++)
            {
                Gizmos.DrawLine(Waypoints[i].position, Waypoints[i + 1].position);
            }
            Gizmos.DrawLine(Waypoints[0].position, Waypoints[Waypoints.Count - 1].position);
        }
    }
}
