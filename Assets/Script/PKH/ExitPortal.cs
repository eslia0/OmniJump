using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPortal : MonoBehaviour
{
    private Vector3 endPoint;
    private Transform player;
    private Camera cam;
    private CameraFollow follow;

    private void Start()
    {

    }

    public void Init()
    {
        cam = Camera.main;
        follow = cam.GetComponent<CameraFollow>();
        player = GameObject.FindWithTag("Player").transform;
        endPoint = Creater.Instance.NowPlatform.EndPoint.position;

        if (endPoint == Vector3.zero || endPoint == null)
        {
            endPoint = transform.GetComponentInParent<Transform>().position;
        }

        StartCoroutine(Check());
    }

    private IEnumerator Check()
    {
        while (true)
        {
            if (endPoint.x < player.transform.position.x + follow.screenSize.x
                && Mathf.Abs(transform.position.y - player.position.y) <= 0.96f)
            {
                Debug.Log(Mathf.Abs(transform.position.y - player.position.y));
                follow.follow = false;
            }

            if (transform.position.x <= player.position.x)
            {
                player.position = transform.position - Vector3.up * 0.64f;
                player.GetComponent<Animator>().enabled = true;
                player.GetComponent<Animator>().SetTrigger("Exit");
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(2.0f);

        Creater.Instance.NextStage(1);
    }
}
