using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    private UIController controller;

    public Transform body;
    public Transform face;
    
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
    public bool moveRight = true;
    public Vector3 velocity;

    private float movePeriod = 1.0f;
    private float upSpeed;
    private float startTime;

    private bool isJump;
    public bool isMoving;
    public bool isTeleporting;
    public bool isActing;
    private Animator ani;

    [HideInInspector] public Transform selectedMap;

    [SerializeField] private GameObject idleAction;
    public List<InteractionUI> action = new List<InteractionUI>();

    private void Awake()
    {
        controller = GetComponent<UIController>();
        ani = GetComponent<Animator>();

        Init();
    }

    private void Init()
    {
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeOfJumpApex, 2); // timeOfJumpApex^2 (d = Vi * t + 1/2 * a * t^2)
        jumpVelocity = Mathf.Abs(gravity) * timeOfJumpApex; // Vf = Vi + a * t
        velocity = Vector2.zero;
        revertGravity = false;
        isMoving = false;
        moveRight = true;
        moveSpeed = 3.0f;

        if (!isTeleporting)
        {
            body.gameObject.SetActive(true);
        }
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
        else if (!isActing && action.Count == 0)
        {
            moveSpeed = 0;
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
            face.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if (action[0].dir == Direction.up)
        {
            face.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        }
        else if (action[0].dir == Direction.left)
        {
            face.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
        else if (action[0].dir == Direction.down)
        {
            face.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
        }
        
        if (action[0].action == InteractionUI.UIInteraction.Move)
        {
            moveSpeed = 3.0f;
            JoyStickImageCtrl ctrl = action[0].transform.GetComponentInChildren<JoyStickImageCtrl>();
            ctrl.SetJoyStickImage();

            if (moveRight)
            {
                while (transform.position.x < action[0].transform.position.x && moveRight)
                {
                    yield return null;
                }
            }
            else
            {
                while (transform.position.x > action[0].transform.position.x && !moveRight)
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
            MissileUI missile = action[0].GetComponent<MissileUI>();
            missile.isLunched = true;
            missile.StartCoroutine(missile.Lunch());
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
            if (moving.trigger)
            {
                moving.trigger.SetActive(false);
                GameObject blow = Instantiate(Resources.Load<GameObject>("Effects/TriggerBlow"));
                blow.transform.position = moving.trigger.transform.position;
                Destroy(blow, 0.5f);
            }
            
            if (moving.movePassanger)
            {
                moveSpeed = 0;
                isMoving = true;

                while (moving.enabled && isMoving)
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
        else if (action[0].action == InteractionUI.UIInteraction.Pause)
        {
            Vector3 targetPos = action[0].transform.position + new Vector3(0, 0.16f * ((transform.eulerAngles.z == 180) ? -1 : 1), 0);
            float time = 0;
            float timeToReachTarget = 0.5f;
            moveSpeed = 0f;

            while (Vector2.Distance(targetPos, transform.position) > 0.01f)
            {
                time += Time.deltaTime / timeToReachTarget;
                transform.position = Vector2.Lerp(transform.position, targetPos, time);
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            moveSpeed = 3f;
        }
        else if (action[0].action == InteractionUI.UIInteraction.Rotate)
        {
            body.rotation = Quaternion.Euler(action[0].rotation);
            SetJump();
        }
        else if (action[0].action == InteractionUI.UIInteraction.Dead)
        {
            Destroy(Instantiate(Resources.Load<GameObject>("Effects/GlowExplosion 1"), transform.position, Quaternion.identity), 1.5f);
            body.gameObject.SetActive(false);
            gravity = 0;
            velocity = Vector3.zero;
        }

        action.RemoveAt(0);

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

    // UI Map 선택
    public void SelectUIMap(Transform map)
    {
        selectedMap = map;
        InitActions();
    }

    // UI Map의 Action 초기화
    public void InitActions()
    {
        Init();
        action.Clear();

        for (int i = 0; i < selectedMap.childCount; i++)
        {
            InteractionUI add = selectedMap.GetChild(i).GetComponent<InteractionUI>();
            action.Add(add);

            if (add.action == InteractionUI.UIInteraction.Move)
            {
                JoyStickImageCtrl[] ctrls = add.GetComponentsInChildren<JoyStickImageCtrl>();
                for (int j = 0; j < ctrls.Length; j++)
                {
                    ctrls[j].enabled = true;
                }
            }
            else if (add.action == InteractionUI.UIInteraction.Moving)
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
        
        if (idleAction)
        {
            for (int i = 0; i < idleAction.transform.childCount; i++)
            {
                action.Add(idleAction.transform.GetChild(i).GetComponent<InteractionUI>());
            }
        }
    }

    // Teleport에서 사용
    public IEnumerator HoldPlayer(Transform exit, float time)
    {
        yield return null;

        moveSpeed = 0.0f;
        body.gameObject.SetActive(false);
        GameObject poof = Instantiate(Resources.Load<GameObject>("Effects/Poof 1"));
        poof.transform.position = exit.position + Vector3.up * 0.3f;
        Destroy(poof, 0.5f);

        float check = 0.0f;
        isTeleporting = true;
        while (check < time && isTeleporting)
        {
            check += Time.deltaTime;
            transform.position = exit.position;
            yield return null;
        }

        moveSpeed = 3.0f;
        body.gameObject.SetActive(true);

        if (isTeleporting)
        {
            isTeleporting = false;
            poof = Instantiate(Resources.Load<GameObject>("Effects/Poof 1"));
            poof.transform.position = transform.position + Vector3.up * 0.3f;
            Destroy(poof, 0.5f);
        }
    }
}
