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

        if (!PlayerPrefs.HasKey("BodySkin"))
        {
            PlayerPrefs.SetInt("BodySkin", 0);
        }
        skinArray = XMLManager.S.LoadSkinXML();
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

    [Space(10)][Header("--------------------------------------")][Space(10)]

    [SerializeField] private List<SkinInfo> skinArray = new List<SkinInfo>();
    public List<SkinInfo> GetSkinArray() { return skinArray; }
    [System.Serializable] public class SkinInfo
    {
        public bool LOCK = true;
        public string body;
        public string face;
        //public string hitEffect;

        public SkinTailState tailState;
        public string tailEffect;
        public Color tailColor;
    }
    public enum SkinTailState
    {
        Effect,
        Color
    }
    
    // 플레이어가 선택한 body와 effect 반환
    public SkinInfo GetSkin()
    {
        return skinArray[PlayerPrefs.GetInt("BodySkin")];
    }

    public Sprite GetBody()
    {
        return Resources.Load<Sprite>(skinArray[PlayerPrefs.GetInt("BodySkin")].body);
    }
    public Sprite GetFace()
    {
        return Resources.Load<Sprite>(skinArray[PlayerPrefs.GetInt("BodySkin")].face);
    }

    // 플레이어가 선택한 body와 effect 저장
    public void SetSkin(int body)
    {
        PlayerPrefs.SetInt("BodySkin", (body < skinArray.Count) ? body : 0) ;
    }

    public GameObject SetTailEffect(string path, Color color)
    {
        GameObject game = Resources.Load<GameObject>(path);

        // ColorOverTime 설정
        ParticleSystem.ColorOverLifetimeModule cot = game.GetComponent<ParticleSystem>().colorOverLifetime;

        Gradient gradient = new Gradient();
        gradient.SetKeys(new GradientColorKey[] { // 시간에 따라 변화하도록 설정
            new GradientColorKey(color, 0.0f), new GradientColorKey(Color.white, 1.0f) }, // 색상
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 0.75f), new GradientAlphaKey(0.0f, 1.0f) }); // 알파값

        cot.color = gradient;

        return game;
    }
}
