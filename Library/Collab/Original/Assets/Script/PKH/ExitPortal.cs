using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPortal : MonoBehaviour
{
    private Vector3 endPoint;
    private Vector3 exitPosition;
    private Transform player;
    private Camera cam;
    private CameraFollow follow;

    private void Start()
    {
        cam = Camera.main;
        follow = cam.GetComponent<CameraFollow>();
        player = GameObject.FindWithTag("Player").transform;
        exitPosition = transform.position;
        endPoint = Creater.Instance.NowPlatform.endPoint.position;

        StartCoroutine(Check());
    }

    private IEnumerator Check()
    {
        while (true)
        {
            if (endPoint.x < cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x - 0.16f)
            {
                follow.enabled = false;
            }

            if (exitPosition.x <= player.position.x)
            {
                player.GetComponent<Animator>().Play("EndGame");
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(2.0f);

        Creater.Instance.NextStage(1);
    }
}
