using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : InteractiveObject
{
    private bool playerHold = false;
    private bool playerRelease = false;
    private uint realCount;

    private float t = 0;
    private float timeToReachTarget = 0.5f;
    private Vector2 playerV2;

    [Space(5)]
    [Header("랜덤 방향")]
    [SerializeField] private bool randomDirection; // 4방향 회전이 가능한 파티클 및 방향의 랜덤성 부여 여부

    [Space(5)]
    [Header("회전이 필요한 파티클")]
    [SerializeField] private ParticleSystem[] rotationParticle;

    [Space(5)]
    [Header("랜덤 방향")]
    [SerializeField] private float speed = 3;


    protected override void Init()
    {
        realCount = actionCount * 2;

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
        Creater.Instance.player.onClick = false; // 플레이어 클릭 초기화
        playerHold = !playerHold;
        realCount--;

        if (playerHold)
        {
            t = 0;
            playerV2 = Creater.Instance.player.transform.position;

            Creater.Instance.player.moveSpeed = 0; // 플레이어 속도 제거
            Creater.Instance.player.velocity = Vector2.zero; // 플레이어 속력 제거
            Creater.Instance.AddScore(20);
        } else
        {
            Creater.Instance.player.moveSpeed = speed; // 플레이어 속도 제거
        }

        StartCoroutine(PlayerHolding());

        if ((realCount % 2) == 0)
        {
            direction = Creater.Instance.randomizer.RandomizeDirection();
            Creater.Instance.particleRotation.SetParticlesFourWayDirection(direction, rotationParticle);
            base.update();
        }
    }

    private IEnumerator PlayerHolding()
    {
        while (playerHold)
        {
            if (Vector2.Distance(transform.position, Creater.Instance.player.transform.position) > 0.01f)
            {
                t += Time.deltaTime / timeToReachTarget;
                Creater.Instance.player.transform.position = Vector2.Lerp(playerV2, transform.position, t);
            }
            else
            {
                Creater.Instance.player.transform.position = transform.position;
                playerRelease = true;
            }

            if (playerRelease &&
                FaceCompare() &&
                actionCount > 0 &&
                Creater.Instance.player.onClick)
            {
                playerRelease = playerIsOn = false;
                Creater.Instance.player.onClick = false;
                update();
            }

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerIsOn = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 플레이어 태그, 플레이어와 필요 방향, 플레이어의 접촉 여부, 사용 가능 횟수 확인
        if (playerIsOn &&
            collision.tag == "Player" &&
            FaceCompare() &&
            actionCount > 0 &&
            realCount > 0 &&
            !playerHold)
        {
            playerIsOn = true;
            Creater.Instance.player.onClick = false;
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
