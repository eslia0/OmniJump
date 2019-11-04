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
    public int highScore { get; private set; }
    public int coin { get; private set; }
    private ObjectData[] objectData;

    void Awake()
    {
        instance = FindObjectOfType<SceneManagement>();

        if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        clearStage = new bool[60];
        DontDestroyOnLoad(gameObject);
        LoadData();
    }

    void Start()
    {
        SceneManager.sceneLoaded += FadeIn;
        fade.gameObject.SetActive(false);
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
        highScore = PlayerPrefs.GetInt("HighScore");
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

        fade.gameObject.SetActive(true);

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
        objectData = new ObjectData[9];

        for (int i = 0; i < 9; i++)
        {
            objectData[i] = new ObjectData();
            objectData[i].SetObjectType((ObjectType)i);
            objectData[i].SetLevel(PlayerPrefs.GetInt("ObjectLevel" + i.ToString()));
            objectData[i].InitObjectData();
        }
    }

    public bool UseCoin(int cost) // 구매 가능할 시 True 반환. 불가능할 시 False 반환
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

    public bool PurchaseObjectScoreLevel(int type)
    {
        if (UseCoin(objectData[type].cost))
        {
            objectData[type].SetLevel(objectData[type].level + 1);
            PlayerPrefs.SetInt("ObjectLevel" + type.ToString(), objectData[type].level);

            objectData[type].InitObjectData();
            return true;
        }

        return false;
    }

    public ObjectData GetObjectData(ObjectType type)
    {
        return objectData[(int)type];
    }

    public void FadeIn(Scene scene, LoadSceneMode mode)
    {
        fade.fadeCanvas.SetActive(true);
        fade.StartCoroutine(fade.FadeIn());
    }
}
