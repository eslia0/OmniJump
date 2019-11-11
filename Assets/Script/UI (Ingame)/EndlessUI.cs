using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndlessUI : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject adPanel;
    [SerializeField] private GameObject resultPanel;

    private ScoreText scoreText;
    private Text highScore;
    private bool doubleCoin;
    [SerializeField] private GameObject doubleEffect;
    
    [SerializeField] Text resultScoreText;
    [SerializeField] Text resultCoinText;

    private void Start() {
        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(delegate () { Pause(); });

        buttons[1].onClick.RemoveAllListeners();
        buttons[1].onClick.AddListener(delegate () { Resume(); });

        buttons[2].onClick.RemoveAllListeners();
        buttons[2].onClick.AddListener(delegate () { Creater.Instance.NextStage(-1); });

        buttons[3].onClick.RemoveAllListeners();
        buttons[3].onClick.AddListener(delegate () { ToTitle(); });

        buttons[4].onClick.RemoveAllListeners();
        buttons[4].onClick.AddListener(delegate () { SelectReward("Revive"); });

        buttons[5].onClick.RemoveAllListeners();
        buttons[5].onClick.AddListener(delegate () { SelectReward("DoubleCoin"); });

        buttons[6].onClick.RemoveAllListeners();
        buttons[6].onClick.AddListener(delegate () { SkipAd(); });

        buttons[7].onClick.RemoveAllListeners();
        buttons[7].onClick.AddListener(delegate () { ToTitle(); });

        buttons[8].onClick.RemoveAllListeners();
        buttons[8].onClick.AddListener(delegate () { Creater.Instance.NextStage(-1); });

        highScore = transform.GetChild(1).GetComponent<Text>();
        scoreText = transform.GetChild(2).GetComponent<ScoreText>();
        highScore.text = PlayerPrefs.GetInt("HighScore").ToString();

        doubleCoin = false;
        pausePanel.SetActive(false);
        adPanel.SetActive(false);
        resultPanel.SetActive(false);
    }

    public void SetResultPanel() {
        resultPanel.SetActive(true);
        Creater.Instance.isRewarded = false;

        UnityAdsHelper.Instance.RequestBanner();

        StartCoroutine(SetResultScore(Creater.Instance.score));
    }

    private void ToTitle()
    {
        if (Creater.Instance)
        {
            Creater.Instance.player.enabled = false;
            Creater.Instance.Disable();
            SceneManager.sceneLoaded -= Creater.Instance.StartStage;

            UnityAdsHelper.Instance.HideBanner();

            SceneManagement.Instance.StartCoroutine(SceneManagement.Instance.LoadScene("TitleScene"));
        }
    }

    private void Pause()
    {
        if (Creater.Instance.player.enabled)
        {
            Creater.Instance.Pause();
            buttons[0].enabled = false;
            pausePanel.SetActive(true);
        }
    }

    private void Resume()
    {
        if (Creater.Instance.player.enabled)
        {
            Creater.Instance.Pause();
            buttons[0].enabled = true;
            pausePanel.SetActive(false);
        }
    }

    public void SetAdPanel() {
        buttons[0].enabled = false;
        adPanel.SetActive(true);
    }

    private void SelectReward(string reward) {
        adPanel.SetActive(false);

        SceneManagement.Instance.adReward = reward;
        Creater.Instance.isRewarded = true;

        if (reward == "Coin")
            doubleCoin = true;

        UnityAdsHelper.Instance.ShowRewardedAd();
    }

    private void SkipAd() {
        adPanel.SetActive(false);
        SetResultPanel();
    }

    public IEnumerator SetResultScore(int score)
    {
        int rScore = 0;
        int amount = (score - rScore) / 10;

        while (rScore < score)
        {
            rScore += amount;
            resultScoreText.text = rScore.ToString();
            yield return new WaitForSeconds(0.1f);
        }

        rScore = score;
        resultScoreText.text = rScore.ToString();

        StartCoroutine(SetResultCoin(rScore));

        if (SceneManagement.Instance.currentScene == "EndlessScene" && PlayerPrefs.GetInt("HighScore") < score)
        {
            PlayerPrefs.SetInt("HighScore", score);
            GooglePlayManager.Instance.ReportScore(score);
        }
    }

    public IEnumerator SetResultCoin(int score)
    {
        int coin = 0;
        int rCoin = CalculateCoin(score);

        int amount = rCoin - coin;

        if (rCoin >= 10) {
            while (coin < rCoin) {
                coin += amount / 10;
                resultCoinText.text = coin.ToString();
                yield return new WaitForSeconds(0.1f);
            }
        }

        coin = rCoin;
        resultCoinText.text = coin.ToString();

        yield return new WaitForSeconds(0.2f);

        if (doubleCoin) {
            coin *= 2;
            doubleCoin = false;
            doubleEffect.SetActive(true);
        }

        yield return null;
        resultCoinText.text = coin.ToString();

        SceneManagement.Instance.AddCoin(coin);
    }

    private int CalculateCoin(int score)
    {
        if (score <= 300)
        {
            return 1;
        }
        else
        {
            return CalculateCoin(score / 2) + (int)((Mathf.Log(score, 2) - Mathf.Log(300, 2)) * 5);
        }
    }
}
