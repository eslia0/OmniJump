﻿using UnityEngine;
using UnityEngine.Tilemaps;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Creater.Instance.player.Dead();
        }
    }
}
