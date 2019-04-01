using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSwitch : MonoBehaviour
{
    public enum PlatformMode
    {
        Active,
        Passive,
        Trigger
    }
    public PlatformMode mode;

    [SerializeField] private Direction direction;
    [SerializeField] private MovingPlatform platform;

    // Start is called before the first frame update
    void Start()
    {
        if (mode == PlatformMode.Active)
        {
            platform.isActive = false;
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
            StartCoroutine(ActiveUpdate());
        }
        else if(mode == PlatformMode.Trigger)
        {
            platform.isActive = false;
            GetComponent<ParticleSystem>().Stop();
            StartCoroutine(TriggerUpdate());
        }
        else if(mode == PlatformMode.Passive)
        {
            GetComponent<ParticleSystem>().Stop();
            Destroy(gameObject);
        }
    }

    IEnumerator ActiveUpdate()
    {
        Collider2D check;
        PlayerController player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        while (true)
        {
            check = Physics2D.OverlapBox(transform.position, new Vector2(0.32f, 0.32f), 0f, platform.passengerMask);
            
            if (check && check.GetComponent<PlayerController>().interactionDirection == direction && player.onClick)
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
        PlayerController player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        while (true)
        {
            //if (Vector3.Distance(player.transform.position, transform.position) <= 1)
            //{
            //    platform.isActive = true;
            //    break;
            //}

            if (player.transform.position.x > transform.position.x)
            {
                platform.isActive = true;
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
        yield return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.32f, 0.32f, 1));
    }
}
