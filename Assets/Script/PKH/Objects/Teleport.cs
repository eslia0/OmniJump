using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : InteractiveObject
{
    [SerializeField] private bool moveRight;                 // 플레이어와 상호작용시 플레이어 방향 변경 여부
    [SerializeField] private Transform teleportExit;            // 텔레포터 출구 위치
    [SerializeField] private float waitSecond = 0.8f;

    [Header("회전이 필요한 파티클")]
    [SerializeField] private ParticleSystem[] portalLight;


    protected override void Init()
    {
        Creater.Instance.particleRotation.SetParticlesRotation(transform.eulerAngles.z, portalLight);
    }

    protected override void update()
    {
        Creater.Instance.player.onClick = false;
        playerIsOn = false;

        // 텔레포트 이동 파티클 생성
        Creater.Instance.GetPoofPrefab(transform);

        Creater.Instance.player.moveRight = moveRight; // 텔로포터 사용 후 플레이어 방향 설정

        Creater.Instance.player.StartCoroutine(Creater.Instance.player.HoldPlayer(teleportExit, waitSecond)); // 텔레포터 딜레이

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
    private void OnDrawGizmos()
    {
        if (teleportExit)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, teleportExit.position);
        }
    }
}
