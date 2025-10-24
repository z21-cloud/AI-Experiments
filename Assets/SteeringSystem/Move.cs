using UnityEngine;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;

public class Move : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float avoidSpeed = 200f;
    [SerializeField] private float distanceThreshold = .1f;
    public PathMaker PathMaker;
    private List<Transform> path { get { return PathMaker.Waypoints; } }
    private Rigidbody rb;
    private Sensor sensor;
    private int currentWaypoint = 0;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sensor = GetComponent<Sensor>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float avoid = sensor.Check();
        if (avoid == 0)
        {
            StandardSteer();
        }
        else
        {
            AvoidSteer(avoid);
        }
        MoveAgent();
        CheckWaypoint();
    }

    private void MoveAgent()
    { 
        rb.MovePosition(rb.position + (transform.forward * speed * Time.fixedDeltaTime));
    }

    private void StandardSteer()
    {
        Vector3 targetDirection = path[currentWaypoint].position - rb.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, Time.fixedDeltaTime, 0);
        transform.rotation = Quaternion.LookRotation(newDirection);
    } 

    private void AvoidSteer(float avoid)
    {
        transform.RotateAround(transform.position, transform.up, avoidSpeed * Time.fixedDeltaTime * avoid);
    }

    private void CheckWaypoint()
    {
        if(Vector3.Distance(rb.position, path[currentWaypoint].position) < distanceThreshold)
        {
            currentWaypoint++;
            if (currentWaypoint == path.Count)
                currentWaypoint = 0;
        }
    }
}
