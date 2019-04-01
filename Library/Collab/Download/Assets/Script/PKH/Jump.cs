using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    private float movePeriod = 1.0f;

    private Transform target;
    private Transform height;

    private bool isJump = false;
    private bool isCheck = false;
    
    private float upSpeed;
    private float moveSpeed;
    private float startTime;
    private float gravity;


    public void SetJump(Vector2 target, Vector2 height, float time)
    {
        movePeriod = time;

        float h1 = 0;
        float h2 = 0;
        // 각자의 높이
        if (!Creater.Instance.player.revertGravity)
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

        upSpeed = (((1.0f + Mathf.Sqrt(h2 / h1)) * 2.0f) * h1) * ((Creater.Instance.player.revertGravity) ? -1 : 1);
        gravity = ((Mathf.Pow(upSpeed, 2) / -2.0f) / h1) * ((Creater.Instance.player.revertGravity) ? -1 : 1);

        startTime = 0.0f;

        Creater.Instance.player.SetJump(false);
    }

    public void JumpMove()
    {
        Debug.Log("jump Move");
        Creater.Instance.player.velocity = Vector3.zero;

        transform.Translate(new Vector3(moveSpeed, upSpeed) * Time.deltaTime / movePeriod);
        
        upSpeed += gravity * Time.deltaTime / movePeriod;

        startTime += Time.deltaTime;

        if (startTime >= movePeriod) // 일반 점프가 되지 않을때, 지면과 충돌치 않을때 발동
        {
            Creater.Instance.player.isTargetJump = false;
            Creater.Instance.player.movementController -= Creater.Instance.player.jumpFun.JumpMove;
        }
    }
}
