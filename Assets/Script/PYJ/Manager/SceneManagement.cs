using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ObjectScore
{
    JumpPad = 0,
    CirclePad = 1,
    ReversePad = 2,
    Missile = 3,
    Gravity = 4,
    Teleport = 5,
    Lift = 6,
    Rotate = 7,
    Pause = 8,
}

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

    public Fade fade;
    // Scene Manage 관련 Scene 정보
    public int selectedStage;
    public string currentScene;
    public string prevScene;

    private bool[] clearStage;
    private int highScore;
    private int coin;
    private int[] objectScoreLevels;

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

    void Start()
    {
        SceneManager.sceneLoaded += FadeIn;
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

        coin = PlayerPrefs.GetInt("Coin");
        InitObjectScoreLevel();
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
    }

    public IEnumerator LoadScene(string name)
    {
        prevScene = currentScene;
        currentScene = name;

        fade.fadeCanvas.SetActive(true);
        fade.StartCoroutine(fade.FadeOut());

        while (!fade.ended)
        {
            yield return null;
        }

        SceneManager.LoadScene(name);
    }

    public bool[] GetClearData()
    {
        return clearStage;
    }

    public void ClearStage(int stage)
    {
        clearStage[stage] = true;
    }

    private void InitObjectScoreLevel()
    {
        objectScoreLevels = new int[9];

        for (int i = 0; i < 9; i++)
        {
            objectScoreLevels[i] = PlayerPrefs.GetInt("ObjectScoreLevel" + i.ToString());
        }
    }

    public bool UseCoin(int cost)   // 구매 가능할 시 True 반환. 불가능할 시 False 반환
    {
        if (coin < cost)
        {
            return false;
        }
        else
        {
            coin -= cost;
            PlayerPrefs.SetInt("Coin", coin);
            return true;
        }
    }

    public void AddCoin(int amount)
    {
        if (amount < 200)
        {
            coin += amount;

            PlayerPrefs.SetInt("Coin", coin);
        }
    }

    public void PurchaseObjectScoreLevel(ObjectScore objectName, int cost)
    {
        if (UseCoin(cost))
        {
            objectScoreLevels[(int)objectName]++;
            PlayerPrefs.SetInt("ObjectScoreLevel" + (int)objectName, objectScoreLevels[(int)objectName]);
        }
    }

    public float GetObjectScoreLevel(ObjectScore objectName)
    {
        return objectScoreLevels[(int)objectName];
    }

    public void FadeIn(Scene scene, LoadSceneMode mode)
    {
        fade.fadeCanvas.SetActive(true);
        fade.StartCoroutine(fade.FadeIn());
    }
}
