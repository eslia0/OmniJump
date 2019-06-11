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

    private void Awake()
    {
        GetComponent<BoxCollider2D>().size = transform.GetChild(0).GetComponent<SpriteRenderer>().size * transform.GetChild(0).transform.localScale;
        GetComponent<BoxCollider2D>().offset = transform.GetChild(0).localPosition;
    }

    public void Start()
    {
        for (int i = 0; i < globalWaypoints.Length; i++)
        {
            globalWaypoints[i] += transform.position;
        }
    }
    
    public Vector3 CalculatePlatformMovement()
    {
        Vector3 vec = globalWaypoints[waypointIndex + 1] - globalWaypoints[waypointIndex];
        float distance = Vector3.Magnitude(vec) / speed;

        currentPercent = Vector3.Magnitude(globalWaypoints[waypointIndex] - transform.position) / Vector3.Magnitude(vec);
        
        if (currentPercent >= 1.0f)
        {
            transform.position = globalWaypoints[globalWaypoints.Length - 1];
            return globalWaypoints[waypointIndex + 1] - transform.position;
        }

        return vec / distance;
    }

    public IEnumerator StartMoving(PlayerUIController player)
    {
        while (currentPercent < 1f)
        {
            Vector3 moveVector = CalculatePlatformMovement();
            transform.Translate(moveVector * Time.deltaTime);

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