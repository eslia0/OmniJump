using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    private static SceneManagement instance;
    public static SceneManagement Instance {
        get {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<SceneManagement>();

                if (instance == null)
                {
                    GameObject managment = new GameObject("SceneManagement");
                    instance = managment.AddComponent<SceneManagement>();
                }
            }

            return instance;
        }
    }

    private UnityAdsHelper adsHelper;
    private GooglePlayManager GPManager;

    [SerializeField] private bool isTesting;
    public int selectedStage;
    public string currentScene;
    public string prevScene;

    public bool[] clearStage;
    private int highScore;


    void Awake()
    {
        instance = FindObjectOfType<SceneManagement>();

        if (instance != this)
        {
            Destroy(gameObject);
        }

        clearStage = new bool[60];
        DontDestroyOnLoad(gameObject);
        LoadData();
    }

    public void LoadData()
    {
        string str = "";
        str = PlayerPrefs.GetString("ClearStage");

        if (str != "")
        {
            string[] stages = str.Split('.');
            for (int i = 0; i < stages.Length - 1; i++)
            {
                int num = int.Parse(stages[i]);
                clearStage[num] = true;
            }
        }

        //string path = Application.persistentDataPath + "/data0.txt";

        //if (File.Exists(path))
        //{
        //    string[] data = File.ReadAllLines(path);

        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        int num = int.Parse(data[i]);
        //        clearStage[num] = true;
        //    }
        //}
    }

    public void WriteData()
    {
        string num = "";

        for (int i = 0; i < clearStage.Length; i++)
        {
            if (clearStage[i])
            {
                num += i.ToString() + ".";
            }
        }
        
        PlayerPrefs.SetString("ClearStage", num);

        //string path = Application.persistentDataPath + "/data0.txt";
        //string num = "";

        //for (int i = 0; i < clearStage.Length; i++)
        //{
        //    if (clearStage[i])
        //    {
        //        num += i.ToString() + "\n";
        //    }
        //}

        //File.Open(path, FileMode.OpenOrCreate).Dispose();
        //File.WriteAllText(path, num);
    }

    public void LoadScene(string name)
    {
        prevScene = currentScene;
        currentScene = name;

        SceneManager.LoadScene(name);

        if (currentScene == "TitleManager")
        {
            SoundManager.Instance.Play("Title");
            SoundManager.Instance.SetLoop(true);
        }
        else if (currentScene == "TutorialScene")
        {

        }
    }
}
