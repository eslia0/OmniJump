using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RayCastController : MonoBehaviour {
    
    public LayerMask grounds;

    public RaycastOrigins raycastOrigins;
    [HideInInspector] public BoxCollider2D boxCollider;

    public const float skinWitdth = 0.015f;
    public int horizontalRayCount;
    public int verticalRayCount;
    //value of RayCast Counts

    [HideInInspector] public float horiaontalRaySpacing;
    [HideInInspector] public float verticalRaySpacing;
    //value of Raycasts space

    public virtual void Start()
    {
        if(GetComponent<BoxCollider2D>())
            boxCollider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCollider.bounds;
        //set BoxCollider2D's bounds
        bounds.Expand(skinWitdth * -2);
        //Expand(or in this case Shrink) BoxCollider2D's bounds witdth to -0.03

        raycastOrigins.BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.BottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.TopLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.TopRight = new Vector2(bounds.max.x, bounds.max.y);
        //set Ray in each edge of character
        //minimum x equals left
        //minimum y equals down
    }

    //set the spaces between number of rays
    public void CalculateRaySpacing()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWitdth * -2);

        //set Ray Counts to min 2 and max 2147483647
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        //set space between rays
        horiaontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);//0.035 = 0.14 / (*8 -1)
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);//0.035 = 0.14 / (*8 -1)
    }

    public struct RaycastOrigins//RaycastOrigins 구조체 선언
    {
        public Vector2 TopLeft, TopRight, BottomLeft, BottomRight;
    }
}