﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinButtonScript : MonoBehaviour
{
    [SerializeField] private Image playerBody;

    private void Start()
    {
        playerBody.sprite = SceneManagement.Instance.GetPlayerBody();
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        playerBody.sprite = SceneManagement.Instance.GetPlayerBody();
    }
}
