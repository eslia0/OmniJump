using System;
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

    [SerializeField] private bool isTesting;
    public int selectedStage;
    public string currentScene;
    public string prevScene;

    public bool[] clearStage;

    void Awake()
    {
        instance = FindObjectOfType<SceneManagement>();

        if (instance != this)
        {
            Destroy(gameObject);
        }

        adsHelper = UnityAdsHelper.Instance;

        clearStage = new bool[60];
        DontDestroyOnLoad(gameObject);
        LoadData();

        try
        {

        }
        catch (Exception e)
        {

        }
        if(PlayerPrefs.GetInt("BodySkin") == -1)
        {

        }

        if (!PlayerPrefs.HasKey("BodySkin"))
        {
            PlayerPrefs.SetInt("BodySkin", 0);
        }
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/data0.txt";

        if (File.Exists(path))
        {
            string[] data = File.ReadAllLines(path);

            for (int i = 0; i < data.Length; i++)
            {
                int num = int.Parse(data[i]);
                clearStage[num] = true;
            }
        }
    }

    public void WriteData()
    {
        string path = Application.persistentDataPath + "/data0.txt";
        string num = "";

        Debug.Log("Write");
        for (int i = 0; i < clearStage.Length; i++)
        {
            if (clearStage[i])
            {
                num += i.ToString() + "\n";
                Debug.Log(i);
            }
        }

        File.Open(path, FileMode.OpenOrCreate).Dispose();
        File.WriteAllText(path, num);
    }

    public void LoadScene(string name)
    {
        prevScene = currentScene;
        currentScene = name;

        SceneManager.LoadScene(name);
    }


    [Space(10)][Header("--------------------------------------")][Space(10)]

    [SerializeField] private SkinInfo[] playerSkinArray;
    [System.Serializable] public class SkinInfo
    {
        private bool Lock;
        public void IsLock(string password, bool isLock)
        {
            isLock = Lock;
        }
        public bool getLock()
        {
            return Lock;
        }

        public Sprite playerSkinArray;
        public Sprite face;
        public ParticleSystem hitEffect;
        public ParticleSystem tailParticle;
        public Color tailColor;
    }

    // 플레이어가 선택한 body와 effect 반환
    public SkinInfo GetSkin()
    {
        return playerSkinArray[PlayerPrefs.GetInt("BodySkin")];
    }
    // 플레이어가 선택한 body와 effect 저장
    public void SetSkin(int body)
    {
        PlayerPrefs.SetInt("BodySkin", body);
    }
}
