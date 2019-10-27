using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndlessUI : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private ResultScore resultScore;

    private ScoreText scoreText;
    private Text highScore;

    // [SerializeField] Sprite[] sprites;
    // [SerializeField] Image image;

    private void Start()
    {
        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(delegate () { Creater.Instance.NextStage(-1); });

        buttons[1].onClick.RemoveAllListeners();
        buttons[1].onClick.AddListener(delegate () { UnityAdsHelper.Instance.ShowRewardedAd(); });

        buttons[2].onClick.RemoveAllListeners();
        buttons[2].onClick.AddListener(delegate () { ToTitle(); });

        buttons[3].onClick.RemoveAllListeners();
        buttons[3].onClick.AddListener(delegate () { Pause(); });

        buttons[4].onClick.RemoveAllListeners();
        buttons[4].onClick.AddListener(delegate () { Resume(); });

        buttons[5].onClick.RemoveAllListeners();
        buttons[5].onClick.AddListener(delegate () { Creater.Instance.NextStage(-1); });

        buttons[6].onClick.RemoveAllListeners();
        buttons[6].onClick.AddListener(delegate () { ToTitle(); });

        highScore = transform.GetChild(1).GetComponent<Text>();
        scoreText = transform.GetChild(2).GetComponent<ScoreText>();
        highScore.text = PlayerPrefs.GetInt("HighScore").ToString();

        pausePanel.SetActive(false);
        resultPanel.SetActive(false);
    }

    public void SetResultPanel()
    {
        resultPanel.SetActive(true);
        buttons[3].enabled = false;

        resultScore.StartCoroutine(resultScore.SetResultScore(Creater.Instance.Score));
    }

    private void ToTitle()
    {
        Creater.Instance.Disable();
        SceneManager.sceneLoaded -= Creater.Instance.StartStage;

        SceneManagement.Instance.LoadScene("TitleScene");
    }

    private void Pause()
    {
        Debug.Log(Creater.Instance.player.enabled);

        if (Creater.Instance.player.enabled)
        {
            Creater.Instance.Pause();
            buttons[3].enabled = false;
            pausePanel.SetActive(true);
        }
    }

    private void Resume()
    {
        if (Creater.Instance.player.enabled)
        {
            Creater.Instance.Pause();
            buttons[3].enabled = true;
            pausePanel.SetActive(false);
        }
    }
}
