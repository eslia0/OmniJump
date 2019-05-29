using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPortal : MonoBehaviour
{
    private Vector3 endPoint;
    private Transform player;
    private CameraFollow follow;
    
    public void Init()
    {
        follow = CameraFollow.mainCam.GetComponent<CameraFollow>();
        player = Creater.Instance.player.transform;
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
            if (endPoint.x < player.transform.position.x + follow.screenSize.x * 0.7f - 0.32f
                && Mathf.Abs(transform.position.y - player.position.y) <= 0.96f)
            {
                follow.follow = false;
            }

            if (Vector3.Distance(transform.position, player.position) <= 0.65f)
            {
                player.GetComponent<Animator>().enabled = true;

                if (Creater.Instance.player.revertGravity)
                {
                    player.position = transform.position + Vector3.up * 0.64f;
                    player.GetComponent<Animator>().SetBool("upsidedown", true);
                }
                else
                {
                    player.position = transform.position - Vector3.up * 0.64f;
                    player.GetComponent<Animator>().SetBool("upsidedown", false);
                }
                player.GetComponent<Animator>().SetTrigger("Exit");

                break;
            }

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(2.0f);

        if (SceneManagement.Instance.currentScene == "StageScene")
        {
            CameraFollow.mainCam.GetComponentInChildren<StageButtonInput>().SetResultPanel();
        }
        else if (SceneManagement.Instance.currentScene == "PYJTestScene")
        {
            Creater.Instance.NextStage(1);
        }
    }
}
