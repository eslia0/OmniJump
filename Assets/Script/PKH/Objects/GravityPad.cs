using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPad : InteractiveObject
{
    protected override void Init()
    {

    }

    protected override void update()
    {
        playerIsOn = false;
        // 중력패드 사용 직후 3의 속도를 추가한 만큼 y축 이동을 시킨다.
        Creater.Instance.player.velocity.y += (Creater.Instance.player.revertGravity) ? -3 : 3;
        Creater.Instance.player.revertGravity = !Creater.Instance.player.revertGravity;
        Creater.Instance.player.skinManager.FlipEffect(Creater.Instance.player.revertGravity);
        Creater.Instance.AddScore(15);
        
        base.update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerIsOn = true;
            Creater.Instance.player.onClick = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 플레이어 태그, 플레이어와 필요 방향, 플레이어의 접촉 여부, 사용 가능 횟수 확인
        if (collision.tag == "Player" && FaceCompare() &&
            Creater.Instance.player.onClick && playerIsOn && actionCount > 0)
        {
            update();
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
