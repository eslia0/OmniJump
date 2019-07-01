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
    public bool stopXSpeedOnMovePassinger = false;

    public float speed;
    [Range(0, 3)] public float EaseAmount;
    public float WaitTime;
    [HideInInspector] public float percentBetweenWaypoints; // 두 점 사이의 간격 퍼센트 (0~1)

    private bool playerIsOn = false;
    private int fromWaypointIndex; // 멀어져야할 이전 원점
    private float nextMoveTime;

    private Vector3 velocity;
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

        velocity = CalculatePlatformMovement();

        if (movePassinger)
        {
            CalculatePassengerMovement();
        }

        transform.Translate(velocity);
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

            if (toWaypointIndex == globalWaypoints.Length - 1)
            {
                if (stopXSpeedOnMovePassinger && playerIsOn)
                {
                    Creater.Instance.player.moveSpeed = 3;
                }

                if (moveOnce)
                {
                    enabled = false;
                }
            }
        }

        return newPos - transform.position;
    }

    private void CalculatePassengerMovement()
    {
        // When objects are on top of a horizontally or downward moving platform
        float rayLength = skinWitdth * 3;
        float distance = speed * 0.03f;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (((Creater.Instance.player.revertGravity) ? raycastOrigins.BottomLeft : raycastOrigins.TopLeft)
                + Vector2.right * (verticalRaySpacing * i)) - new Vector2(0, rayLength);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin,
                ((Creater.Instance.player.revertGravity) ? Vector2.down : Vector2.up), distance, Creater.Instance.playerLayer);

            //Debug.DrawRay(rayOrigin, hit.point, Color.blue);

            if (hit)
            {
                if (stopXSpeedOnMovePassinger && !playerIsOn)
                {
                    playerIsOn = true;
                    Creater.Instance.player.moveSpeed = 0;
                }
                Creater.Instance.player.transform.Translate(new Vector2(0, velocity.y));
                break;
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