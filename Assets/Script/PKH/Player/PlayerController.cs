using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

[RequireComponent(typeof(Controller))]
public class PlayerController : MonoBehaviour
{
    public delegate void PlayerBehaviour();
    public PlayerBehaviour movementController;

    public bool KeyBoardControll = true;

    [Header("프리팹")]
    [SerializeField] private Transform body;
    [SerializeField] private Transform face;
    [SerializeField] private GameObject deathParticle;
    [SerializeField] private Transform returnPoint;

    // 방향 설정
    [Header("액션변수"), Space(10)]
    public Direction interactionDirection;
    public int faceDirection {
        get { return currentFace; }
        set {
            currentFace = ((value + 4) % 4);
            interactionDirection = (Direction)((currentFace + rotationZ + 4) % 4);

            face.localRotation = Quaternion.Euler(0, 0, currentFace * 90);
        }
    }
    private int currentFace;
    private Controller controller;

    // 점프
    [Header("점프"), Space(10)]
    [SerializeField] private float timeOfJumpApex = 0.4f;
    [SerializeField] private float jumpHeight = 1f;
    // [SerializeField] private float maxGravity = 2f;
    [HideInInspector] public Jump jumpFun;
    [HideInInspector] public bool isTargetJump = false;
    /*[HideInInspector]*/ public bool revertGravity = false;
    private float gravity;
    private float jumpVelocity;

    // 이동
    [Header("이동"), Space(10)]
    public bool moveRight = true;
    public float moveSpeed = 3.2f;
    [HideInInspector] public bool onClick;
    [HideInInspector] public Vector3 slowVelocity;
    public Vector3 velocity;
    private float velocityXSmoothing;

    // 회전
    [SerializeField] private int zAxis = 0;
    public int rotationZ {
        get { return (zAxis % 4); }
        set {
            zAxis += value;

            faceDirection = faceDirection;
            targetAngle = zAxis * 90;
            increaseAmount = ((value == 1) ? 10 : -10);

            movementController -= RotationZ;
            movementController += RotationZ;
        }
    }
    private int targetAngle = 0;
    private int currentAngle = 0;
    private int increaseAmount = 0;

    private bool isDead = false;
    public bool IsDead {
        get { return isDead; }
        private set { isDead = value; }
    }
    private Animator ani;
    public PlayerSkinManager skinManager;
    public Transform reviveSpot;

    private void Awake()
    {
        skinManager = GetComponent<PlayerSkinManager>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeOfJumpApex, 2); // timeOfJumpApex^2 (d = Vi * t + 1/2 * a * t^2)
        jumpVelocity = Mathf.Abs(gravity) * timeOfJumpApex; // Vf = Vi + a * t

        jumpFun = GetComponent<Jump>();
        controller = GetComponent<Controller>();
        ani = GetComponent<Animator>();

        transform.position = returnPoint.transform.position;

        GetComponent<BoxCollider2D>().enabled = true;
        body.gameObject.SetActive(true);
        controller.enabled = true;

        movementController = null;
        velocity = Vector2.zero;
        faceDirection = 0;
        rotationZ = 0;
        isDead = false;
    }

    private void Start()
    {
        ani.enabled = false;
    }

    private void Update()
    {
        if (transform.position.y >= Creater.Instance.nowPlatform.highPoint.position.y
            || transform.position.y <= Creater.Instance.nowPlatform.lowPoint.position.y)
        {
            Dead();
            return;
        }

        if (Creater.Instance.isPaused)
        {
            return;
        }

#if UNITY_EDITOR
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            int i = ((Input.GetAxisRaw("Horizontal") == 1) ? 0 : 2);
            faceDirection = i % 4;
            onClick = true;
            KeyBoardControll = true;
        }
        else if (Input.GetAxisRaw("Vertical") != 0) {
            int i = ((Input.GetAxisRaw("Vertical") == 1) ? 1 : 3);
            faceDirection = i % 4;
            onClick = true;
            KeyBoardControll = true;
        }
        else if (KeyBoardControll)
        {
            onClick = false;
        }
#endif

        velocity.x = moveSpeed * ((moveRight) ? 1 : -1);
        velocity.y += gravity * Time.deltaTime * ((revertGravity) ? -1 : 1);

        movementController?.Invoke();
        controller.Move(velocity * Time.deltaTime);

        if (controller.collisioninfo.above || controller.collisioninfo.below)
        {
            velocity.y = 0;
        }

        if (isTargetJump && ((revertGravity && controller.collisioninfo.above) || (!revertGravity && controller.collisioninfo.below)))
        {
            isTargetJump = false;
            movementController -= jumpFun.JumpMove;
        }
    }

    private void RotationZ()
    {
        if (currentAngle != targetAngle)
        {
            currentAngle += increaseAmount;
            body.localRotation = Quaternion.Euler(0, 0, currentAngle);
        }
        else
        {
            body.localRotation = Quaternion.Euler(0, 0, targetAngle);
            movementController -= RotationZ;
        }
    }

    public void SetJump(bool jump)
    {
        if (jump) // 일반 점프
        {
            movementController -= jumpFun.JumpMove;
            velocity.y = jumpVelocity * ((revertGravity) ? -1 : 1);
        }
        else // 타겟 점프
        {
            velocity = Vector3.zero;
            isTargetJump = true;
            movementController -= jumpFun.JumpMove;
            movementController += jumpFun.JumpMove;
        }
    }

    public void Dead()
    {
        Destroy(Instantiate(deathParticle, transform.position, Quaternion.identity), 1.5f);

        // 튜토리얼 시 부활
        if (reviveSpot)
        {
            moveRight = true;
            revertGravity = false;
            skinManager.FlipEffect(false);

            currentAngle = 0;
            zAxis = 0;
            rotationZ = 0;

            Creater.Instance.GetPoofPrefab(reviveSpot);

            StartCoroutine(HoldPlayer(reviveSpot, 1.5f));
        }
        else
        {
            isDead = true;
            CameraFollow.mainCam.GetComponent<CameraFollow>().follow = false;
            GetComponent<BoxCollider2D>().enabled = false;
            body.gameObject.SetActive(false);
            controller.enabled = false;
            enabled = false;

            if (SceneManagement.Instance.currentScene == "PracticeScene")
            {
                FindObjectOfType<PracticeUI>().SetResultPanel();
            }
            else if (SceneManagement.Instance.currentScene == "EndlessScene") {
                if (!Creater.Instance.isRewarded && (Creater.Instance.score >= SceneManagement.Instance.highScore * 0.5f || Creater.Instance.score >= 3000)) {
                    FindObjectOfType<EndlessUI>().SetAdPanel();
                }
                else {
                    FindObjectOfType<EndlessUI>().SetResultPanel();
                }
            }
        }
    }

    public IEnumerator HoldPlayer(Transform exit, float time)
    {
        enabled = false; // 캐릭터 움직임 봉쇄
        body.gameObject.SetActive(false); // 캐릭터 스프라이트 제거
        velocity = Vector3.zero;

        float check = 0.0f;
        while (check < time)
        {
            if (!Creater.Instance.isPaused)
            {
                check += Time.deltaTime;
                transform.position = exit.position;
            }
            yield return null;
        }

        if (!isDead)
        {
            enabled = true;
            body.gameObject.SetActive(true);
            Creater.Instance.GetPoofPrefab(transform);
        }
    }
}
