﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FixedJoystick : Joystick
{
    Vector2 joystickPosition = Vector2.zero;
    private Camera cam = new Camera();
    private bool isClick = false;
    private Image image = null;

    [Header("Face Sprite")]
    public Transform face;

    [Header("상하좌우 이미지 (0(상), 1(우), 2(하), 3(좌))")]
    public Sprite[] AOArrow2 = new Sprite[4]; // 0(상), 1(우), 2(하), 3(좌)

    void Start()
    {
        image = GetComponent<Image>();
        joystickPosition = RectTransformUtility.WorldToScreenPoint(cam, background.position);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (!Creater.Instance.player.KeyBoardControll)
            Creater.Instance.player.onClick = true;

        Vector2 direction = eventData.position - joystickPosition;
        inputVector = (direction.magnitude > background.sizeDelta.x / 2f) ? direction.normalized : direction / (background.sizeDelta.x / 2f);
        ClampJoystick();
        handle.anchoredPosition = (inputVector * background.sizeDelta.x / 2f) * handleLimit;

        float angle = ((Mathf.Atan2(handle.anchoredPosition.x, handle.anchoredPosition.y) * Mathf.Rad2Deg) + 315) % 360;

        if (angle < 90) // 우
        {
            Creater.Instance.player.faceDirection = 0;
            face.localEulerAngles = new Vector3(0, 0, 0);
            image.sprite = AOArrow2[1];
        }
        else if(angle < 180) // 하
        {
            Creater.Instance.player.faceDirection = 3;
            face.localEulerAngles = new Vector3(0, 0, 270);
            image.sprite = AOArrow2[2];
        }
        else if(angle < 270) // 좌
        {
            Creater.Instance.player.faceDirection = 2;
            face.localEulerAngles = new Vector3(0, 0, 180);
            image.sprite = AOArrow2[3];
        }
        else // 상
        {
            Creater.Instance.player.faceDirection = 1;
            face.localEulerAngles = new Vector3(0, 0, 90);
            image.sprite = AOArrow2[0];
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!Creater.Instance.player.KeyBoardControll)
            Creater.Instance.player.onClick = true;
        Creater.Instance.player.KeyBoardControll = false;
        OnDrag(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!Creater.Instance.player.KeyBoardControll)
            Creater.Instance.player.onClick = false;
        handle.anchoredPosition = inputVector = Vector2.zero;
    }
}