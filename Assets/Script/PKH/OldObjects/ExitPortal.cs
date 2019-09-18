using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPortal : MonoBehaviour
{
    private Vector3 endPoint;
    private Transform player;
    private CameraFollow follow;
    [SerializeField] private Vector2 cameraStopArea = new Vector2(0, 0);
    [SerializeField] private float distance = 0f;
    
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
        Vector2 pos = (Vector2)transform.position + cameraStopArea;

        while (true)
        {
            if (Vector2.Distance(player.position, pos) <= distance)
            {
                follow.follow = false;
            }

            if (Vector3.Distance(endPoint, player.position) <= 0.32f)
            {
                yield return new WaitForSeconds(0.15f);

                player.GetComponent<Animator>().enabled = true;

                player.position = transform.position + Vector3.up * 0.64f * (Creater.Instance.player.revertGravity ? 1 : -1);
                player.GetComponent<Animator>().SetBool("upsidedown", Creater.Instance.player.revertGravity);
                player.GetComponent<Animator>().SetTrigger("Exit");

                break;
            }

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(2.0f);

        if (SceneManagement.Instance.currentScene == "EndlessScene")
        {
            Creater.Instance.NextStage(1);
        }
        else if(SceneManagement.Instance.currentScene == "TutorialScene")
        {
            SceneManagement.Instance.LoadScene("EndlessScene");
        }
    }

    private void OnDrawGizmos()
    {
        if (distance != 0)
        {
            Vector2 pos = (Vector2) transform.position + cameraStopArea;

            Gizmos.color = Color.white;
            Gizmos.DrawLine(pos + new Vector2(1, 1) * distance, pos + new Vector2(1, -1) * distance);
            Gizmos.DrawLine(pos + new Vector2(1, -1) * distance, pos + new Vector2(-1, -1) * distance);
            Gizmos.DrawLine(pos + new Vector2(-1, -1) * distance, pos + new Vector2(-1, 1) * distance);
            Gizmos.DrawLine(pos + new Vector2(-1, 1) * distance, pos + new Vector2(1, 1) * distance);
        }
    }
}
