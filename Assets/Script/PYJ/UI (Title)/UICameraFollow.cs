using UnityEngine;

public class UICameraFollow : MonoBehaviour
{
    private PlayerUIController player;
    private float yPos;
    public float smoothTimeX;
    public float smoothTimeY;
    public float smoothZoomIn = 5;
    public float smoothZoomOut = 5;

    public Vector3 screenSize;

    public bool follow = true;
    private float SpacingX;
    private float SpacingY;
    private Vector2 velocity = new Vector2(0, 0);

    public static Camera mainCam;

    void Awake()
    {
        player = FindObjectOfType<PlayerUIController>();
        mainCam = Camera.main;

        screenSize = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)) - mainCam.ScreenToWorldPoint(new Vector3(0, 0, 0));
        SpacingX = screenSize.x * 0.3f;
        SpacingY = -screenSize.y * 0.3f;
    }

    void LateUpdate()
    {
        if (follow)
        {
            float xPos = 0;

            if (player.moveRight)
            {
                xPos = Mathf.Clamp(transform.position.x,
                    player.transform.position.x + (SpacingX * 0.8f),
                    player.transform.position.x + SpacingX);
            }
            else
            {
                xPos = Mathf.Clamp(transform.position.x,
                    player.transform.position.x - SpacingX,
                    player.transform.position.x);
            }

            xPos = Mathf.SmoothDamp(transform.position.x, xPos, ref velocity.x, smoothTimeX);

            yPos = Mathf.Clamp(transform.position.y,
                player.transform.position.y - SpacingY * 0.6f,
                player.transform.position.y - SpacingY * 0.3f);
            yPos = Mathf.SmoothDamp(transform.position.y, yPos, ref velocity.y, smoothTimeY);

            transform.position = new Vector3(xPos, yPos, -100);
        }
    }
}
