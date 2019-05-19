using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : RayCastController
{
    public Vector3[] globalWaypoints;

    [HideInInspector] public bool isActive = true;
    [Header("동작 변수")]
    public bool moveOnce;
    public bool cyclic; // 움직임 반복 확인
    public bool movePassinger = false;
    public float speed;
    [Range(0, 3)] public float EaseAmount; 
    public float WaitTime;
    [HideInInspector] public float percentBetweenWaypoints; // 두 점 사이의 간격 퍼센트 (0~1)
    
    private int fromWaypointIndex; // 멀어져야할 이전 원점
    private float nextMoveTime;
    
    private List<PassengerMovement> passengerMovement;
    private Dictionary<Transform, Controller> passengerDictionary = new Dictionary<Transform, Controller>();


    private void Awake()
    {
        GetComponent<BoxCollider2D>().size = transform.GetChild(0).GetComponent<SpriteRenderer>().size * transform.GetChild(0).transform.localScale;
    }

    public override void Start()
    {
        base.Start();

        for (int i = 0; i < globalWaypoints.Length; i++)
        {
            globalWaypoints[i] += transform.position;
        }        
    }

    void Update()
    {
        if (!isActive)
        {
            return;
        }

        UpdateRaycastOrigins();

        Vector3 velocity = CalculatePlatformMovement();
        
        transform.Translate(velocity);
        /*
        if (movePassinger)
        {
            CalculatePassengerMovement(velocity);

            MovePassengers(true);
            transform.Translate(velocity);
            MovePassengers(false);
        }
        else
        {
            transform.Translate(velocity);
        }
        */
    }

    private float Ease(float x)
    {
        float a = EaseAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    Vector3 CalculatePlatformMovement()
    {
        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        fromWaypointIndex %= globalWaypoints.Length; // 멀어질 원점 웨이포인트 설정
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length; // 다음 웨이포인트 변수
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints; // 거리에따른 이동속도 저하
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints); // 두 점의 거리를 0~1로 표현
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints); // 속도 조정

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1) // 다음 웨이포인트에 도달시
        {
            if(toWaypointIndex == globalWaypoints.Length - 1)
            {
                if (movePassinger && Creater.Instance.player.transform.IsChildOf(transform))
                {
                    Creater.Instance.player.moveSpeed = 3;
                    Creater.Instance.player.transform.parent = null;
                }

                if (moveOnce)
                {
                    enabled = false;
                }
            }

            percentBetweenWaypoints = 0;
            fromWaypointIndex++;

            if (!cyclic)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1) // 원점 웨이포인트가 웨이포인트 배열보다 클때
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints); // 배열 반전
                }
            }
            nextMoveTime = Time.time + WaitTime;
        }

        return newPos - transform.position;
    }

    private void MovePassengers(bool beforMovePlatform)
    {
        foreach (PassengerMovement passenger in passengerMovement)
        {
            if (!passengerDictionary.ContainsKey(passenger.transform))
            {
                passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<Controller>());
            }

            if (passenger.moveBeforePlatform == beforMovePlatform)
            {
                // Debug.Log("Velocity out : " + passenger.velocity.x + ", " + passenger.velocity.y);
                passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
            }
        }
    }

    private void CalculatePassengerMovement(Vector3 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        /// Vertically moving platform
        /// When platform is moving up or down with player on it
        /// if its goes up player must move first and platform follows
        /// if its goes down platform must move first and player comes next
        /// if platfrom is moving up ray cast up, if it's moving down ray cast downward
        if (velocity.y != 0)//when platform is moving up or down
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWitdth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.BottomLeft : raycastOrigins.TopLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, Creater.Instance.playerLayer);

                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

                if (hit)
                {                    
                    //if the object hit the platform, platform checks if its new or known
                    //if the object is in the list. it doesn't loop for more VerticalRayCount and dosen't add more velocity
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);

                        //this line only runs when platform has velocity to x axis, witch means if it goes diagonally
                        //When player is on the platform velocity x does effect player, but if player is below the platform it doesn't
                        float pushX = (directionY == 1) ? velocity.x - (Creater.Instance.player.velocity.x * Time.deltaTime) : 0;
                        //hit distance between player and platform
                        float pushY = velocity.y - (hit.distance - skinWitdth) * directionY;
                        
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    }
                }
            }
        }

        //horizontally moving platform
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWitdth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.BottomLeft : raycastOrigins.BottomRight;
                rayOrigin += Vector2.up * (horiaontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, Creater.Instance.playerLayer);

                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.green);

                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - skinWitdth) * directionX;
                        float pushY = -skinWitdth;

                        //if platform moves vertically and hit passenger, it's impossible that passenger is on a platform.
                        //So, third value of passengerMovement is false, also forth is "true" because we want to move passenger before platform is moving
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                    }
                }
            }
        }

        // When objects are on top of a horizontally or downward moving platform
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = skinWitdth*3;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.TopLeft + Vector2.right * (verticalRaySpacing * i);
                rayOrigin -= new Vector2(0, rayLength);
                RaycastHit2D hitUp = Physics2D.Raycast(rayOrigin, Vector2.up, 1.2f, Creater.Instance.playerLayer);
                RaycastHit2D hitDown = Physics2D.Raycast(rayOrigin, Vector2.down, 1.2f, Creater.Instance.playerLayer);
                
                if (hitUp)
                {
                    Debug.DrawRay(rayOrigin, hitUp.point - new Vector2(transform.position.x, transform.position.y), Color.blue);

                    if (!movedPassengers.Contains(hitUp.transform))
                    {
                        movedPassengers.Add(hitUp.transform);
                        
                        float pushX = velocity.x - (Creater.Instance.player.velocity.x * Time.deltaTime);
                        float pushY = velocity.y;
                        
                        passengerMovement.Add(new PassengerMovement(hitUp.transform, new Vector3(pushX, pushY), true, false));
                    }
                }

                if (hitDown)
                {
                    Debug.DrawRay(rayOrigin, hitDown.point - new Vector2(transform.position.x, transform.position.y), Color.blue);

                    if (!movedPassengers.Contains(hitDown.transform))
                    {
                        movedPassengers.Add(hitDown.transform);

                        float pushX = velocity.x - (Creater.Instance.player.velocity.x * Time.deltaTime);
                        float pushY = -velocity.y;

                        passengerMovement.Add(new PassengerMovement(hitDown.transform, new Vector3(pushX, pushY), true, false));
                    }
                }

            }
        }
    }

    struct PassengerMovement//all value of passenger on platform
    {
        public Transform transform;//transform of passenger
        public Vector3 velocity;//velocity of passenger
        public bool standingOnPlatform;//weather or not player is on platform
        public bool moveBeforePlatform;//weather or not we move the passenger before platform move(Line 62~65)

        public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform)
        {            
            transform = _transform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (movePassinger && isActive)
            {
                Creater.Instance.player.moveSpeed = 0;
                Creater.Instance.player.transform.parent = transform;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(movePassinger && collision.tag == "Player")
        {
            Creater.Instance.player.moveSpeed = 3;
            Creater.Instance.player.transform.parent = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (globalWaypoints != null)
        {
            Gizmos.color = Color.cyan;
            for(int i = 0; i < globalWaypoints.Length-1; i++)
            {
                Gizmos.DrawLine(globalWaypoints[i] + transform.position, globalWaypoints[i+1] + transform.position);
            }

            Gizmos.color = Color.red;
            float size = .3f;

            for (int i = 0; i < globalWaypoints.Length; i++)
            {
                Vector3 globalWaypointsPos = (Application.isPlaying) ? globalWaypoints[i] : globalWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointsPos - Vector3.up * size, globalWaypointsPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointsPos - Vector3.left * size, globalWaypointsPos + Vector3.left * size);
            }

        }
    }
}