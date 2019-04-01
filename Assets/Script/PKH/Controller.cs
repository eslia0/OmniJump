using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Automatically add BoxCollider2D
[RequireComponent(typeof(BoxCollider2D))]
public class Controller : RayCastController { // Extends RayCastController script

    PlayerController player;

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
    
    [Range(0,90f)] public float MaxClimbing;
    [Range(0, 90f)] public float MaxDescendAngle;
    public CollisionInfo collisioninfo;


    public override void Start()
    {
        // call RayCastController's Start method first; continue the Start method
        base.Start();
        player = GetComponent<PlayerController>();
    }

    public void Move(Vector3 velocity, bool isOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisioninfo.Reset();
        collisioninfo.velocityOld = velocity;

        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }

        if (velocity.x != 0)
        {
            HorizontalCollision(ref velocity);
        }

        if (velocity.y != 0)
        {
            VerticalCollision(ref velocity);
        }

        transform.Translate(velocity);

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
                if(hit.distance == 0)
                {
                    continue;
                }

                player.Dead();
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

                if (collisioninfo.ClimbingSlope)
                {
                    //While climbing a slope if player hit's objects above him
                    velocity.x = velocity.y / Mathf.Tan(collisioninfo.SlopAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                //if directionY equals -1 or 1, bool values turns ture
                collisioninfo.below = directionY == -1;
                collisioninfo.above = directionY == 1;
            }
        }

        if (collisioninfo.ClimbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x)+skinWitdth;

            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.BottomLeft : raycastOrigins.BottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, grounds);

            if (hit)
            {
                float slopAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(slopAngle != collisioninfo.SlopAngle)
                {
                    velocity.x = (hit.distance - skinWitdth) * directionX;
                    collisioninfo.SlopAngle = slopAngle;
                }
            }
        }
    }

    void ClimbSlops(ref Vector3 velocity, float slopAngle) // use trigonometry to find X and Y's velocity
    {
        float ClimeBelocityY = Mathf.Sin(slopAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x); // y = sin(theta) * deltaMove

        if(velocity.y <= ClimeBelocityY)
        {
            velocity.y = ClimeBelocityY;
            //x = cos(theta) * deltaMove * direction
            velocity.x = Mathf.Cos(slopAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x) * Mathf.Sign(velocity.x);

            collisioninfo.below = true;
            collisioninfo.ClimbingSlope = true;
            collisioninfo.SlopAngle = slopAngle;
        }
    }

    void DescendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.BottomRight : raycastOrigins.BottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, grounds);

        if (hit)
        {
            float slopAngle = Vector2.Angle(hit.normal, Vector2.up);
            if(slopAngle != 0 && slopAngle <= MaxDescendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if(hit.distance - skinWitdth <= Mathf.Tan(slopAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float DescendVelocityY = Mathf.Sin(slopAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x); // y = sin(theta) * deltaMove
                        velocity.x = Mathf.Cos(slopAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x) * Mathf.Sign(velocity.x);
                        velocity.y -= DescendVelocityY;

                        collisioninfo.SlopAngle = slopAngle;
                        collisioninfo.descendingSlope = true;
                        collisioninfo.below = true;
                    }
                }
            }
        }
    }
}
