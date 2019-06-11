﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : InteractiveObject
{
    [SerializeField, Range(-1, 1)] private int rotateDirection; // 좌우 회전 방향 결정 변수

    protected override void Init()
    {
        if(rotateDirection == 0)
        {
            rotateDirection = 1;
        }
    }

    protected override void update()
    {
        Creater.Instance.player.SetJump(true);
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
