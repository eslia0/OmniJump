using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;

public class FixedJoystick : Joystick
{
    Vector2 joystickPosition = Vector2.zero;
    private Camera cam = new Camera();
    private bool isClick = false;
    private Image image = null;
    private Coroutine coroutine;

    [Header("상하좌우 이미지 (0(상), 1(우), 2(하), 3(좌))")]
    public Sprite AOArrow2 = null;
    public Sprite[] AOArrow2Press = new Sprite[4]; // 0(상), 1(우), 2(하), 3(좌)
    private int selectedFace = 1;

    void Start()
    {
        image = GetComponent<Image>();
        joystickPosition = RectTransformUtility.WorldToScreenPoint(cam, background.position);
    }
    
    IEnumerator onPress()
    {
        while (true)
        {
            Creater.Instance.player.onClick = true;
            yield return new WaitForFixedUpdate();
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
#if UNITY_EDITOR
        if (!Creater.Instance.player.KeyBoardControll)
            Creater.Instance.player.onClick = true;
#endif

        Vector2 direction = eventData.position - joystickPosition;
        inputVector = (direction.magnitude > background.sizeDelta.x / 2f) ? direction.normalized : direction / (background.sizeDelta.x / 2f);
        ClampJoystick();
        Vector2 pos = (inputVector * background.sizeDelta.x / 2f) * handleLimit;

        float angle = ((Mathf.Atan2(pos.x, pos.y) * Mathf.Rad2Deg) + 315) % 360;

        if (angle < 90) // 우
        {
            selectedFace = 1;
            Creater.Instance.player.faceDirection = 0;
        }
        else if(angle < 180) // 하
        {
            selectedFace = 2;
            Creater.Instance.player.faceDirection = 3;
        }
        else if(angle < 270) // 좌
        {
            selectedFace = 3;
            Creater.Instance.player.faceDirection = 2;
        }
        else // 상
        {
            selectedFace = 0;
            Creater.Instance.player.faceDirection = 1;
        }

        image.sprite = AOArrow2Press[selectedFace];
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
#if UNITY_EDITOR
        Creater.Instance.player.KeyBoardControll = false;
#endif

        coroutine = StartCoroutine(onPress());
        OnDrag(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
#if UNITY_EDITOR
        //if (!Creater.Instance.player.KeyBoardControll)
#endif

        StopCoroutine(coroutine);
        image.sprite = AOArrow2;
        Creater.Instance.player.onClick = false;
    }
}