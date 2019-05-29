using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    private GameObject titlePanel;
    private GameObject stagePanel;
    private GameObject endlessPanel;

    Camera mainCam;
    Animation UIAnim;

    [SerializeField] private Button[] buttons;
    [SerializeField] private Button[] levelButtons;
    private GameObject playerUI;

    private void Start()
    {
        InitStage();
        InitTitle();
    }

    public void InitTitle()
    {
        playerUI = GameObject.Find("PlayerUI");
        titlePanel = GameObject.Find("TitlePanel");
        stagePanel = GameObject.Find("StagePanel");
        endlessPanel = GameObject.Find("EndlessPanel");

        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(delegate () { ToStagePanel(); });
        buttons[1].onClick.RemoveAllListeners();
        buttons[1].onClick.AddListener(delegate () { ToEndlessPanel(); });
        buttons[2].onClick.RemoveAllListeners();
        buttons[2].onClick.AddListener(delegate () { BackToTitle(); });
        buttons[3].onClick.RemoveAllListeners();
        buttons[3].onClick.AddListener(delegate () { BackToStage(); });
        buttons[4].onClick.RemoveAllListeners();
        buttons[4].onClick.AddListener(delegate () { StartEndless(); });
        buttons[5].onClick.RemoveAllListeners();
        buttons[5].onClick.AddListener(delegate () { ToTitlePanel(); });

        stagePanel.SetActive(false);
        endlessPanel.SetActive(false);

        mainCam = Camera.main;
        UIAnim = GetComponent<Animation>();
    }

    private void InitStage()
    {
        levelButtons[0].onClick.RemoveAllListeners();
        levelButtons[0].onClick.AddListener(delegate () { ToLevel(1); });
        levelButtons[1].onClick.RemoveAllListeners();
        levelButtons[1].onClick.AddListener(delegate () { ToLevel(1); });
        levelButtons[2].onClick.RemoveAllListeners();
        levelButtons[2].onClick.AddListener(delegate () { ToLevel(2); });
    }

    // 타이틀
    private void ToTitlePanel()
    {
        titlePanel.SetActive(true);
        stagePanel.SetActive(false);
        endlessPanel.SetActive(false);
    }

    // 스테이지
    private void ToStagePanel()
    {
        titlePanel.SetActive(false);
        UIAnim.Play("StageButton");
        stagePanel.SetActive(true);
        titlePanel.SetActive(false);
    }

    // 스테이지에 타이틀로 돌아가기
    private void BackToTitle()
    {
        stagePanel.SetActive(false);
        StartCoroutine(MovePlayer(new Vector3(450, -67, 0)));
    }

    // 레벨 선택으로 돌아가기
    public void BackToStage()
    {
        StartCoroutine(MoveStagePanel(new Vector3(0, 0, 0)));
    }

    // 엔드리스
    private void ToEndlessPanel()
    {
        titlePanel.SetActive(false);
        endlessPanel.SetActive(true);
    }

    private void StartEndless()
    {
        SceneManagement.Instance.LoadScene("PYJTestScene");
    }

    // 레벨로 이동
    public void ToLevel(int level)
    {
        if (level == 1)
        {
            StartCoroutine(MoveStagePanel(new Vector3(-820, 0, 0)));
        }
        else if (level == 2)
        {
            StartCoroutine(MoveStagePanel(new Vector3(-1620, 0, 0)));
        }
    }

    // 플레이어 움직임
    private IEnumerator MovePlayer(Vector3 target)
    {
        RectTransform rect = playerUI.GetComponent<RectTransform>();

        while (Vector3.Distance(rect.localPosition, target) > 0.1f)
        {
            Vector3 dir = target - rect.localPosition;

            if (dir.magnitude < 1)
                dir = dir.normalized;

            rect.Translate(dir * Time.deltaTime);
            yield return null;
        }

        rect.localPosition = target;

        if (target == new Vector3(450, -67, 0))
        {
            titlePanel.SetActive(true);
        }
        else if (target == new Vector3(-230, -67, 0))
        {
            stagePanel.SetActive(true);
            titlePanel.SetActive(false);
        }
    }

    // 패널 UI움직임
    private IEnumerator MoveStagePanel(Vector3 target)
    {
        RectTransform rect = stagePanel.GetComponent<RectTransform>();

        while (Vector3.Distance(rect.localPosition, target) > 0.1f)
        {
            Vector3 dir = target - rect.localPosition;

            if (dir.magnitude < 1)
                dir = dir.normalized;

            rect.Translate(dir * Time.deltaTime * 1.5f);
            yield return null;
        }

        rect.localPosition = target;
    }
}
