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
    [SerializeField] private GameObject path;
    private EdgeCollider2D collider2D;
    [SerializeField] private Material mat;
    private float lifeTime = 0;


    private void Awake()
    {
        collider2D = GetComponent<EdgeCollider2D>();
        collider2D.enabled = body.enabled = false;

        mat = path.GetComponent<MeshRenderer>().material;
        mat.mainTextureScale = new Vector2(1, (distance / 2) * 10);

        path.transform.localScale = new Vector3(0.25f, distance, 1);
        path.transform.localPosition = new Vector3(0, (distance / 2), 1);

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
    
    public Missile Lunch()
    {
        collider2D.enabled = body.enabled = true;
        path.SetActive(false);
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
            Creater.Instance.GetMissilePopPrefab(transform);
            Destroy(gameObject);
        }
    }

    public void OffsetUp()
    {
        if (mat)
        {
            mat.mainTextureOffset -= new Vector2(0, 3 * Time.deltaTime);
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
            if (Creater.Instance.player.interactionDirection != direction)
            {
                collision.GetComponent<PlayerController>().Dead();
            }

            Creater.Instance.AddScore(SceneManagement.Instance.GetObjectData(ObjectType.Missile).currentScore);
            Creater.Instance.GetMissilePopPrefab(transform);

            Destroy(gameObject);
        }
    }
}
