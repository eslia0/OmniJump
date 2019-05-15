using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Automatically add BoxCollider2D
[RequireComponent(typeof(BoxCollider2D))]
public class Controller : RayCastController { // Extends RayCastController script

    public struct CollisionInfo
    {
        public bool above, below, left, right;
        public bool ClimbingSlope;
        public bool descendingSlope;

        public float SlopAngle, SlopAngleOld;
        public Vector3 velocityOld;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            ClimbingSlope = descendingSlope = false;

            SlopAngleOld = SlopAngle;
            SlopAngle = 0;
        }
    }
    
    public CollisionInfo collisioninfo;

    private Vector3 velocityDummy = new Vector3(skinWitdth, 0, 0);

    public override void Start()
    {
        // call RayCastController's Start method first; continue the Start method
        base.Start();
    }

    public void Move(Vector3 velocity, bool isOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisioninfo.Reset();
        collisioninfo.velocityOld = velocity;
        
        if (velocity.x != 0)
        {
            HorizontalCollision(ref velocity);
        }
        else
        {
            Vector3 tmp = velocityDummy * (EndlessManager.Instance.player.moveRight ? 1 : -1);
            HorizontalCollision(ref tmp);
        }

        if (velocity.y != 0)
        {
            VerticalCollision(ref velocity);
        }

        transform.position += velocity;

        if (isOnPlatform)
        {
            collisioninfo.below = true;
        }
    }
    
    public void HorizontalCollision(ref Vector3 velocity)
    {
        //Mathf.Sign : 
        //if velocity.x equals to positive or 0, return 1 to directionX(Right)
        //if velocity.x equals to negative, return -1 to directionX(Left)
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWitdth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            //if directionX equals to -1 = BottomLeft else TopRight
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.BottomLeft : raycastOrigins.BottomRight;
            rayOrigin += Vector2.up * (horiaontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, grounds);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.blue);

            if (hit)
            {
                EndlessManager.Instance.player.Dead();
                break;
            }
        }
    }

    void VerticalCollision(ref Vector3 velocity) // get reference to prevent value error
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWitdth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.BottomLeft : raycastOrigins.TopLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);//Vector (1,0) * (0.035 * i + 10)
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, grounds);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - skinWitdth) * directionY;
                rayLength = hit.distance;
                
                //if directionY equals -1 or 1, bool values turns ture
                collisioninfo.below = directionY == -1;
                collisioninfo.above = directionY == 1;
            }
        }
    }
}
