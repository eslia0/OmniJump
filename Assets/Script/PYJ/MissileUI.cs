using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileUI : MonoBehaviour
{
    [HideInInspector] public Direction direction;
    [HideInInspector] public int angle;
    public float distance;
    public float time;
    public float lunchDelay;

    [SerializeField] private ParticleSystem arrow;
    private LineRenderer lr;
    private float lifeTime = 0;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.SetPosition(1, new Vector3(0, distance, 0));

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
    }

    // Update is called once per frame
    public IEnumerator Lunch()
    {
        lr.enabled = false;
        arrow.gameObject.SetActive(false);
        while (time > lifeTime)
        {
            lifeTime += Time.deltaTime;
            transform.position += transform.up * ((distance / time) * Time.deltaTime);
            yield return null;
        }

        Destroy(Instantiate(Resources.Load<GameObject>("Effects/GlowExplosion 1"), transform.position, Quaternion.identity), 1.0f);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (transform.up * distance));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Destroy(Instantiate(Resources.Load<GameObject>("Effects/GlowExplosion 1"), transform.position, Quaternion.identity), 1.0f);
            Destroy(gameObject);
        }
    }
}
