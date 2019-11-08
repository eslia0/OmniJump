using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    const int normal = 5;

    public float yPos;
    public float smoothTimeX;
    public float smoothTimeY;
    public float maxY;
    public float smoothZoomIn = 5;
    public float smoothZoomOut = 5;

    public Vector3 screenSize;

    public bool follow = true;
    private float SpacingX;
    private float SpacingY;
    private Vector2 velocity = new Vector2(0, 0);

    public static Camera mainCam;
    private BoxCollider2D b2d;

    void Awake()
    {
        mainCam = Camera.main;
        b2d = GetComponent<BoxCollider2D>();

        screenSize = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)) - mainCam.ScreenToWorldPoint(new Vector3(0, 0, 0));
        SpacingX = screenSize.x * 0.3f;
        SpacingY = screenSize.y * 0.3f;

        b2d.size = new Vector2(mainCam.orthographicSize + 1.5f,
            (mainCam.orthographicSize * 2) + 1.5f);
    }
	
	void LateUpdate ()
    {
        if (follow && Creater.Instance)
        {
            float xPos = 0;

            if (Creater.Instance.player.moveRight)
            {
                xPos = Mathf.Clamp(transform.position.x,
                    Creater.Instance.player.transform.position.x + (SpacingX * 0.6f),
                    Creater.Instance.player.transform.position.x + SpacingX);
            }
            else
            {
                xPos = Mathf.Clamp(transform.position.x,
                    Creater.Instance.player.transform.position.x - SpacingX,
                    Creater.Instance.player.transform.position.x - (SpacingX * 0.6f));
            }

            xPos = Mathf.SmoothDamp(transform.position.x, xPos, ref velocity.x, smoothTimeX);

            yPos = Mathf.Clamp(transform.position.y,
                Creater.Instance.player.transform.position.y - SpacingY * 0.6f,
                Creater.Instance.player.transform.position.y - SpacingY * 0.3f);
            yPos = Mathf.SmoothDamp(transform.position.y, yPos, ref velocity.y, smoothTimeY);

            transform.position = new Vector3(xPos, yPos, -100);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Pad")
        {
            InteractiveObject tmp = collision.GetComponent<InteractiveObject>();
            if (tmp != null)
                tmp.SetParticle(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Pad")
        {
            InteractiveObject tmp = collision.GetComponent<InteractiveObject>();
            if (tmp != null)
                tmp.SetParticle(false);
        }
    }
}
