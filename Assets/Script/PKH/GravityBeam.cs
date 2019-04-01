using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBeam : MonoBehaviour
{
    [SerializeField] private bool up;
    [SerializeField] private Transform rayPoint;
    [SerializeField] private LayerMask ground;
    private LineRenderer line;
    
    // Use this for initialization
    void Update()
    {
        line = GetComponent<LineRenderer>();
        line.SetPosition(0, Vector3.zero);

        RaycastHit2D hit = Physics2D.Raycast(rayPoint.position, ((up) ? transform.up : -transform.up), ((up) ? 20 : -20), ground);

        if (hit.collider)
        {
            line.SetPosition(1, new Vector3(0, (hit.point.y - transform.position.y) * ((up)?1:-1), 0));    
        }
        else
        {
            line.SetPosition(1, new Vector3(0, ((up) ? 20 : -20), 0));
        }
    }
}
