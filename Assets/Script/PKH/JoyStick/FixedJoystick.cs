using UnityEngine;
using UnityEngine.EventSystems;

public class FixedJoystick : Joystick
{
    Vector2 joystickPosition = Vector2.zero;
    private Camera cam = new Camera();
    private PlayerController player;

    [Header("Face Sprite")]
    public Transform face;

    private bool press = false;

    void Start()
    {
        joystickPosition = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        player.onClick = press;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - joystickPosition;
        inputVector = (direction.magnitude > background.sizeDelta.x / 2f) ? direction.normalized : direction / (background.sizeDelta.x / 2f);
        ClampJoystick();
        handle.anchoredPosition = (inputVector * background.sizeDelta.x / 2f) * handleLimit;

        float angle = ((Mathf.Atan2(handle.anchoredPosition.x, handle.anchoredPosition.y) * Mathf.Rad2Deg) + 315) % 360;

        if (angle < 90) // 우
        {
            player.faceDirection = 0;
            face.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if(angle < 180) // 하
        {
            player.faceDirection = 3;
            face.localEulerAngles = new Vector3(0, 0, 270);
        }
        else if(angle < 270) // 좌
        {
            player.faceDirection = 2;
            face.localEulerAngles = new Vector3(0, 0, 180);
        }
        else // 상
        {
            player.faceDirection = 1;
            face.localEulerAngles = new Vector3(0, 0, 90);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        press = true;
        OnDrag(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        press = false;
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
}