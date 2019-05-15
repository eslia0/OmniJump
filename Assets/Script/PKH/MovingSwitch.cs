using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSwitch : MonoBehaviour
{
    public PlatformMode mode;

    public Direction direction;
    public MovingPlatform platform;

    // Start is called before the first frame update
    void Start()
    {
        if (mode == PlatformMode.Trigger)
        {
            platform.isActive = false;
            if (GetComponent<ParticleSystem>())
                GetComponent<ParticleSystem>().Stop();
            StartCoroutine(TriggerUpdate());
        }
        else if(mode == PlatformMode.Passive)
        {
            platform.enabled = true;
            if (GetComponent<ParticleSystem>())
                GetComponent<ParticleSystem>().Stop();
            Destroy(gameObject);
        }
        else if(mode == PlatformMode.Distance)
        {
            platform.isActive = false;
            if (GetComponent<ParticleSystem>())
                GetComponent<ParticleSystem>().Stop();
            StartCoroutine(DistanceUpdate());
        }
        else
        {
            if (GetComponent<ParticleSystem>())
                switch (direction)
                {
                    case Direction.right:
                        GetComponent<ParticleSystem>().startRotation = 90 * Mathf.Deg2Rad;
                        break;
                    case Direction.left:
                        GetComponent<ParticleSystem>().startRotation = 270 * Mathf.Deg2Rad;
                        break;
                    case Direction.down:
                        GetComponent<ParticleSystem>().startRotation = 180 * Mathf.Deg2Rad;
                        break;
                }

            if (mode == PlatformMode.Active)
            {
                platform.isActive = false;
                platform.enabled = false;
                StartCoroutine(ActiveUpdate());
            }
        }
    }

    IEnumerator ActiveUpdate()
    {
        Collider2D check;

        while (true)
        {
            check = Physics2D.OverlapBox(transform.position, new Vector2(0.32f, 0.32f), 0f, EndlessManager.Instance.playerLayer);
            
            if (check 
                && EndlessManager.Instance.player.interactionDirection == direction
                && EndlessManager.Instance.player.onClick)
            {
                platform.enabled = true;
                platform.isActive = true;
                EndlessManager.Instance.GetTriggerBlowParticles(direction, transform);
                if (platform.movePassinger)
                {
                    EndlessManager.Instance.player.moveSpeed = 0;
                    EndlessManager.Instance.player.transform.parent = platform.transform;
                }
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
        yield return null;
    }

    IEnumerator TriggerUpdate()
    {
        while (true)
        {
            if (EndlessManager.Instance.player.transform.position.x > transform.position.x)
            {
                platform.enabled = true;
                platform.isActive = true;
                if (platform.movePassinger)
                {
                    EndlessManager.Instance.player.moveSpeed = 0;
                    EndlessManager.Instance.player.transform.parent = platform.transform;
                }
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
        yield return null;
    }

    IEnumerator DistanceUpdate()
    {
        while (true)
        {
            if (Vector3.Distance(EndlessManager.Instance.player.transform.position, transform.position) < 0.16f)
            {
                platform.enabled = true;
                platform.isActive = true;
                if (platform.movePassinger)
                {
                    EndlessManager.Instance.player.moveSpeed = 0;
                    EndlessManager.Instance.player.transform.parent = platform.transform;
                }
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
        yield return null;
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.32f, 0.32f, 1));
    }
    */
}
