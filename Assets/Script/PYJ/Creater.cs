using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Creater : GameVariables
{
    private static Creater instance;
    public static Creater Instance {
        get {
            if (instance == null)
            {
                instance = new Creater();
            }

            return instance;
        }
    }

    // 테스트용
    private bool testing;
    public bool Testing {
        get { return testing; }
        private set { testing = value; }
    }
    private GameObject testPlatform;

    // 생성될 수 있는 플랫폼의 리스트
    // 레벨마다 3개의 플랫폼이 있다.
    public GameObject[] platforms;
    
    private GameObject nowPlatform;
    public Platform NowPlatform {
        get {
            if (testing)
            {
                return testPlatform.GetComponent<Platform>();
            }
            else
            {
                return nowPlatform.GetComponent<Platform>();
            }
        }
    }
    
    // 현제 레벨
    private int level;
    [SerializeField] private int score;
    public int Score {
        get {
            return score;
        }
        private set {
            score = value;
        }
    }
    private float scoreMultiply;

    private ScoreText m_scoreText;
    private ScoreText scoreText {
        set { m_scoreText = value; }
        get {
            if (m_scoreText == null)
            {
                m_scoreText = GameObject.Find("ScoreText").GetComponent<ScoreText>();
                SetScoreMultiply(1f);
            }

            return m_scoreText;
        }
    }

    private StageText m_stageText;
    public StageText stageText {
        set { m_stageText = value; }
        get {
            if (m_stageText == null)
            {
                m_stageText = GameObject.Find("StageText").GetComponent<StageText>();
            }

            return m_stageText;
        }
    }

    private int maxPlatform;
    
    // 테스트용 초기화
    public void TestInit()
    {
        testing = true;

        testPlatform = Object.FindObjectOfType<Platform>().gameObject;
        GameVariablesInit();

        ExitPortal exitPortal = Object.FindObjectOfType<ExitPortal>();
        exitPortal.Init();
    }

    public void InitEndless()
    {
        score = 0;
        level = 1;
        maxPlatform = 6;

        SceneManager.sceneLoaded += StartStage;
        GameVariablesInit();

        int length = 3 * maxPlatform;
        platforms = new GameObject[length];

        for (int i = 0; i < length; i++)
        {
            platforms[i] = Resources.Load<GameObject>("Maps/Map" + (i + 1).ToString());
        }
    }

    public void InitStage(int stageNum)
    {
        score = 0;
        maxPlatform = 40;
        SceneManager.sceneLoaded += StartStage;
        GameVariablesInit();

        level = stageNum;
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
    public void StartStage(Scene scene, LoadSceneMode mode)
    {
        if (SceneManagement.Instance.currentScene == "PYJTestScene")
        {
            int num = Random.Range(0, 3);

            nowPlatform = Object.Instantiate(platforms[(level - 1) * 3 + num]);

            scoreText.SetText(score);
        }
        else if (SceneManagement.Instance.currentScene == "StageScene")
        {
            nowPlatform = Object.Instantiate(platforms[level - 1]);

            stageText.Init();
            stageText.SetText(NowPlatform.stageText);
        }

        ExitPortal exitPortal = Object.FindObjectOfType<ExitPortal>();
        exitPortal.Init();
    }

    // 스테이지 로딩
    public void NextStage(int nextLevel)
    {
        if (nextLevel == -1)
        {
            score = 0;
            level = 1;
            scoreText.SetText(score);
        }
        else if (nextLevel == 1 && level < maxPlatform)
        {
            level++;
            SceneManagement.Instance.selectedStage++;
        }

        if (SceneManagement.Instance.currentScene == "StageScene")
        {
            SceneManager.LoadScene("StageScene");
        }
        else if (SceneManagement.Instance.currentScene == "PYJTestScene")
        {
            if (testing)
            {
                SceneManager.LoadScene("MapTest");
            }
            else
            {
                SceneManager.LoadScene("PYJTestScene");
            }
        }
    }

    public void AddScore(int score)
    {
        if (SceneManagement.Instance.currentScene == "PYJTestScene" && score <= 100 && score > 0)
        {
            this.score += (int)(score * scoreMultiply);

            scoreText.SetText(this.score);
        }
    }

    public void SetScoreMultiply(float multiply)
    {
        scoreMultiply = multiply;
    }
}
