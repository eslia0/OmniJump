using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStickImageCtrl : MonoBehaviour
{
    private FixedJoystick joystick;
    private PlayerUIController player;

    public bool press;
    public Direction dir;

    public void Start()
    {
        joystick = FindObjectOfType<FixedJoystick>();
        player = FindObjectOfType<PlayerUIController>();

        dir = transform.parent.GetComponent<InteractionUI>().dir;
    }

    public void SetJoyStickImage()
    {
        if (press)
        {
            if (dir == Direction.right)
            {
                joystick.StartCoroutine(joystick.SetImage(joystick.AOArrow2Press[1]));
            }
            else if (dir == Direction.down)
            {
                joystick.StartCoroutine(joystick.SetImage(joystick.AOArrow2Press[2]));
            }
            else if (dir == Direction.left)
            {
                joystick.StartCoroutine(joystick.SetImage(joystick.AOArrow2Press[3]));
            }
            else if (dir == Direction.up)
            {
                joystick.StartCoroutine(joystick.SetImage(joystick.AOArrow2Press[0]));
            }
        }

        enabled = false;
    }
}
