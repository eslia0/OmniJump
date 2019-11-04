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
    public bool isRewarded;

    // 생성될 수 있는 플랫폼의 리스트
    // 레벨마다 3개의 플랫폼이 있다.
    public GameObject[] platforms;
    public Platform nowPlatform;
    private const int maxPlatform = 60;

    // 현제 레벨
    private int level;
    private int currentMap;

    // 점수와 점수배율 레벨이 오를수록 배율이 증가
    [SerializeField] public int score { get; private set; }
    private float scoreMultiply;
    public string adReward;

    private void Awake()
    {
        if (SceneManagement.Instance.currentScene == "EndlessScene" || SceneManagement.Instance.currentScene == "PracticeScene")
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

            SceneManagement.Instance.ClearStage(currentMap);
            SceneManagement.Instance.WriteData();

            nowPlatform = Instantiate(platforms[currentMap]).GetComponent<Platform>();

            SoundManager.Instance.Play("Ready to go");
            isRewarded = false;

            SetScoreMultiply(1f);
            StartCoroutine(ScoreUp());
        }
        else if (SceneManagement.Instance.currentScene == "PracticeScene")
        {
            SceneManager.sceneLoaded += StartStage;
            currentMap = SceneManagement.Instance.selectedStage;

            SelectBGM();
            nowPlatform = Instantiate(Resources.Load<GameObject>("Maps/Map" + (currentMap + 1).ToString())).GetComponent<Platform>();

            SetScoreMultiply(1f);
            StartCoroutine(ScoreUp());
        }
        else // 테스트맵, 튜토리얼
        {
            nowPlatform = FindObjectOfType<Platform>().gameObject.GetComponent<Platform>();
            SoundManager.Instance.Play("Smash it");
        }

        GameVariablesInit();
        ExitPortal exitPortal = FindObjectOfType<ExitPortal>();
        exitPortal.Init();
    }

    // Endless 초기화
    public void InitEndless()
    {
        score = 0;
        level = 1;

        SceneManager.sceneLoaded += StartStage;

        platforms = new GameObject[maxPlatform];

        for (int i = 0; i < maxPlatform; i++)
        {
            platforms[i] = Resources.Load<GameObject>("Maps/Map" + (i + 1).ToString());
        }
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
            score = 0;
            SelectBGM();
            nowPlatform = Instantiate(Resources.Load<GameObject>("Maps/Map" + (currentMap + 1).ToString())).GetComponent<Platform>();
        }
        else if (SceneManagement.Instance.currentScene == "EndlessScene")
        {
            SelectMap();
            SelectBGM();

            nowPlatform = Instantiate(platforms[currentMap]).GetComponent<Platform>();
            SceneManagement.Instance.ClearStage(currentMap);
            SceneManagement.Instance.WriteData();

            SetScoreMultiply(1 + level * 0.03f);
        }
        
        isPaused = false;
        ExitPortal exitPortal = Object.FindObjectOfType<ExitPortal>();
        exitPortal.Init();
    }

    // 스테이지 로딩
    public void NextStage(int nextLevel) {
        UnityAdsHelper.Instance.HideBanner();

        if (nextLevel == -1)
        {
            score = 0;
            level = 1;
        }
        else if (nextLevel == 1 && level < maxPlatform)
        {
            level++;
        }

        SceneManagement.Instance.StartCoroutine(SceneManagement.Instance.LoadScene("EndlessScene"));
    }

    public void AddScore(int score)
    {
        this.score += (int)(score * scoreMultiply);
    }

    public void SetScoreMultiply(float multiply)
    {
        scoreMultiply = multiply;
    }

    // 시간의 흐름에 따른 추가 점수
    IEnumerator ScoreUp()
    {
        yield return new WaitForSeconds(1f);

        while (this)
        {
            if (!isPaused && player.enabled && !player.IsDead)
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

    // 현재 맵에 따른 음악 재생
    private void SelectBGM()
    {
        SoundManager.Instance.SetLoop(true);
        
        if (currentMap == 9 || currentMap == 22 || currentMap == 38 || currentMap == 59)
        {
            SoundManager.Instance.Play("Highway");
            if (SceneManagement.Instance.currentScene == "EndlessScene") {
                SoundManager.Instance.SetLoop(false);
            }
        }
        else if (currentMap < 9)
        {
            SoundManager.Instance.Play("Ready to go");
        }
        else if (currentMap < 22)
        {
            SoundManager.Instance.Play("Step in");
        }
        else if (currentMap < 38)
        {
            SoundManager.Instance.Play("Dreaming");
        }
        else if (currentMap < 59)
        {
            SoundManager.Instance.Play("Before");
        }
    }

    // 현재 레벨에 따른 맵 선택
    private void SelectMap()
    {
        int num = 0;
        if (level < 4)
        {
            num = Random.Range(0, 3);
            currentMap = (level - 1) * 3 + num;
        }
        else if (level == 4)
        {
            currentMap = 9;
        }
        else if (level < 9) // 10 ~ 21
        {
            num = Random.Range(-2, 1);
            currentMap = (level - 1) * 3 + num;
        }
        else if (level == 9)
        {
            currentMap = 22;
        }
        else if (level < 15) // 23 ~ 37
        {
            num = Random.Range(-4, -1);
            currentMap = (level - 1) * 3 + num;
        }
        else if (level == 15)
        {
            currentMap = 38;
        }
        else // 39 ~ 59
        {
            currentMap = 23 + level;
        }
    }
}