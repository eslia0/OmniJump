using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndlessUI : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject resultPanel;

    private ScoreText scoreText;
    private Text highScore;

    // [SerializeField] Sprite[] sprites;
    // [SerializeField] Image image;

    [SerializeField] Text resultScoreText;
    [SerializeField] Text leftTime;

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

        StartCoroutine(SetResultScore(Creater.Instance.Score));
        StartCoroutine(SetLeftTime());
    }

    private void ToTitle()
    {
        Creater.Instance.player.enabled = false;
        Creater.Instance.Disable();
        SceneManager.sceneLoaded -= Creater.Instance.StartStage;

        SceneManagement.Instance.StartCoroutine(SceneManagement.Instance.LoadScene("TitleScene"));
    }

    private void Pause()
    {
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

    public IEnumerator SetResultScore(int score)
    {
        int rScore = 0;
        int amount = (score - rScore) / 10; // (int)(3 + Mathf.Log10(score)) / 10;

        while (rScore < score)
        {
            rScore += amount;
            resultScoreText.text = rScore.ToString();
            yield return null;
        }

        rScore = score;
        resultScoreText.text = rScore.ToString();

        if (SceneManagement.Instance.currentScene == "EndlessScene" && PlayerPrefs.GetInt("HighScore") < score)
        {
            PlayerPrefs.SetInt("HighScore", score);
            GooglePlayManager.Instance.ReportScore(score);
        }
    }

    public IEnumerator SetLeftTime()
    {
        leftTime.gameObject.SetActive(true);
        buttons[1].interactable = false;

        TimeSpan time = DateTime.Now.TimeOfDay - UnityAdsHelper.Instance.LastTime;

        while (time <= new TimeSpan(0, 15, 0))
        {
            time = new TimeSpan(0, 15, 0) - time;
            leftTime.text = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);

            yield return new WaitForSeconds(1.0f);

            time = DateTime.Now.TimeOfDay - UnityAdsHelper.Instance.LastTime;
        }

        leftTime.gameObject.SetActive(false);
        buttons[1].interactable = true;
    }
}
