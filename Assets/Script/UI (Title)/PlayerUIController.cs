using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    private UIController controller;

    [SerializeField] private Transform body;

    // 점프
    [Header("점프"), Space(10)]
    [SerializeField] private float timeOfJumpApex = 0.3f;
    [SerializeField] private float jumpHeight = 1.2f;
    [HideInInspector] public bool revertGravity = false;
    private float gravity;
    private float jumpVelocity;

    // 이동
    [Header("이동"), Space(10)]
    public float moveSpeed = 3.0f;
    [HideInInspector] public bool moveRight = true;
    public Vector3 velocity;

    private float movePeriod = 1.0f;
    private float upSpeed;
    private float startTime;

    private bool isJump;
    public bool isActing;
    private Animator ani;

    private GameObject[] UIMaps;
    [HideInInspector] public GameObject selectedMap;
    [SerializeField] private GameObject idleAction;
    [SerializeField] private List<InteractionUI> action = new List<InteractionUI>();

    private void Awake()
    {
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeOfJumpApex, 2); // timeOfJumpApex^2 (d = Vi * t + 1/2 * a * t^2)
        jumpVelocity = Mathf.Abs(gravity) * timeOfJumpApex; // Vf = Vi + a * t
        
        controller = GetComponent<UIController>();
        ani = GetComponent<Animator>();
        
        velocity = Vector2.zero;
        revertGravity = false;
        moveRight = true;

        InitUIMap();
        InitActions();
    }

    private void Update()
    {
        velocity.x = moveSpeed * ((moveRight) ? 1 : -1);
        velocity.y += gravity * Time.deltaTime * ((revertGravity)?-1:1);

        if (!isActing && action.Count > 0)
        {
            isActing = true;
            StartCoroutine(Action());
        }

        JumpMove();
        controller.Move(velocity * Time.deltaTime);

        if (controller.collisioninfo.above || controller.collisioninfo.below)
        {
            velocity.y = 0;
        }
    }

    public IEnumerator Action()
    {
        if (action[0].dir == Direction.right)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if (action[0].dir == Direction.up)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        }
        else if (action[0].dir == Direction.left)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
        else if (action[0].dir == Direction.down)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
        }
        
        if (action[0].action == InteractionUI.UIInteraction.Move)
        {
            moveSpeed = 3.0f;

            if (moveRight)
            {
                while (transform.position.x < action[0].transform.position.x)
                {
                    yield return null;
                }
            }
            else
            {
                while (transform.position.x > action[0].transform.position.x)
                {
                    yield return null;
                }
            }
        }
        else if (action[0].action == InteractionUI.UIInteraction.Jump)
        {
            if (action[0].usingEffect != null)
            {
                Destroy(Instantiate(action[0].usingEffect, action[0].transform.position, Quaternion.identity), 1.0f);
            }

            SetJump();
        }
        else if (action[0].action == InteractionUI.UIInteraction.Missile)
        {
            action[0].Lunch();
        }
        else if (action[0].action == InteractionUI.UIInteraction.Reverse)
        {
            moveRight = !moveRight;
        }
        else if (action[0].action == InteractionUI.UIInteraction.Gravity)
        {
            revertGravity = !revertGravity;
        }
        else if (action[0].action == InteractionUI.UIInteraction.Teleport)
        {
            StartCoroutine(HoldPlayer(action[0].exit.transform, 1.0f));
        }
        else if (action[0].action == InteractionUI.UIInteraction.Moving)
        {
            MovingPlatformUI moving = action[0].GetComponent<MovingPlatformUI>();
            
            if (moving.movePassanger)
            {
                moveSpeed = 0;

                while (moving.enabled)
                {
                    Vector3 pos = moving.CalculatePlatformMovement();
                    transform.position = moving.transform.position + pos;
                    moving.transform.position += pos;

                    yield return null;
                }

                moveSpeed = 3f;
            }
            else
            {
                StartCoroutine(moving.StartMoving());
            }
        }

        action.RemoveAt(0);

        if (action.Count == 0)
        {
            InitActions();
        }

        isActing = false;
    }

    // 일반 점프
    public void SetJump()
    {
        velocity.y = jumpVelocity * ((revertGravity) ? -1 : 1);
    }

    // 타겟 점프
    public void SetJump(Vector2 target, Vector2 height, float time)
    {
        isJump = true;
        velocity = Vector3.zero;
        movePeriod = time;

        float h1 = 0;
        float h2 = 0;

        // 각자의 높이
        if (!revertGravity)
        {
            h1 = height.y - transform.position.y; // 최대 높이 - 현재 높이 = 플레이어 높이
            h2 = height.y - target.y; // 최대 높이 - 목표 높이 = 목표 높이
        }
        else
        {
            h1 = transform.position.y - height.y; // 최대 높이 - 현재 높이 = 플레이어 높이
            h2 = target.y - height.y; // 최대 높이 - 목표 높이 = 목표 높이
        }

        // x축 전체 이동 길이
        float dis = (target.x - transform.position.x); // 두 점의 거리
        moveSpeed = dis; // 이동 속도 = 거리

        upSpeed = (((1.0f + Mathf.Sqrt(h2 / h1)) * 2.0f) * h1) * ((revertGravity) ? -1 : 1);
        gravity = ((Mathf.Pow(upSpeed, 2) / -2.0f) / h1) * ((revertGravity) ? -1 : 1);

        startTime = 0.0f;
    }

    public void JumpMove()
    {
        if (isJump)
        {
            velocity = Vector3.zero;
            transform.Translate(new Vector3(moveSpeed, upSpeed) * Time.deltaTime / movePeriod);

            upSpeed += gravity * Time.deltaTime / movePeriod;
            startTime += Time.deltaTime;

            if (startTime >= movePeriod)
            {
                isJump = false;
            }
        }
    }

    // UIMap을 초기화 후 선택
    private void InitUIMap()
    {
        UIMaps = new GameObject[10];

        for (int i = 0; i< 10; i++)
        {
            UIMaps[i] = Resources.Load<GameObject>("UIMaps/TitleMap" + (i+1).ToString());
        }

        int rand = Random.Range(0, UIMaps.Length);
        selectedMap = Instantiate<GameObject>(UIMaps[rand], new Vector3(4.8f, -1.76f, 0f), Quaternion.identity);
    }

    private void InitActions()
    {
        action.Clear();
        Transform mapAction = selectedMap.transform.GetChild(1);
        for (int i = 0; i < mapAction.childCount; i++)
        {
            InteractionUI add = mapAction.GetChild(i).GetComponent<InteractionUI>();
            action.Add(add);
            if (add.action == InteractionUI.UIInteraction.Moving)
            {
                add.GetComponent<MovingPlatformUI>().enabled = true;
                add.GetComponent<MovingPlatformUI>().Init();
            }
            else if (add.action == InteractionUI.UIInteraction.Missile)
            {
                add.gameObject.SetActive(true);
                add.GetComponent<MissileUI>().enabled = true;
                add.GetComponent<MissileUI>().Init();
            }
        }

        for (int i = 0; i < idleAction.transform.childCount; i++)
        {
            action.Add(idleAction.transform.GetChild(i).GetComponent<InteractionUI>());
        }
    }

    public IEnumerator HoldPlayer(Transform exit, float time)
    {
        moveSpeed = 0.0f;
        body.gameObject.SetActive(false);

        float check = 0.0f;
        while (check < time)
        {
            check += Time.deltaTime;
            transform.position = exit.position;
            yield return null;
        }

        moveSpeed = 3.0f;
        body.gameObject.SetActive(true);
    }
}
