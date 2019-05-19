using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    enum ActionJump
    {
        None = 0,
        jump,
        targetJump,
        targetInputJump,
        inputJump
    }
    enum ActionETC
    {
        None = 0,
        reverse,
        rotate,
        teleportEnter,
        reverseRotate,
        gravityReverse,
        score,
    }

    /// 플레이어가 해당 오브젝트와 상호작용할 때,
    /// 오브젝트가 플레이어에게 요구하는 바라보는 방향
    [Header("탐지 방향")]
    public Direction direction;

    [Header("동작")]
    [SerializeField] private ActionJump jump; // 점프 종류 판단 enum
    [SerializeField] private ActionETC action; // 점프 제외 행동을 판단 enum

    private Transform jumpHeight;
    private bool playerIsOn;

    // 플레이어가 해당 오브젝트를 사용할 수 있는 횟수 변수
    [Header("동작 가능 횟수")]
    [SerializeField] private uint input = 1;
    public uint Input {
        get {
            return input;
        }
        private set {
            input = value;
        }
    }

    [Header("동작 변수")]
    [SerializeField] private bool exploudOnDeath;               // 사용 횟수가 0이되면 폭발하며 소멸할지 여부 확인   
    [SerializeField] private bool ReverseRight;                 // 플레이어와 상호작용시 플레이어 방향 변경 여부
    [SerializeField] private bool randomDirection;              // 4방향 회전이 가능한 파티클 및 방향의 랜덤성 부여 여부
    [SerializeField] private float jumpDuration = 1;            // 타겟 점프류의 점프 시간
    [SerializeField] private Transform teleportExit;            // 텔레포터 출구 위치
    [SerializeField] private Transform nextTarget;              // 키입력 타겟의 다음 타겟 위치
    [SerializeField, Range(-1, 1)] private int rotateDirection; // 좌우 회전 방향 결정 변수

    /// <summary>
    ///  현재 오브젝트에 존재하는 파티클 중 회전이 필요한 파티클들을 배열에 담아 특정 회전 함수에 호출되게 하는 변수
    ///  fourWayParticle배열 : 오브젝트가 요구하는 방향으로 4방향으로 회전을 해야하는 파티클의 모음 (화살표, 트리거 파티클)
    ///  rotationParticles배열 : 오브젝트의 회전 값만큼 회전을 해야하는 파티클의 모음 (텔레포트 파티클)
    /// </summary>
    [Header("회전이 필요한 파티클")]
    [SerializeField] private ParticleSystem[] fourWayParticle;
    [SerializeField] private ParticleSystem[] rotationParticles;


    private void Awake()
    {
        // 방향을 랜덤하게 생성할때 호출
        if (randomDirection)
        {
            direction = Creater.Instance.randomizer.RandomizeDirection();
        }

        /// 4방향 중 하나로 회전을 해야하는 파티클이 존재하는 경우,
        /// 해당 파티클을 회전시키기위한 함수를 호출
        if (fourWayParticle != null)
        {
            Creater.Instance.particleRotation.SetParticlesFourWayDirection(direction, fourWayParticle);
        }

        /// 현재 오브젝트의 회전 값을 통해 해당 리스트의 오브젝트 또한 회전시키는 함수 호출
        if (rotationParticles != null)
        {
            Creater.Instance.particleRotation.SetParticlesRotation(transform.eulerAngles.z, rotationParticles);
        }
    }

    private void Start()
    {
        if(action == ActionETC.None && jump == ActionJump.None)
        {
            enabled = false;
            return;
        }
        else if(input == 0) // 사용가능한 횟수가 초기 설정부터 0인 경우 1회 사용 가능하도록 변경
        {
            input = 1;
        }

        // 키입력 점프 전용 if문
        // 다음 목표가 존재하면 해당 목표의 첫 자식(점프 높이를 위한 프리펩)의 위치 확인
        if (nextTarget != null)
        {
            if (nextTarget.childCount > 0)
            {
                jumpHeight = nextTarget.GetChild(0);
            }
            else // 자식이 없으면 임의의 위치를 최대 높이로 설정
            {
                jumpHeight = nextTarget;
                jumpHeight.position += new Vector3(0, 0.5f, 0);
            }
        }
    }

    // 플레이어가 바라보는 방향과 요구하는 방향의 일치를 확인하는 함수
    private bool FaceCompare()
    {
        return (direction == (Direction)((Creater.Instance.player.rotationZ + Creater.Instance.player.faceDirection + 4) % 4));
    }

    private void Dead()
    {
        if (exploudOnDeath)
        {
            Creater.Instance.GetPopPrefab(transform);
            Destroy(gameObject);
        }
        enabled = false;
    }

    private void Jump()
    {
        if (playerIsOn && jump != ActionJump.None)
        {
            bool tmp = true;

            if (jump == ActionJump.jump) // 점프
            {
                Creater.Instance.player.SetJump(true);
                Creater.Instance.AddScore(10);
                tmp = false;
            }
            else if (jump == ActionJump.targetJump) // 타겟 점프
            {
                Creater.Instance.player.jumpFun.SetJump(nextTarget.position, jumpHeight.position, jumpDuration);
                Creater.Instance.AddScore(10);
                tmp = false;
            }
            else if(jump == ActionJump.inputJump) // 키입력 점프
            {
                if (Creater.Instance.player.onClick)
                {
                    Creater.Instance.player.onClick = false;

                    if (FaceCompare())
                    {
                        tmp = false;
                        Creater.Instance.player.SetJump(true);
                        Creater.Instance.AddScore(10);
                    }

                }
            }
            else if(jump == ActionJump.targetInputJump) // 키입력 타겟 점프
            {
                if (Creater.Instance.player.onClick)
                {
                    Creater.Instance.player.onClick = false;

                    if (FaceCompare())
                    {
                        tmp = false;
                        Creater.Instance.player.jumpFun.SetJump(nextTarget.position, jumpHeight.position, jumpDuration);
                        Creater.Instance.AddScore(10);
                    }
                }
            }

            // ETC 액션이 존재하지 않으면 플레이어가 패드를 사용한 여부를 확인
            if(action == ActionETC.None)
            {
                playerIsOn = tmp;
            }
        }
    }

    private void ETC()
    {
        if (playerIsOn && action != ActionETC.None)
        {
            if (action == ActionETC.rotate) // 회전
            {
                playerIsOn = false;
                Creater.Instance.player.rotationZ = rotateDirection;
            }
            else if (action == ActionETC.reverse) // 방향 전환
            {
                playerIsOn = false;
                Creater.Instance.player.moveRight = ReverseRight;
                Creater.Instance.AddScore(15);
            }
            else if (action == ActionETC.reverseRotate) // 회전 & 방향 전환
            {
                playerIsOn = false;
                Creater.Instance.player.moveRight = ReverseRight;
                Creater.Instance.player.rotationZ = rotateDirection;
            }
            else if (action == ActionETC.gravityReverse) // 중력패드
            {
                if (Creater.Instance.player.onClick)
                {
                    Creater.Instance.player.onClick = false;

                    if (FaceCompare())
                    {
                        playerIsOn = false;
                        // 중력패드 사용 직후 3의 속도를 추가한 만큼 y축 이동을 시킨다.
                        Creater.Instance.player.velocity.y += (Creater.Instance.player.revertGravity) ? -3 : 3;
                        Creater.Instance.player.revertGravity = !Creater.Instance.player.revertGravity;
                        Creater.Instance.AddScore(15);
                    }
                }
            }
            else if (action == ActionETC.teleportEnter) // 텔레포터
            {
                if (teleportExit)
                {
                    if (Creater.Instance.player.onClick)
                    {
                        Creater.Instance.player.onClick = false;

                        if (FaceCompare())
                        {
                            playerIsOn = false;

                            // 텔레포트 이동 파티클 생성
                            Creater.Instance.GetPoofPrefab(transform);
                            
                            Creater.Instance.player.velocity = Vector3.zero;
                            Creater.Instance.player.gameObject.transform.position = teleportExit.position;
                            Creater.Instance.player.moveRight = ReverseRight; // 텔로포터 사용 후 플레이어 방향 설정
                           
                            StartCoroutine(Creater.Instance.player.HoldPlayer(0.8f)); // 텔레포터 딜레이

                            Creater.Instance.AddScore(20);
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (action == ActionETC.score)
            {
                Creater.Instance.AddScore(50);
                Dead();
                return;
            }

            playerIsOn = true;
            Creater.Instance.player.onClick = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 플레이어 태그, 플레이어와 필요 방향, 플레이어의 접촉 여부, 사용 가능 횟수 확인
        if(collision.tag == "Player" && FaceCompare() && playerIsOn && input > 0)
        {
            Jump();
            ETC();

            if (!playerIsOn)
            {
                input--;
            }

            if (input < 1)
            {
                Dead();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerIsOn = false;
        }
    }
}
