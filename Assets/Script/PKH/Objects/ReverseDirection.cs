using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseDirection : InteractiveObject
{
    [SerializeField] private bool moveRight; // 플레이어와 상호작용시 플레이어 방향 변경 여부

    protected override void Init()
    {

    }

    protected override void update()
    {
        Creater.Instance.AddScore(15);
        Creater.Instance.player.moveRight = moveRight;

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
