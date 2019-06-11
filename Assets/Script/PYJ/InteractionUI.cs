using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    public enum UIInteraction
    {
        Move,
        Jump,
        Rotate,
        TargetJump,
        Missile,
        Reverse,
        Teleport,
        Gravity,
        Moving,
    }

    public UIInteraction action;
    public Direction dir;
    public GameObject usingEffect;
    public GameObject exit;
    public GameObject trigger;

    public void Lunch()
    {
        StartCoroutine(GetComponent<MissileUI>().Lunch());
    }
}
