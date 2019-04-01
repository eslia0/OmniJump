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
    }
        
    public Direction direction;
    [SerializeField] private ActionJump jump;
    [SerializeField] private ActionETC action;

    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject teleportEffect;

    private Transform jumpHeight;
    [SerializeField] private bool canDestroy;
    [SerializeField] private bool isActivated;
    [SerializeField] private bool ReverseRight;
    [SerializeField] private bool randomDirection;
    [SerializeField] private float jumpDuration = 1;
    [SerializeField] private Transform teleportExit;
    [SerializeField] private Transform nextTarget;
    [SerializeField] private int input = 0;
    [SerializeField, Range(-1, 1)] private int rotateDirection;

    private void Awake()
    {
        if (randomDirection)
        {
            direction = (Direction)Random.RandomRange(0,3);
        }
    }

    private void OnDisable()
    {
        if (canDestroy)
        {
            if (isActivated)
            {
                Creater.Instance.GetPopPrefab(transform);
                //Destroy(Instantiate(explosion, transform.position, Quaternion.identity), 1.5f);
            }
            Destroy(gameObject);
        }
    }

    private void Start()
    {
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

        if (input == 0 && (jump == ActionJump.inputJump
            || jump == ActionJump.targetInputJump
            || action == ActionETC.gravityReverse
            || action == ActionETC.teleportEnter))
        {
            input = 1;
        }
    }

    private void Jump()
    {
        if (input == 0)
        {
            switch (jump)
            {
                case ActionJump.None:
                    return;

                case ActionJump.jump:
                    Creater.Instance.player.SetJump(true);
                    Creater.Instance.AddScore(10);
                    break;

                case ActionJump.targetJump:
                    Creater.Instance.player.jumpFun.SetJump(nextTarget.position, jumpHeight.position, jumpDuration);
                    Creater.Instance.AddScore(10);
                    break;
            }
        }
        else
        {
            switch (jump)
            {
                case ActionJump.jump:
                    Creater.Instance.player.SetJump(true);
                    input--;
                    Creater.Instance.AddScore(10);
                    break;

                case ActionJump.targetInputJump:
                    if (Creater.Instance.player.onClick)
                    {
                        Creater.Instance.player.onClick = false;
                        if (direction == (Direction)((Creater.Instance.player.rotationZ + Creater.Instance.player.faceDirection + 4) % 4))
                        {
                            Creater.Instance.player.jumpFun.SetJump(nextTarget.position, jumpHeight.position, jumpDuration);
                            input--;
                            Creater.Instance.AddScore(10);
                        }
                    }

                    if (Input.GetAxisRaw("Horizontal") != 0)
                    {
                        int i = ((Input.GetAxisRaw("Horizontal") == 1) ? 0 : 2);
                        if (direction == (Direction)((Creater.Instance.player.rotationZ + i + 4) % 4))
                        {
                            Creater.Instance.player.jumpFun.SetJump(nextTarget.position, jumpHeight.position, jumpDuration);
                            input--;
                            Creater.Instance.AddScore(10);
                        }
                    }
                    else if (Input.GetAxisRaw("Vertical") != 0)
                    {
                        int i = ((Input.GetAxisRaw("Vertical") == 1) ? 1 : 3);
                        if (direction == (Direction)((Creater.Instance.player.rotationZ + i + 4) % 4))
                        {
                            Creater.Instance.player.jumpFun.SetJump(nextTarget.position, jumpHeight.position, jumpDuration);
                            input--;
                            Creater.Instance.AddScore(10);
                        }
                    }
                    break;

                case ActionJump.inputJump:
                    if (Creater.Instance.player.onClick)
                    {
                        Creater.Instance.player.onClick = false;
                        if (direction == (Direction)((Creater.Instance.player.rotationZ + Creater.Instance.player.faceDirection + 4) % 4))
                        {
                            Creater.Instance.player.SetJump(true);
                            input--;
                            Creater.Instance.AddScore(10);
                        }
                    }

                    if (Input.GetAxisRaw("Horizontal") != 0)
                    {
                        int i = ((Input.GetAxisRaw("Horizontal") == 1) ? 0 : 2);
                        if (direction == (Direction)((Creater.Instance.player.rotationZ + i + 4) % 4))
                        {
                            Creater.Instance.player.SetJump(true);
                            input--;
                            Creater.Instance.AddScore(10);
                        }
                    }
                    else if (Input.GetAxisRaw("Vertical") != 0)
                    {
                        int i = ((Input.GetAxisRaw("Vertical") == 1) ? 1 : 3);
                        if (direction == (Direction)((Creater.Instance.player.rotationZ + i + 4) % 4))
                        {
                            Creater.Instance.player.SetJump(true);
                            input--;
                            Creater.Instance.AddScore(10);
                        }
                    }
                    break;
            }
        }
    }

    private void ETC()
    {
        if (input == 0)
        {
            switch (action)
            {
                case ActionETC.None:
                    break;

                case ActionETC.rotate:
                    Creater.Instance.player.rotationZ += rotateDirection;
                    break;

                case ActionETC.reverse:
                    Creater.Instance.player.moveRight = ReverseRight;
                    Creater.Instance.AddScore(15);
                    break;

                case ActionETC.reverseRotate:
                    Creater.Instance.player.rotationZ += rotateDirection;
                    Creater.Instance.player.moveRight = ReverseRight;
                    break;
            }

            enabled = false;
        }
        else
        {
            switch (action)
            {
                case ActionETC.gravityReverse:
                    if (Creater.Instance.player.onClick && isActivated)
                    {
                        Creater.Instance.player.onClick = false;
                        if (direction == (Direction)((Creater.Instance.player.rotationZ + Creater.Instance.player.faceDirection + 4) % 4))
                        {
                            isActivated = false;
                            Creater.Instance.player.isJump = true;
                            Creater.Instance.player.velocity.y += (Creater.Instance.player.revertGravity) ? -3 : 3;
                            Creater.Instance.player.revertGravity = !Creater.Instance.player.revertGravity;
                            input--;
                            Creater.Instance.AddScore(15);
                        }
                    }

                    if (Input.GetAxisRaw("Horizontal") != 0 && isActivated)
                    {
                        int i = ((Input.GetAxisRaw("Horizontal") == 1) ? 0 : 2);
                        if (direction == (Direction)((Creater.Instance.player.rotationZ + i + 4) % 4))
                        {
                            isActivated = false;
                            Creater.Instance.player.isJump = true;
                            Creater.Instance.player.velocity.y += (Creater.Instance.player.revertGravity) ? -3 : 3;
                            Creater.Instance.player.revertGravity = !Creater.Instance.player.revertGravity;
                            input--;
                            Creater.Instance.AddScore(15);
                        }
                    }
                    else if (Input.GetAxisRaw("Vertical") != 0 && isActivated)
                    {
                        int i = ((Input.GetAxisRaw("Vertical") == 1) ? 1 : 3);
                        if (direction == (Direction)((Creater.Instance.player.rotationZ + i + 4) % 4))
                        {
                            isActivated = false;
                            Creater.Instance.player.isJump = true;
                            Creater.Instance.player.velocity.y += (Creater.Instance.player.revertGravity) ? -3 : 3;
                            Creater.Instance.player.revertGravity = !Creater.Instance.player.revertGravity;
                            input--;
                            Creater.Instance.AddScore(15);
                        }
                    }
                    break;

                case ActionETC.teleportEnter:
                    if (teleportExit)
                    {
                        if (Creater.Instance.player.onClick)
                        {
                            Creater.Instance.player.onClick = false;
                            if (direction == (Direction)((Creater.Instance.player.rotationZ + Creater.Instance.player.faceDirection + 4) % 4))
                            {
                                Destroy(Instantiate(teleportEffect, teleportExit.position, Quaternion.identity), 1.5f);

                                Creater.Instance.player.velocity = Vector3.zero;
                                Creater.Instance.player.gameObject.transform.position = teleportExit.position;
                                Creater.Instance.player.moveRight = ReverseRight;
                                input--;
                                Creater.Instance.AddScore(20);
                            }
                        }

                        if (Input.GetAxisRaw("Horizontal") != 0)
                        {
                            int i = ((Input.GetAxisRaw("Horizontal") == 1) ? 0 : 2);
                            if (direction == (Direction)((Creater.Instance.player.rotationZ + i + 4) % 4))
                            {
                                Destroy(Instantiate(teleportEffect, teleportExit.position, Quaternion.identity), 1.5f);

                                Creater.Instance.player.velocity = Vector3.zero;
                                Creater.Instance.player.gameObject.transform.position = teleportExit.position;
                                Creater.Instance.player.moveRight = ReverseRight;
                                input--;
                                Creater.Instance.AddScore(20);
                            }
                        }
                        else if (Input.GetAxisRaw("Vertical") != 0)
                        {
                            int i = ((Input.GetAxisRaw("Vertical") == 1) ? 1 : 3);
                            if (direction == (Direction)((Creater.Instance.player.rotationZ + i + 4) % 4))
                            {
                                Destroy(Instantiate(teleportEffect, teleportExit.position, Quaternion.identity), 1.5f);

                                Creater.Instance.player.velocity = Vector3.zero;
                                Creater.Instance.player.gameObject.transform.position = teleportExit.position;
                                Creater.Instance.player.moveRight = ReverseRight;
                                input--;
                                Creater.Instance.AddScore(20);
                            }
                        }
                    }
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isActivated = true;

            if (input > 0)
            {
                Creater.Instance.player.onClick = false;
            }
            else if (Creater.Instance.player.interactionDirection == direction)
            {
                Jump();
                ETC();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player" && Creater.Instance.player.interactionDirection == direction && input > 0)
        {
            Jump();
            ETC();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isActivated = false;
        }
    }
}
