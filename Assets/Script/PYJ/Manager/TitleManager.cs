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
    private Image[] levelImage;
    private int selectedLevel;      // 현제 선택된 레벨. 0 ~ 59
    private Text selectedLevelText;

    private ScrollRect mapScroll;
    private Animation scrollAnim;
    [SerializeField] private RectTransform map;
    private Image[] maps;
    private Button[] levels;
    
    private Sprite[] mapImages;

    private void Start()
    {
        InitTitle();
        SoundManager.Instance.Play("Title");
    }

    private void Update()
    {
        selectedLevelText.text = "Lv. " + ((int)(map.anchoredPosition.x / 430 * -1 + 1.2f + selectedLevel)).ToString();
    }

    public void InitTitle()
    {
        titlePanel = transform.GetChild(0).gameObject;
        practicePanel = transform.GetChild(1).gameObject;
        mapScroll = practicePanel.transform.GetChild(3).GetComponent<ScrollRect>();
        scrollAnim = mapScroll.GetComponent<Animation>();

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

        Button backButton = practicePanel.transform.GetChild(1).GetComponent<Button>();
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(delegate () { ToTitlePanel(); });

        // 하단의 10단위 레벨 버튼 초기화
        Transform level = practicePanel.transform.GetChild(2);
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
        levels[0].onClick.AddListener(delegate () { StartCoroutine(SetLevel(0)); });
        levels[1].onClick.AddListener(delegate () { StartCoroutine(SetLevel(10)); });
        levels[2].onClick.AddListener(delegate () { StartCoroutine(SetLevel(20)); });
        levels[3].onClick.AddListener(delegate () { StartCoroutine(SetLevel(30)); });
        levels[4].onClick.AddListener(delegate () { StartCoroutine(SetLevel(40)); });
        levels[5].onClick.AddListener(delegate () { StartCoroutine(SetLevel(50)); });

        // 맵 이미지 초기화
        maps = new Image[10];

        for (int i = 0; i < 10; i++)
        {
            maps[i] = map.GetChild(i).GetChild(0).GetComponent<Image>();
        }

        SetScrollImages();
    }

    private void SelectLevel(int level)
    {
        if (SceneManagement.Instance.clearStage[level])
        {
            SceneManagement.Instance.selectedStage = level + 1;
            SceneManagement.Instance.LoadScene("PracticeScene");
        }
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
    private IEnumerator SetLevel(int level)
    {
        if (selectedLevel != level)
        {
            scrollAnim.Play("ScrollUp");
            mapScroll.velocity = Vector2.zero;
            yield return new WaitForSeconds(0.5f);
            mapScroll.horizontalNormalizedPosition = 0;

            selectedLevel = level;
            SetScrollImages();

            selectedLevelText.text = "Lv. " + (selectedLevel + 1).ToString();
            scrollAnim.Play("ScrollDown");
        }
    }

    // 스크롤 되는 맵 이미지 초기화
    private void SetScrollImages()
    {
        Button[] buttons = new Button[10];

        for (int i = 0; i < 10; i++)
        {
            maps[i].sprite = mapImages[selectedLevel + i];

            if (!SceneManagement.Instance.clearStage[selectedLevel + i])
                maps[i].color = new Color(0.2f, 0.2f, 0.2f);
            else
                maps[i].color = new Color(1, 1, 1);

            buttons[i] = maps[i].GetComponent<Button>();
            buttons[i].onClick.RemoveAllListeners();
        }

        buttons[0].onClick.AddListener(delegate () { SelectLevel(selectedLevel); });
        buttons[1].onClick.AddListener(delegate () { SelectLevel(selectedLevel + 1); });
        buttons[2].onClick.AddListener(delegate () { SelectLevel(selectedLevel + 2); });
        buttons[3].onClick.AddListener(delegate () { SelectLevel(selectedLevel + 3); });
        buttons[4].onClick.AddListener(delegate () { SelectLevel(selectedLevel + 4); });
        buttons[5].onClick.AddListener(delegate () { SelectLevel(selectedLevel + 5); });
        buttons[6].onClick.AddListener(delegate () { SelectLevel(selectedLevel + 6); });
        buttons[7].onClick.AddListener(delegate () { SelectLevel(selectedLevel + 7); });
        buttons[8].onClick.AddListener(delegate () { SelectLevel(selectedLevel + 8); });
        buttons[9].onClick.AddListener(delegate () { SelectLevel(selectedLevel + 9); });
    }
}
