using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCircle : InteractiveObject
{
    [Header("랜덤 방향")]
    [SerializeField] private bool randomDirection;              // 4방향 회전이 가능한 파티클 및 방향의 랜덤성 부여 여부

    [Header("동작 가능 횟수")]
    [SerializeField] private Transform nextTarget;
    [SerializeField] private float jumpDuration = 1;

    [Header("회전이 필요한 파티클")]
    [SerializeField] private ParticleSystem[] rotationParticle;

    private Transform jumpHeight;         // 텔레포터 출구 위치

    protected override void Init()
    {
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

        // 방향을 랜덤하게 생성할때 호출
        if (randomDirection)
        {
            direction = Creater.Instance.randomizer.RandomizeDirection();
        }

        if (rotationParticle != null)
        {
            /// 현재 오브젝트의 회전 값을 통해 해당 리스트의 오브젝트 또한 회전시키는 함수 호출
            Creater.Instance.particleRotation.SetParticlesFourWayDirection(direction, rotationParticle);
        }
    }

    protected override void update()
    {
        Creater.Instance.AddScore((int)(10 * (1 + SceneManagement.Instance.GetObjectScoreLevel(ObjectScore.CirclePad) * 0.1f)));
        if (nextTarget == null)
        {
            Creater.Instance.player.SetJump(true);
        }
        else
        {
            Creater.Instance.player.jumpFun.SetJump(nextTarget.position, jumpHeight.position, jumpDuration);
        }

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
