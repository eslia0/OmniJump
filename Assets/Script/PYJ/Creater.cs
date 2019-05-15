using UnityEngine;
using UnityEngine.UI;
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

    [SerializeField] private int maxLevel;
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

    public Creater()
    {

    }

    ~Creater()
    {
        platforms = null;
    }

    // 시작시 플랫폼밑 점수 초기화
    public void Init()
    {
        score = 0;
        level = 1;

        maxLevel = 6;
        InitPlatforms();

        SceneManager.sceneLoaded += InitStage;
        GameVariablesInit();
    }
    
    // 테스트용 초기화
    public void TestInit()
    {
        testing = true;

        testPlatform = Object.FindObjectOfType<Platform>().gameObject;
        GameVariablesInit();

        ExitPortal exitPortal = Object.FindObjectOfType<ExitPortal>();
        exitPortal.Init();
    }

    private void InitPlatforms()
    {
        int length = 3 * maxLevel;
        platforms = new GameObject[length];

        for (int i = 0; i < length; i++)
        {
            platforms[i] = Resources.Load<GameObject>("Maps/Map" + (i + 1).ToString());
        }
    }

    public override void Disable()
    {
        base.Disable();
        platforms = null;
    }

    // 스테이지 시작시 생성
    public void InitStage(Scene scene, LoadSceneMode mode)
    {
        int num = Random.Range(0, 3);

        nowPlatform = Object.Instantiate(platforms[(level - 1) * 3 + num]);

        scoreText.SetText(score);
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
        else if (nextLevel == 1 && level < maxLevel)
        {
            level++;
        }

        if (testing)
        {
            SceneManager.LoadScene("MapTest");
        }
        else
        {
            SceneManager.LoadScene("PYJTestScene");
        }
    }

    public void AddScore(int score)
    {
        if (score <= 100 && score > 0)
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
