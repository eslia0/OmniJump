using System.Collections;
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
    [SerializeField]private bool isTesting;
    private int selectedStage;

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

    public void SelectStage(int stageNum)
    {
        selectedStage = stageNum;
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
                creater.InitPlatforms();
            }
        }
        else if (name == "StageScene")
        {
            if (creater == null)
            {
                creater = Creater.Instance;
                creater.Init();
                creater.InitStagePlatforms(selectedStage);
            }
        }

        SceneManager.LoadScene(name);
    }
}
