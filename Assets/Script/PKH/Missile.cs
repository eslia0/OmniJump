 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [HideInInspector] public Direction direction;
    [HideInInspector] public int angle;
    public bool isActive = false;
    public bool isChecked = false;
    public float distance;
    public float time;
    public float lunchDelay;
    public GameObject appearance;
    public GameObject explosion;
    public GameObject missileWarning;

    private GameObject clone;
    private ParticleSystem arrow;
    private SpriteRenderer body;
    private PlayerController player;
    private Transform playerTran;
    private float lifeTime = 0;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        body = transform.GetChild(0).GetComponent<SpriteRenderer>();
        arrow = transform.GetChild(1).GetComponent<ParticleSystem>();

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

        SetState(false);
    }

    public void SetState(bool isLunching)
    {
        isActive = body.enabled = isLunching;
        if(isLunching) arrow.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            if (!isChecked)
            {
                if (direction == Direction.right && player.moveRight)
                {
                    float xPos = CameraFollow.mainCam.GetComponent<CameraFollow>().screenSize.x + player.transform.position.x;

                    if (transform.position.x <= xPos)
                    {
                        float cloneX = CameraFollow.mainCam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;

                        clone = Instantiate(missileWarning, CameraFollow.mainCam.transform);
                        clone.transform.position = new Vector2(cloneX, transform.position.y);
                        Destroy(clone, 1.0f);
                        isChecked = true;
                    }
                }
                else if (direction == Direction.left && !player.moveRight)
                {
                    float xPos = player.transform.position.x - CameraFollow.mainCam.GetComponent<CameraFollow>().screenSize.x;

                    if (transform.position.x >= xPos)
                    {
                        float cloneX = CameraFollow.mainCam.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;

                        clone = Instantiate(missileWarning, CameraFollow.mainCam.transform);
                        clone.transform.position = new Vector2(cloneX, transform.position.y);
                        clone.transform.rotation = Quaternion.Euler(0, 0, 90);
                        Destroy(clone, 1.0f);
                        isChecked = true;
                    }
                }
            }

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
            if (player.interactionDirection != direction && isActive)
            {
                collision.GetComponent<PlayerController>().Dead();
            }

            Creater.Instance.AddScore(15);
            Creater.Instance.GetMissilePopPrefab(transform);

            Destroy(gameObject);
        }
    }
}
