using UnityEngine;
using UnityEngine.Rendering;

public class Sensor : MonoBehaviour
{
    [SerializeField] private float sensorLength = 2.5f;
    [SerializeField] private float frontSensorPoint = 1f;
    [SerializeField] private float rightSensorPoint = 1f;
    [SerializeField] private float frontSensorAngle = 40f;

    public float Check()
    {
        RaycastHit hit;
        float avoidDirection = 0;

        Vector3 frontPosition = transform.position + (transform.forward * frontSensorPoint);
        if(DrawSensors(frontPosition, transform.forward, sensorLength * 2, out hit))
        {
            if(hit.normal.x < 0)
            {
                avoidDirection = -0.25f;
            }
            else
            {
                avoidDirection = 0.25f;
            }
        }
        avoidDirection -= FrontSideSensors(frontPosition, out hit, 1);
        avoidDirection += FrontSideSensors(frontPosition, out hit, -1);
        return avoidDirection;
    }

    private bool DrawSensors(Vector3 sensorPosition, Vector3 direction, float lenght, out RaycastHit hit)
    {
        if (Physics.Raycast(sensorPosition, direction, out hit, lenght))
        {
            Debug.DrawLine(sensorPosition, hit.point, Color.black);
            return true;
        }
        return false;
    }

    private float FrontSideSensors(Vector3 frontPosition, out RaycastHit hit, float sensorDirection)
    {
        float avoidDirection = 0;

        Vector3 sensorPosition = frontPosition + (transform.right * rightSensorPoint * sensorDirection);
        Vector3 sensorAngle = Quaternion.AngleAxis(frontSensorAngle * sensorDirection, transform.up) * transform.forward;
        if (Physics.Raycast(sensorPosition, transform.forward, out hit, sensorLength))
        {
            avoidDirection += 1;
            Debug.DrawLine(sensorPosition, hit.point, Color.black);
        }

        if (Physics.Raycast(sensorPosition, sensorAngle, out hit, sensorLength))
        {
            avoidDirection += 0.5f;
            Debug.DrawLine(sensorPosition, hit.point, Color.black);
        }
        return avoidDirection;
    }
}
