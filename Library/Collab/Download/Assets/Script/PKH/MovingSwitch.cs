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
            if(GetComponent<ParticleSystem>())
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
                StartCoroutine(ActiveUpdate());
            }
        }
    }

    IEnumerator ActiveUpdate()
    {
        Collider2D check;

        while (true)
        {
            check = Physics2D.OverlapBox(transform.position, new Vector2(0.32f, 0.32f), 0f, Creater.Instance.playerLayer);
            
            if (check && Creater.Instance.player.interactionDirection == direction
                && Creater.Instance.player.onClick)
            {
                platform.isActive = true;
                Creater.Instance.GetTriggerBlowParticles(direction, transform);
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
            if (Creater.Instance.player.transform.position.x > transform.position.x)
            {
                platform.isActive = true;
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
            if (Vector3.Distance(Creater.Instance.player.transform.position, transform.position) < 0.32f)
            {
                platform.isActive = true;
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
