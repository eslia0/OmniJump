using UnityEngine;
using UnityEngine.EventSystems;

public class FixedJoystick : Joystick
{
    Vector2 joystickPosition = Vector2.zero;
    private Camera cam = new Camera();
    private bool isClick = false;

    [Header("Face Sprite")]
    public Transform face;

    void Start()
    {
        joystickPosition = RectTransformUtility.WorldToScreenPoint(cam, background.position);
    }

    private void FixedUpdate()
    {
        if(!EndlessManager.Instance.player.KeyBoardControll)
            EndlessManager.Instance.player.onClick = isClick;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        isClick = true;

        Vector2 direction = eventData.position - joystickPosition;
        inputVector = (direction.magnitude > background.sizeDelta.x / 2f) ? direction.normalized : direction / (background.sizeDelta.x / 2f);
        ClampJoystick();
        handle.anchoredPosition = (inputVector * background.sizeDelta.x / 2f) * handleLimit;

        float angle = ((Mathf.Atan2(handle.anchoredPosition.x, handle.anchoredPosition.y) * Mathf.Rad2Deg) + 315) % 360;

        if (angle < 90) // 우
        {
            EndlessManager.Instance.player.faceDirection = 0;
            face.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if(angle < 180) // 하
        {
            EndlessManager.Instance.player.faceDirection = 3;
            face.localEulerAngles = new Vector3(0, 0, 270);
        }
        else if(angle < 270) // 좌
        {
            EndlessManager.Instance.player.faceDirection = 2;
            face.localEulerAngles = new Vector3(0, 0, 180);
        }
        else // 상
        {
            EndlessManager.Instance.player.faceDirection = 1;
            face.localEulerAngles = new Vector3(0, 0, 90);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        isClick = true;
        EndlessManager.Instance.player.KeyBoardControll = false;
        OnDrag(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        isClick = false;
        handle.anchoredPosition = inputVector = Vector2.zero;
    }
}