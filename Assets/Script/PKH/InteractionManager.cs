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
    
    [Header("탐지 방향")]
    public Direction direction;

    [Header("동작")]
    [SerializeField] private ActionJump jump;
    [SerializeField] private ActionETC action;

    private Transform jumpHeight;
    private bool playerIsOn;
    private bool inputDecrease = false;

    [Header("동작 가능 횟수")]
    [SerializeField] private int input = 1;

    [Header("동작 변수")]
    [SerializeField] private bool exploudOnDeath;    
    [SerializeField] private bool ReverseRight;
    [SerializeField] private bool randomDirection;
    [SerializeField] private float jumpDuration = 1;
    [SerializeField] private Transform teleportExit;
    [SerializeField] private Transform nextTarget;
    [SerializeField, Range(-1, 1)] private int rotateDirection;

    [Header("회전이 필요한 파티클")]
    [SerializeField] private ParticleSystem[] fourWayParticle;
    [SerializeField] private ParticleSystem[] rotationParticles;

    private void Awake()
    {
        if (randomDirection)
        {
            direction = Creater.Instance.randomizer.RandomizeDirection();
        }
    }

    private void Start()
    {
        if (fourWayParticle != null)
        {
            Creater.Instance.particleRotation.SetParticlesFourWayDirection(direction, fourWayParticle);
        }

        if (rotationParticles != null)
        {
            Creater.Instance.particleRotation.SetParticlesRotation(transform.eulerAngles.z, rotationParticles);
        }

        if(action == ActionETC.None && jump == ActionJump.None)
        {
            enabled = false;
            return;
        } else if(input == 0)
        {
            input = 1;
        }

        if (rotateDirection == 0)
        {
            rotateDirection = 1;
        }

        if (nextTarget != null)
        {
            if (nextTarget.childCount > 0)
            {
                jumpHeight = nextTarget.GetChild(0);
            }
            else
            {
                jumpHeight = nextTarget;
                jumpHeight.position += new Vector3(0, 0.5f, 0);
            }
        }
    }
    
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

            if (jump == ActionJump.jump)
            {
                Creater.Instance.player.SetJump(true);
                Creater.Instance.AddScore(10);
                tmp = false;
            }
            else if (jump == ActionJump.targetJump)
            {
                Creater.Instance.player.jumpFun.SetJump(nextTarget.position, jumpHeight.position, jumpDuration);
                Creater.Instance.AddScore(10);
                tmp = false;
            }
            else if(jump == ActionJump.inputJump)
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
                if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && FaceCompare())
                {
                    tmp = false;
                    Creater.Instance.player.SetJump(true);
                    Creater.Instance.AddScore(10);
                }
            }
            else if(jump == ActionJump.targetInputJump)
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
                if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && FaceCompare())
                {
                    tmp = false;
                    Creater.Instance.player.jumpFun.SetJump(nextTarget.position, jumpHeight.position, jumpDuration);
                    Creater.Instance.AddScore(10);
                }
            }

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
            if (action == ActionETC.rotate)
            {
                playerIsOn = false;
                Creater.Instance.player.rotationZ += rotateDirection;
            }
            else if (action == ActionETC.reverse)
            {
                playerIsOn = false;
                Creater.Instance.player.moveRight = ReverseRight;
                Creater.Instance.AddScore(15);
            }
            else if (action == ActionETC.reverseRotate)
            {
                playerIsOn = false;
                Creater.Instance.player.moveRight = ReverseRight;
                Creater.Instance.player.rotationZ += rotateDirection;
            }
            else if (action == ActionETC.gravityReverse)
            {
                if (Creater.Instance.player.onClick)
                {
                    Creater.Instance.player.onClick = false;

                    if (FaceCompare())
                    {
                        playerIsOn = false;
                        Creater.Instance.player.isJump = true;
                        Creater.Instance.player.velocity.y += (Creater.Instance.player.revertGravity) ? -3 : 3;
                        Creater.Instance.player.revertGravity = !Creater.Instance.player.revertGravity;
                        Creater.Instance.AddScore(15);
                    }
                }
                if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && FaceCompare())
                {
                    playerIsOn = false;

                    Creater.Instance.player.isJump = true;
                    Creater.Instance.player.velocity.y += (Creater.Instance.player.revertGravity) ? -3 : 3;
                    Creater.Instance.player.revertGravity = !Creater.Instance.player.revertGravity;
                    Creater.Instance.AddScore(15);
                }
            }
            else if (action == ActionETC.teleportEnter)
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
                            Creater.Instance.player.moveRight = ReverseRight;
                            Creater.Instance.AddScore(20);
                        }
                    }
                    if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && FaceCompare())
                    {
                        playerIsOn = false;

                        // 텔레포트 이동 파티클 생성
                        Creater.Instance.GetPoofPrefab(transform);

                        Creater.Instance.player.velocity = Vector3.zero;
                        Creater.Instance.player.gameObject.transform.position = teleportExit.position;
                        Creater.Instance.player.moveRight = ReverseRight;
                        Creater.Instance.AddScore(20);
                    }
                }
            } else if(action == ActionETC.score)
            {
                Creater.Instance.AddScore(50);
            }
        }

        if (!playerIsOn)
        {
            input--;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerIsOn = true;
            Creater.Instance.player.onClick = false;

            if (FaceCompare())
            {
                Jump();
                ETC();
                
                if (input <= 0)
                {
                    Dead();
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player" && FaceCompare() && playerIsOn)
        {
            Jump();
            ETC();
            
            if (input <= 0)
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
