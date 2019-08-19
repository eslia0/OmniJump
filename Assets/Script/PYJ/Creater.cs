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
                instance = FindObjectOfType<Creater>();

                //if (instance == null) {
                //    GameObject creater = new GameObject("Creater");
                //    instance = creater.AddComponent<Creater>();
                //}
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

    // 생성될 수 있는 플랫폼의 리스트
    // 레벨마다 3개의 플랫폼이 있다.
    public GameObject[] platforms;
    
    public GameObject nowPlatform;
    public Platform NowPlatform {
        get {
            if (!nowPlatform)
                nowPlatform = FindObjectOfType<Platform>().gameObject;

            return nowPlatform.GetComponent<Platform>();
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

    private void Awake()
    {
        instance = FindObjectOfType<Creater>();
        
        if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (SceneManagement.Instance.currentScene == "EndlessScene")
        {
            InitEndless();
            int num = Random.Range(0, 3);

            nowPlatform = Instantiate(platforms[(level - 1) * 3 + num]);

            scoreText.SetText(score);
            SetScoreMultiply(1f);
        }
        else if (SceneManagement.Instance.currentScene == "StageScene")
        {
            InitStage(SceneManagement.Instance.selectedStage);
            nowPlatform = Instantiate(platforms[level - 1]);

            stageText.Init();
            stageText.SetText(NowPlatform.stageText);
        }
        else
        {
            TestInit();
        }

        ExitPortal exitPortal = FindObjectOfType<ExitPortal>();
        exitPortal.Init();
    }

    // 테스트용 초기화
    public void TestInit()
    {
        testing = true;

        nowPlatform = FindObjectOfType<Platform>().gameObject;
        GameVariablesInit();

        SetScoreMultiply(0f);
        // StartCoroutine(StartTimer());
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
        maxPlatform = 39;
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
        Destroy(gameObject);
    }

    // 스테이지 시작시 생성
    public void StartStage(Scene scene, LoadSceneMode mode)
    {
        if (SceneManagement.Instance.currentScene == "EndlessScene")
        {
            if(level == 4 || level == 9 || level == 15|| level == 25)
            {
                nowPlatform = Instantiate(platforms[(level - 1) * 3]);
            }
            else
            {
                int num = Random.Range(0, 3);

                nowPlatform = Instantiate(platforms[(level - 1) * 3 + num]);
            }

            scoreText.SetText(score);
        }
        else if (SceneManagement.Instance.currentScene == "StageScene")
        {
            nowPlatform = Instantiate(platforms[level - 1]);

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
        else if (SceneManagement.Instance.currentScene == "EndlessScene")
        {
            SceneManager.LoadScene("EndlessScene");
        }
    }

    public void AddScore(int score)
    {
        if (SceneManagement.Instance.currentScene == "EndlessScene")
        {
            this.score += (int)(score * scoreMultiply);

            scoreText.SetText(this.score);
        }
        else if (SceneManagement.Instance.currentScene == "MapTest")
        {
            this.score += (int)(score * scoreMultiply);

            scoreText.SetText(this.score);
        }
    }

    public void SetScoreMultiply(float multiply)
    {
        scoreMultiply = multiply;
    }

    IEnumerator StartTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            score += 10;
            scoreText.SetText(this.score);
        }
    }
}
