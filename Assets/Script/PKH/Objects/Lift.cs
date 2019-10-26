using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : RayCastController
{
    enum PlatformMode
    {
        Active, // 플레이어와 충돌 후 요구 방향을 입력 시 동작
        Passive, // 생성과 동시에 동작
        Trigger, // 플레이어의 X 위치가 더 높을 경우 동작
        Distance, // 플레이어와 직선 거리를 검토하여 동작
    }

    [Space(10), Header("플렛폼 모드")]
    [SerializeField] private PlatformMode mode;
    [SerializeField] private Direction direction;
    [SerializeField] private Transform trigger;
    private SpriteRenderer body;

    public Vector3[] globalWaypoints;

    [Header("동작 변수")]
    public bool moveOnce;
    public bool cyclic; // 움직임 반복 확인
    public bool movePassinger = false;
    public bool stopXSpeedOnMovePassinger = false;
    public bool disabledAfterMove = false;

    [Header("이동 변수")]
    public float speed;
    public float WaitTime;
    [Range(0, 3)] public float EaseAmount;

    private delegate void TriggerUpdate();
    private TriggerUpdate triggerUpdate;

    private bool isActive = false;
    private bool playerIsOn = false;
    private int fromWaypointIndex; // 멀어져야할 이전 원점
    private float nextMoveTime;
    private float percentBetweenWaypoints; // 두 점 사이의 간격 퍼센트 (0~1)

    private Vector3 velocity;


    private void OnEnable()
    {
        try
        {
            if(trigger == null)
            {
                trigger = transform.Find("Trigger");
            }
            ParticleSystem particle = trigger.GetComponent<ParticleSystem>();

            switch (mode)
            {
                case PlatformMode.Active:
                    switch (direction)
                    {
                        case Direction.right:
                            particle.startRotation = 90 * Mathf.Deg2Rad;
                            break;
                        case Direction.left:
                            particle.startRotation = 270 * Mathf.Deg2Rad;
                            break;
                        case Direction.down:
                            particle.startRotation = 180 * Mathf.Deg2Rad;
                            break;
                    }
                    particle.Play();
                    break;
                case PlatformMode.Passive:
                    isActive = true;
                    Destroy(trigger.gameObject);
                    break;
            }

            if (body == null)
                body = transform.Find("Body").GetComponent<SpriteRenderer>();

            GetComponent<BoxCollider2D>().size = body.size * body.transform.localScale;

            if (body && body.transform.localScale.x > 0 && body.transform.localScale.y > 0)
            {
                boxCollider.size = body.size * body.transform.localScale;
            }
        } catch(Exception e)
        {

        }
    }

    public override void Start()
    {
        base.Start();

        for (int i = 0; i < globalWaypoints.Length; i++)
        {
            globalWaypoints[i] += transform.localPosition;
        }
    }

    void Update()
    {
        if (Creater.Instance.isPaused)
        {
            return;
        }

        if (!isActive)
        {
            SwitchUpdate();
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

    private void SwitchUpdate()
    {
        switch (mode)
        {
            case PlatformMode.Active:
                Collider2D check = Physics2D.OverlapBox(trigger.position, new Vector2(0.16f, 0.16f), 0f, Creater.Instance.playerLayer);

                if (check
                && Creater.Instance.player.interactionDirection == direction
                && Creater.Instance.player.onClick)
                {
                    isActive = true;
                    Creater.Instance.AddScore(15);
                    Creater.Instance.GetTriggerBlowParticles(direction, trigger);
                    Destroy(trigger.gameObject);
                }
                break;
            case PlatformMode.Distance:
                if (Vector3.Distance(Creater.Instance.player.transform.position, trigger.position) < 0.16f)
                {
                    isActive = true;
                    Destroy(trigger.gameObject);
                }
                break;
            case PlatformMode.Trigger:
                if (Creater.Instance.player.transform.position.x > trigger.position.x)
                {
                    isActive = true;
                    Destroy(trigger.gameObject);
                }
                break;
        }
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

            // 최종 웨이포인트 도달
            if (toWaypointIndex == globalWaypoints.Length - 1)
            {
                if (!cyclic && Creater.Instance.player.moveSpeed == 0 && playerIsOn)
                {
                    StartCoroutine(SetPlayerAtTheEndOfFrame());
                }

                if (moveOnce)
                {
                    enabled = false;
                }

                if (disabledAfterMove)
                {
                    Destroy(gameObject);
                }
            }
        }

        return newPos - transform.localPosition;
    }

    private IEnumerator SetPlayerAtTheEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        Creater.Instance.player.moveSpeed = 3;

        yield return null;
    }

    private void CalculatePassengerMovement()
    {
        // When objects are on top of a horizontally or downward moving platform
        float rayLength = skinWitdth * 3;
        float distance = speed * 0.08f;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (((Creater.Instance.player.revertGravity) ? raycastOrigins.BottomLeft : raycastOrigins.TopLeft) + Vector2.right * (verticalRaySpacing * i)) - new Vector2(0, rayLength);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, ((Creater.Instance.player.revertGravity) ? Vector2.down : Vector2.up), distance, Creater.Instance.playerLayer);

            Debug.DrawLine(rayOrigin, 
                rayOrigin + (((Creater.Instance.player.revertGravity)? Vector2.down : Vector2.up) * distance), Color.blue);
            
            if (hit)
            {
                if (!playerIsOn)
                {
                    playerIsOn = true;
                    Creater.Instance.player.moveSpeed = (stopXSpeedOnMovePassinger) ? 0 : 3f;
                }
                
                Creater.Instance.player.transform.Translate(new Vector2((stopXSpeedOnMovePassinger) ? velocity.x : 0, velocity.y));
                return;
            }
        }

        if (playerIsOn)
        {
            playerIsOn = false;
            StartCoroutine(SetPlayerAtTheEndOfFrame());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsOn = false;
            if (Creater.Instance.player.moveSpeed != 3)
                Creater.Instance.player.moveSpeed = 3;
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