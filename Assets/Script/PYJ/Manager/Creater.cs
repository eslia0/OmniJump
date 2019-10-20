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
            }

            return instance;
        }
    }

    private bool isPaused;
    public bool IsPaused {
        get { return isPaused; }
        private set { isPaused = value; }
    }
    private bool isRetry;

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
    private int currentMap;
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
    private ScoreText scoreText;
    private int maxPlatform;

    private void Awake()
    {
        if (SceneManagement.Instance.currentScene == "EndlessScene")
        {
            instance = FindObjectOfType<Creater>();

            if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }
    }

    // Endless Scene 처음 로드되었을때 한 번 실행
    private void Start()
    {
        if (SceneManagement.Instance.currentScene == "EndlessScene")
        {
            InitEndless();

            int num = Random.Range(0, 3);
            currentMap = (level - 1) * 3 + num;

            SceneManagement.Instance.clearStage[currentMap] = true;
            SceneManagement.Instance.WriteData();
            nowPlatform = Instantiate(platforms[currentMap]);

            scoreText = GameObject.Find("ScoreText").GetComponent<ScoreText>();
            scoreText.SetText(score);
            SetScoreMultiply(1f);

        }
        else if (SceneManagement.Instance.currentScene == "PracticeScene")
        {
            GameVariablesInit();
            SceneManager.sceneLoaded += StartStage;
            currentMap = SceneManagement.Instance.selectedStage;

            nowPlatform = Instantiate(Resources.Load<GameObject>("Maps/Map" + currentMap.ToString()));
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
        nowPlatform = FindObjectOfType<Platform>().gameObject;
        GameVariablesInit();

        // scoreText = GameObject.Find("ScoreText").GetComponent<ScoreText>();
        // SetScoreMultiply(0f);
        // StartCoroutine(StartTimer());
    }

    // Endless 초기화
    public void InitEndless()
    {
        score = 0;
        level = 1;
        maxPlatform = 60;

        SceneManager.sceneLoaded += StartStage;
        GameVariablesInit();

        int length = 60;
        platforms = new GameObject[length];

        for (int i = 0; i < length; i++)
        {
            platforms[i] = Resources.Load<GameObject>("Maps/Map" + (i + 1).ToString());
        }

        SoundManager.Instance.Play("Ready to go");
    }

    public override void Disable()
    {
        base.Disable();
        platforms = null;
        Destroy(gameObject);
    }

    // 매 스테이지마다 실행
    public void StartStage(Scene scene, LoadSceneMode mode)
    {
        if (SceneManagement.Instance.currentScene == "PracticeScene")
        {
            nowPlatform = Instantiate(Resources.Load<GameObject>("Maps/Map" + currentMap.ToString()));
            return;
        }
        else if (SceneManagement.Instance.currentScene == "EndlessScene")
        {
            // 재시작이면 이전 맵 그대로 실행
            if (!isRetry)
            {
                if (level == 4 || level == 9 || level == 15 || level == 35)
                {
                    currentMap = (level - 1) * 3;

                    SoundManager.Instance.Play("13");
                }
                else if (level > 15)
                {
                    currentMap = 23 + level;
                }
                else
                {
                    int num = Random.Range(0, 3);
                    currentMap = (level - 1) * 3 + num;
                }

                if (level == 5)
                {
                    SoundManager.Instance.Play("Step in");
                }
                else if (level == 10)
                {
                    SoundManager.Instance.Play("Dreaming");
                }
                else if (level == 16)
                {
                    SoundManager.Instance.Play("Before");
                }

                SetScoreMultiply(1 + level * 0.03f);
            }

            SceneManagement.Instance.clearStage[currentMap] = true;
            SceneManagement.Instance.WriteData();

            nowPlatform = Instantiate(platforms[currentMap]);
            isRetry = false;
            
            scoreText = GameObject.Find("ScoreText").GetComponent<ScoreText>();
            scoreText.SetText(score);

            ExitPortal exitPortal = Object.FindObjectOfType<ExitPortal>();
            exitPortal.Init();
        }
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
        }

        SceneManager.LoadScene("EndlessScene");
    }

    public void AddScore(int score)
    {
        if (scoreText)
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
            scoreText.SetText(score);
        }
    }

    public void Pause()
    {
        isPaused = !isPaused;
    }
}
