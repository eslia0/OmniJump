 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [HideInInspector] public Direction direction;
    [HideInInspector] public int angle;
    public bool isChecked = false;
    public float distance;
    public float time;
    public float lunchDelay;
    
    [SerializeField] private ParticleSystem arrow;
    [SerializeField] private SpriteRenderer body;
    [SerializeField] private EdgeCollider2D collider2D;
    private LineRenderer lr;
    private GameObject clone;
    private float lifeTime = 0;


    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.SetPosition(1, new Vector3(0, distance, 0));

        collider2D = GetComponent<EdgeCollider2D>();

        angle = ((int)transform.localEulerAngles.z + 360) % 360;
        arrow.startRotation = (360 - angle) * Mathf.Deg2Rad;
        switch (angle)
        {
            case 0:
            case 360:
                direction = Direction.down;
                break;
            case 90:
                direction = Direction.right;
                break;
            case 180:
                direction = Direction.up;
                break;
            case 270:
                direction = Direction.left;
                break;
        }

        collider2D.enabled = body.enabled = false;
    }
    
    public Missile Lunch()
    {
        // isActive = body.enabled = true;
        collider2D.enabled = body.enabled = true;
        lr.enabled = false;
        arrow.gameObject.SetActive(false);

        return this;
    }

    // Update is called once per frame
    public void Updating()
    {
        if (time > lifeTime)
        {
            lifeTime += Time.deltaTime;
            transform.position += transform.up * ((distance / time) * Time.deltaTime);
        }
        else
        {
            EndlessManager.Instance.GetMissilePopPrefab(transform);
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (transform.up * distance));
    }

    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (EndlessManager.Instance.player.interactionDirection != direction)
            {
                collision.GetComponent<PlayerController>().Dead();
            }

            EndlessManager.Instance.AddScore(15);
            EndlessManager.Instance.GetMissilePopPrefab(transform);

            Destroy(gameObject);
        }
    }
}
