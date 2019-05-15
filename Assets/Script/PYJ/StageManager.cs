using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : GameVariables
{
    private static StageManager instance;
    public static StageManager Instance {
        get {
            if (instance == null)
            {
                instance = new StageManager();
            }

            return instance;
        }
    }

    public GameObject[] platforms;

    private GameObject nowPlatform;
    public Platform NowPlatform {
        get {
            return nowPlatform.GetComponent<Platform>();
        }
    }

    const int maxPlatform = 1;
    public int currentStage;

    public StageManager()
    {

    }

    ~StageManager()
    {
        platforms = null;
    }

    public void Init()
    {
        InitPlatforms();
        
        GameVariablesInit();
    }

    private void InitPlatforms()
    {
        int length = maxPlatform;
        platforms = new GameObject[length];

        for (int i = 0; i < length; i++)
        {
            platforms[i] = Resources.Load<GameObject>("Maps/Stages/Map" + (i + 1).ToString());
        }
    }

    public override void Disable()
    {
        base.Disable();
        platforms = null;
    }

    // 스테이지 시작시 생성
    public void InitStage(int num)
    {
        currentStage = num;
        nowPlatform = Object.Instantiate(platforms[currentStage - 1]);
        
        ExitPortal exitPortal = Object.FindObjectOfType<ExitPortal>();
        exitPortal.Init();
    }
}
