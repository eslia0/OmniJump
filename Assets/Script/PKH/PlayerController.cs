using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

[RequireComponent(typeof(Controller))]
public class PlayerController : MonoBehaviour
{
    public float timer;

    public delegate void PlayerBehaviour();
    public PlayerBehaviour movementController;

    public bool KeyBoardControll = true;

    [Header("프리팹")]
    [SerializeField] private Transform body;
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
    [SerializeField] private Transform face;
    private int currentFace;
    private Controller controller;

    // 점프
    [Header("점프"), Space(10)]
    [SerializeField] private float timeOfJumpApex = 0.4f;
    [SerializeField] private float jumpHeight = 1f;
    // [SerializeField] private float maxGravity = 2f;
    [HideInInspector] public Jump jumpFun;
    [HideInInspector] public bool isTargetJump = false;
    [HideInInspector] public bool revertGravity = false;
    private float gravity;
    private float jumpVelocity;

    // 이동
    [Header("이동"), Space(10)]
    public float moveSpeed = 3.2f;
    [HideInInspector] public bool onClick;
    [HideInInspector] public bool moveRight = true;
    [HideInInspector] public Vector3 slowVelocity;
    public Vector3 velocity;
    private float velocityXSmoothing;

    // 회전
    private bool delegateRotate = false;
    private int zAxis = 0;
    public int rotationZ 
    {
        get { return (zAxis % 4); }
        set {
            zAxis += value;

            faceDirection = faceDirection;
            targetAngle = zAxis * 90;
            currentAngle = ((int)body.eulerAngles.z / 10) * 10;
            increaseAmount = ((value == 1) ? 10 : -10);

            if (!delegateRotate)
            {
                delegateRotate = true;
                movementController += RotationZ;
            }
        }
    }
    private int targetAngle = 0;
    private int currentAngle = 0;
    private int increaseAmount = 0;

    private Animator ani;


    private void Awake()
    {
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeOfJumpApex, 2); // timeOfJumpApex^2 (d = Vi * t + 1/2 * a * t^2)
        jumpVelocity = Mathf.Abs(gravity) * timeOfJumpApex; // Vf = Vi + a * t

        jumpFun = GetComponent<Jump>();
        controller = GetComponent<Controller>();
        ani = GetComponent<Animator>();

        transform.position = returnPoint.transform.position;

        MovementReset();
    }

    private void MovementReset()
    {
        GetComponent<BoxCollider2D>().enabled = true;
        body.gameObject.SetActive(true);
        controller.enabled = true;

        movementController = null;
        velocity = Vector2.zero;
        revertGravity = false;
        moveRight = true;
        faceDirection = 0;
        rotationZ = 0;
    }

    private void Start()
    {
        ani.enabled = false;
    }

    private void Update()
    {
        //timer = Time.time;
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            int i = ((Input.GetAxisRaw("Horizontal") == 1) ? 0 : 2);
            faceDirection = i % 4;
            onClick = true;
            KeyBoardControll = true;
        }
        else if (Input.GetAxisRaw("Vertical") != 0)
        {
            int i = ((Input.GetAxisRaw("Vertical") == 1) ? 1 : 3);
            faceDirection = i % 4;
            onClick = true;
            KeyBoardControll = true;
        }
        else if (KeyBoardControll)
        {
            onClick = false;
        }

        velocity.x = moveSpeed * ((moveRight) ? 1 : -1);
        velocity.y += gravity * Time.deltaTime * ((revertGravity)?-1:1);

        movementController?.Invoke();
        controller.Move(velocity * Time.deltaTime);

        if (controller.collisioninfo.above || controller.collisioninfo.below)
        {
            velocity.y = 0;
        }

        if (transform.position.y >= EndlessManager.Instance.NowPlatform.highPoint.position.y 
            || transform.position.y <= EndlessManager.Instance.NowPlatform.lowPoint.position.y - 1.28f)
            Dead();
    }

    /*
    if (!revertGravity)
    {
        if(velocity.y > -maxGravity)
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }
    else
    {
        if(velocity.y < maxGravity)
        {
            velocity.y -= gravity * Time.deltaTime;
        }
    }
    */

    private void RotationZ()
    {
        if (currentAngle != targetAngle)
        {
            currentAngle += increaseAmount;
            body.localRotation = Quaternion.Euler(0, 0, currentAngle);
        } else
        {
            body.localRotation = Quaternion.Euler(0, 0, targetAngle);
            delegateRotate = false;
            movementController -= RotationZ;
        }
    }

    public void SetJump(bool jump)
    {
        if (jump)
        {
            movementController -= jumpFun.JumpMove;
            velocity.y = jumpVelocity * ((revertGravity) ? -1 : 1);
        }
        else
        {
            velocity = Vector3.zero;
            movementController -= jumpFun.JumpMove;
            movementController += jumpFun.JumpMove;
        }
    }

    public void Dead()
    {
        Destroy(Instantiate(deathParticle, transform.position, Quaternion.identity), 1.5f);

        GetComponent<BoxCollider2D>().enabled = false;
        body.gameObject.SetActive(false);
        controller.enabled = false;
        enabled = false;

        CameraFollow.mainCam.transform.GetComponentInChildren<ButtonInput>().SetResultPanel();
    }

    public IEnumerator HoldPlayer(float time)
    {
        enabled = false;
        body.gameObject.SetActive(false);
        yield return new WaitForSeconds(time);
        enabled = true;
        body.gameObject.SetActive(true);
        EndlessManager.Instance.GetPoofPrefab(transform);
    }
}
