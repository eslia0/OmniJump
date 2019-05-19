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

    [SerializeField] private Button[] buttons;

    private void Start()
    {
        InitTitle();
    }

    public void InitTitle()
    {
        titlePanel = GameObject.Find("TitlePanel");
        stagePanel = GameObject.Find("StagePanel");
        endlessPanel = GameObject.Find("EndlessPanel");

        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(delegate () { ToStagePanel(); });
        buttons[1].onClick.RemoveAllListeners();
        buttons[1].onClick.AddListener(delegate () { ToEndlessPanel(); });
        buttons[2].onClick.RemoveAllListeners();
        buttons[2].onClick.AddListener(delegate () { SceneManagement.Instance.SelectStage(1); });
        buttons[2].onClick.AddListener(delegate () { StartStage(); });
        buttons[3].onClick.RemoveAllListeners();
        buttons[3].onClick.AddListener(delegate () { ToTitlePanel(); });
        buttons[4].onClick.RemoveAllListeners();
        buttons[4].onClick.AddListener(delegate () { StartEndless(); });
        buttons[5].onClick.RemoveAllListeners();
        buttons[5].onClick.AddListener(delegate () { ToTitlePanel(); });

        stagePanel.SetActive(false);
        endlessPanel.SetActive(false);
    }

    private void ToTitlePanel()
    {
        titlePanel.SetActive(true);
        stagePanel.SetActive(false);
        endlessPanel.SetActive(false);
    }

    private void ToStagePanel()
    {
        titlePanel.SetActive(false);
        stagePanel.SetActive(true);
        endlessPanel.SetActive(false);
    }

    private void ToEndlessPanel()
    {
        titlePanel.SetActive(false);
        stagePanel.SetActive(false);
        endlessPanel.SetActive(true);
    }

    private void StartStage()
    {
        SceneManagement.Instance.LoadScene("StageScene");
    }

    private void StartEndless()
    {
        SceneManagement.Instance.LoadScene("PYJTestScene");
    }
}
