using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    private static TitleManager instance;
    public static TitleManager Instance {
        get {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<TitleManager>();

                if (instance == null)
                {
                    GameObject managment = new GameObject("TitleManager");
                    instance = managment.AddComponent<TitleManager>();
                }
            }

            return instance;
        }
    }

    private GameObject titlePanel;
    private GameObject practicePanel;

    [SerializeField] private Button[] buttons;
    private Text selectedPercent;
    private Image[] levelImage;
    private int selectedLevel;      // 현제 선택된 레벨. 0 ~ 59
    private Text selectedLevelText;

    private ScrollRect mapScroll;
    [SerializeField] private RectTransform map;
    private Image[] maps;
    private Button[] levels;
    
    private Sprite[] mapImages;

    private void Start()
    {
        InitTitle();
    }

    public void InitTitle()
    {
        titlePanel = transform.GetChild(0).gameObject;
        practicePanel = transform.GetChild(1).gameObject;
        mapScroll = practicePanel.transform.GetChild(4).GetComponent<ScrollRect>();

        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(delegate () { StartEndless(); });
        buttons[1].onClick.RemoveAllListeners();
        buttons[1].onClick.AddListener(delegate () { ToTitlePanel(); });
        buttons[2].onClick.RemoveAllListeners();
        buttons[2].onClick.AddListener(delegate () { ToPracticePanel(); });

        LoadMapImage();
        InitPractice();

        titlePanel.SetActive(true);
        practicePanel.SetActive(false);
    }

    // 타이틀
    private void ToTitlePanel()
    {
        titlePanel.SetActive(true);
        practicePanel.SetActive(false);
    }

    // 프랙티스
    private void ToPracticePanel()
    {
        titlePanel.SetActive(false);
        practicePanel.SetActive(true);

        SetLevel(0);
    }
    
    // 엔드리스 시작
    private void StartEndless()
    {
        if (SceneManagement.Instance.clearStage[0])
        {
            SceneManagement.Instance.LoadScene("EndlessScene");
        }
        else
        {
            SceneManagement.Instance.LoadScene("TutorialScene");
        }
    }

    private void InitPractice()
    {
        selectedLevelText = practicePanel.transform.GetChild(0).Find("SelectedText").GetComponent<Text>();
        selectedPercent = practicePanel.transform.GetChild(1).GetComponent<Text>();

        Button backButton = practicePanel.transform.GetChild(2).GetComponent<Button>();
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(delegate () { ToTitlePanel(); });

        // 하단의 10단위 레벨 버튼 초기화
        Transform level = practicePanel.transform.GetChild(3);
        levels = new Button[6];

        levels[0] = level.GetChild(0).GetComponent<Button>();
        levels[1] = level.GetChild(1).GetComponent<Button>();
        levels[2] = level.GetChild(2).GetComponent<Button>();
        levels[3] = level.GetChild(3).GetComponent<Button>();
        levels[4] = level.GetChild(4).GetComponent<Button>();
        levels[5] = level.GetChild(5).GetComponent<Button>();
        levels[0].onClick.RemoveAllListeners();
        levels[1].onClick.RemoveAllListeners();
        levels[2].onClick.RemoveAllListeners();
        levels[3].onClick.RemoveAllListeners();
        levels[4].onClick.RemoveAllListeners();
        levels[5].onClick.RemoveAllListeners();
        levels[0].onClick.AddListener(delegate () { SetLevel(0); });
        levels[1].onClick.AddListener(delegate () { SetLevel(10); });
        levels[2].onClick.AddListener(delegate () { SetLevel(20); });
        levels[3].onClick.AddListener(delegate () { SetLevel(30); });
        levels[4].onClick.AddListener(delegate () { SetLevel(40); });
        levels[5].onClick.AddListener(delegate () { SetLevel(50); });

        // 맵 이미지 초기화
        maps = new Image[10];

        for (int i = 0; i < 10; i++)
        {
            maps[i] = map.GetChild(i).GetChild(0).GetComponent<Image>();
        }
    }

    private void SelectLevel(int move)
    {
        // 이전 레벨
        if (move == -1 && selectedLevel % 10 != 0)
        {
            map.anchoredPosition += new Vector2(400, 0);
            selectedLevel--;
        }
        // 다음 레벨
        else if (move == 1 && selectedLevel % 10 != 9)
        {
            map.anchoredPosition -= new Vector2(400, 0);
            selectedLevel++;
        }
        // 레벨 시작
        else if (move == 0)
        {
            if (SceneManagement.Instance.clearStage[selectedLevel])
            {
                SceneManagement.Instance.selectedStage = selectedLevel + 1;
                SceneManagement.Instance.LoadScene("PracticeScene");
            }
        }

        selectedLevelText.text = "Lv. " + (selectedLevel + 1).ToString();
    }

    // 리소스에서 맵 이미지를 로드
    private void LoadMapImage()
    {
        mapImages = new Sprite[60];
        for (int i = 0; i< 60; i++)
        {
            mapImages[i] = Resources.Load<Sprite>("Images/Map" + (i + 1).ToString());
        }
    }

    // 현재 레벨에 따른 맵 이미지 설정
    private void SetLevel(int level)
    {
        selectedLevel = level;

        mapScroll.horizontalNormalizedPosition = 0;

        for (int i = 0; i < 10; i++)
        {
            maps[i].sprite = mapImages[selectedLevel + i];

            if (!SceneManagement.Instance.clearStage[selectedLevel + i])
                maps[i].color = new Color(0.2f, 0.2f, 0.2f);
            else
                maps[i].color = new Color(1, 1, 1);
        }

        selectedLevelText.text = "Lv. " + (selectedLevel + 1).ToString();
    }
}
