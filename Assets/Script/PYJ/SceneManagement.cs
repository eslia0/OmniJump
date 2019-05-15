﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    private static SceneManagement instance;
    public static SceneManagement Instance {
        get {
            if (instance == null)
            {
                GameObject managment = new GameObject("SceneManagement");
                instance = managment.AddComponent<SceneManagement>();
            }

            return instance;
        }
    }

    Creater creater;
    [SerializeField] bool isTesting;

    void Awake()
    {
        if (isTesting)
        {
            creater = Creater.Instance;
            creater.TestInit();
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadScene(string name)
    {
        if (name == "TitleScene")
        {
            creater = null;
        }
        else if (name == "PYJTestScene")
        {
            if (creater == null)
            {
                creater = Creater.Instance;
                creater.Init();
            }
        }

        SceneManager.LoadScene(name);
    }
}