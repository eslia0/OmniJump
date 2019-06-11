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

    Creater creater;
    [SerializeField] private bool isTesting;
    public int selectedStage;
    public string currentScene;
    public string prevScene;

    private int clearStage;
    public int ClearStage { get => clearStage; private set => clearStage = value; }

    void Awake()
    {
        instance = FindObjectOfType<SceneManagement>();

        if (instance != this)
        {
            Destroy(gameObject);
        }

        if (isTesting)
        {
            creater = Creater.Instance;
            creater.TestInit();
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/data0.txt";

        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            ClearStage = int.Parse(data);
        }
        else
        {
            File.Open(path, FileMode.OpenOrCreate).Dispose();
            File.WriteAllText(path, "0");
        }
    }

    public void WriteData()
    {
        string path = Application.persistentDataPath + "/data0.txt";
        clearStage = selectedStage;

        File.Open(path, FileMode.OpenOrCreate).Dispose();
        File.WriteAllText(path, clearStage.ToString());
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
                creater.InitEndless();
            }
        }
        else if (name == "StageScene")
        {
            if (creater == null)
            {
                creater = Creater.Instance;
                creater.InitStage(selectedStage);
            }
        }

        prevScene = currentScene;
        currentScene = name;


        SceneManager.LoadScene(name);
    }
}
