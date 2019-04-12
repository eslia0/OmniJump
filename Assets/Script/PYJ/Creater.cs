using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Creater : GameVariables
{
    private static Creater instance;
    public static Creater Instance {
        get {
            if (instance == null)
            {
                instance = FindObjectOfType<Creater>();

                if (instance == null)
                {
                    GameObject creater = new GameObject();
                    creater.name = "Creater";
                    instance = creater.AddComponent<Creater>();
                    DontDestroyOnLoad(creater);
                    instance.Init();
                }
            }

            return instance;
        }
    }

    // 테스트용
    [SerializeField] private bool testing;
    public bool Testing {
        get { return testing; }
        private set { testing = value; }
    }
    [SerializeField] private GameObject testPlatform;

    // 생성될 수 있는 플랫폼의 리스트
    // 레벨마다 6개의 플랫폼이 있다.
    public GameObject[] platforms;

    // 총 4개의 플랫폼을 저장
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
    private float scoreMultiply;

    private ScoreText m_scoreText;
    private ScoreText scoreText {
        set { m_scoreText = value; }
        get {
            if(m_scoreText == null)
            {
                m_scoreText = GameObject.Find("ScoreText").GetComponent<ScoreText>();
                SetScoreMultiply(1f);
            }

            return m_scoreText;
        }
    }


    void Start()
    {
        if (testing)
        {
            score = 0;
            level = 1;

            scoreText.SetText(score.ToString());

            SetPopParticles();
        }
    }

    // 시작시 플랫폼밑 점수 초기화
    public void Init()
    {
        score = 0;
        level = 1;

        maxLevel = 2;

        InitPlatforms();

        SceneManager.sceneLoaded += InitStage;
        
        GameVariablesInit();
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

    // 스테이지 시작시 생성
    public void InitStage(Scene scene, LoadSceneMode mode)
    {
        int num = Random.Range(0, 3);

        nowPlatform = Instantiate(platforms[(level - 1) * 3 + num]);
        Debug.Log(nowPlatform);

        Platform platform = nowPlatform.GetComponent<Platform>();

        scoreText.SetText(score.ToString());
    }

    // 스테이지 로딩
    public void NextStage(int nextLevel)
    {
        if (nextLevel == -1)
        {
            score = 0;
            level = 1;
        }
        else if (nextLevel == 1 && level < maxLevel)
        {
            level++;

            score += nowPlatform.GetComponent<Platform>().score;
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
            
            scoreText.SetText(this.score.ToString());
        }
    }

    public void SetScoreMultiply(float multiply)
    {
        scoreMultiply = multiply;
    }
}
