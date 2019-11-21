using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformUI : MonoBehaviour
{
    public Vector3[] globalWaypoints;
    private int waypointIndex;

    public bool movePassanger;
    public float speed;
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public float currentPercent;

    public GameObject trigger;

    private void Awake()
    {
        if (transform.Find("Body"))
        {
            GetComponent<BoxCollider2D>().size = transform.Find("Body").GetComponent<SpriteRenderer>().size * transform.Find("Body").transform.localScale;
            GetComponent<BoxCollider2D>().offset = transform.Find("Body").localPosition;
        }
        else
        {
            GetComponent<BoxCollider2D>().enabled = false;
        }

        for (int i = 0; i < globalWaypoints.Length; i++)
        {
            globalWaypoints[i] += transform.position;
        }
    }

    public void Init()
    {
        if (trigger)
        {
            trigger.SetActive(true);
        }
        
        currentPercent = 0;
        waypointIndex = 0;
        transform.position = globalWaypoints[waypointIndex];
    }
    
    public Vector3 CalculatePlatformMovement()
    {
        Vector3 vec = globalWaypoints[waypointIndex + 1] - globalWaypoints[waypointIndex];
        
        currentPercent += Time.deltaTime * speed / vec.magnitude;
        Vector3 newPos = Vector3.Lerp(globalWaypoints[waypointIndex], globalWaypoints[waypointIndex + 1], currentPercent / 1);
        
        if (currentPercent >= 1.0f)
        {
            currentPercent = 0;
            waypointIndex++;

            if (waypointIndex == globalWaypoints.Length - 1)
            {
                enabled = false;
            }

            return globalWaypoints[waypointIndex] - transform.position;
        }
        else
        {
            return newPos - transform.position;
        }
    }

    public IEnumerator StartMoving()
    {
        while (enabled)
        {
            Vector3 pos = CalculatePlatformMovement();
            transform.position += pos;
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (globalWaypoints != null)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < globalWaypoints.Length - 1; i++)
            {
                Gizmos.DrawLine(globalWaypoints[i] + transform.position, globalWaypoints[i + 1] + transform.position);
            }

            Gizmos.color = Color.red;
            float size = .16f;

            for (int i = 0; i < globalWaypoints.Length; i++)
            {
                Vector3 globalWaypointsPos = (Application.isPlaying) ? globalWaypoints[i] : globalWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointsPos - Vector3.up * size, globalWaypointsPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointsPos - Vector3.left * size, globalWaypointsPos + Vector3.left * size);
            }

        }
    }
}