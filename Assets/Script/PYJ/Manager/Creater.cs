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

    public bool isPaused;

    // 생성될 수 있는 플랫폼의 리스트
    // 레벨마다 3개의 플랫폼이 있다.
    public GameObject[] platforms;
    public Platform nowPlatform;
    private int maxPlatform;

    // 현제 레벨
    private int level;
    private int currentMap;

    // 점수와 점수배율 레벨이 오를수록 배율이 증가
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
    public EndlessUI endUI;

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

            nowPlatform = Instantiate(platforms[currentMap]).GetComponent<Platform>();
            endUI = FindObjectOfType<EndlessUI>();

            SetScoreMultiply(1f);
            StartCoroutine(ScoreUp());
        }
        else if (SceneManagement.Instance.currentScene == "PracticeScene")
        {
            GameVariablesInit();
            SceneManager.sceneLoaded += StartStage;
            currentMap = SceneManagement.Instance.selectedStage;

            nowPlatform = Instantiate(Resources.Load<GameObject>("Maps/Map" + currentMap.ToString())).GetComponent<Platform>();

            SetScoreMultiply(1f);
            StartCoroutine(ScoreUp());
        }
        else
        {
            TestInit();
        }

        ExitPortal exitPortal = FindObjectOfType<ExitPortal>();
        exitPortal.Init();
    }

    // 테스트용, 튜토리얼용 초기화
    public void TestInit()
    {
        nowPlatform = FindObjectOfType<Platform>().gameObject.GetComponent<Platform>();
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
            nowPlatform = Instantiate(Resources.Load<GameObject>("Maps/Map" + currentMap.ToString())).GetComponent<Platform>();
            return;
        }
        else if (SceneManagement.Instance.currentScene == "EndlessScene")
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

            SceneManagement.Instance.clearStage[currentMap] = true;
            SceneManagement.Instance.WriteData();
            isPaused = false;

            nowPlatform = Instantiate(platforms[currentMap]).GetComponent<Platform>();
            endUI = FindObjectOfType<EndlessUI>();

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
        }
        else if (nextLevel == 1 && level < maxPlatform)
        {
            level++;
        }

        // 플레이어가 죽었을 시에 다시 점수 증가
        if(nextLevel == -1 || nextLevel == 0)
        {
            StartCoroutine(ScoreUp());
        }

        SceneManager.LoadScene("EndlessScene");
    }

    public void AddScore(int score)
    {
        this.score += (int)(score * scoreMultiply);
    }

    public void SetScoreMultiply(float multiply)
    {
        scoreMultiply = multiply;
    }

    IEnumerator ScoreUp()
    {
        yield return new WaitForSeconds(1f);

        while (this)
        {
            if (!isPaused && !player.IsDead)
            {
                score += (int)(5 * scoreMultiply);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public void Pause()
    {
        isPaused = !isPaused;
    }
}
