using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : InteractiveObject
{
    [SerializeField, Range(-1, 1)] private int rotateDirection; // 좌우 회전 방향 결정 변수
    [SerializeField] bool canJump = false;

    [Header("동작 가능 횟수")]
    [SerializeField] private Transform nextTarget;
    [SerializeField] private float jumpDuration = 1;
    private Transform jumpHeight;         // 텔레포터 출구 위치

    protected override void Init()
    {
        if(rotateDirection == 0)
        {
            rotateDirection = 1;
        }


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

    protected override void update()
    {
        Creater.Instance.AddScore(30);
        if (canJump)
        {
            if (nextTarget == null)
            {
                Creater.Instance.player.SetJump(true);
            }
            else
            {
                Creater.Instance.player.jumpFun.SetJump(nextTarget.position, jumpHeight.position, jumpDuration);
            }
        }
        Creater.Instance.player.rotationZ = rotateDirection;

        base.update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && FaceCompare() && actionCount > 0)
        {
            update();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 플레이어 태그, 플레이어와 필요 방향, 플레이어의 접촉 여부, 사용 가능 횟수 확인
        if (collision.tag == "Player" && FaceCompare() && actionCount > 0)
        {
            update();
        }
    }
}
