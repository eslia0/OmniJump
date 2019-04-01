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

    private float SpacingX;
    private float SpacingY;
    private Vector2 velocity = new Vector2(0, 0);
    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        screenSize = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)) - Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        SpacingX = screenSize.x * 0.2f;
        SpacingY = screenSize.y * 0.3f;
    }
	
	void LateUpdate ()
    {
        //float posX = Mathf.SmoothDamp(transform.position.x, player.position.x + SpacingX, ref velocity.x, smoothTimeX);
        float posX = Mathf.Clamp(transform.position.x, player.position.x + SpacingX, player.position.x + SpacingX * 1.5f);

        yPos = Mathf.Clamp(transform.position.y, player.position.y - SpacingY, player.position.y - SpacingY * 0.3f);

        //if (player.position.y > yPos + maxY)
        //{
        //    Camera.main.orthographicSize
        //        = Mathf.Lerp(Camera.main.orthographicSize, normal + player.position.y, Time.deltaTime * smoothZoomOut);
        //}
        //else
        //{
        //    Camera.main.orthographicSize
        //        = Mathf.Lerp(Camera.main.orthographicSize, normal, Time.deltaTime * smoothZoomIn);
        //}

        transform.position = new Vector3(posX, yPos, -100);
    }
}
