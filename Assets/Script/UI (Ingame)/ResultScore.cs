using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResultScore : MonoBehaviour
{
    [SerializeField] Text scoreText;
    [SerializeField] Text leftTime;

    public IEnumerator SetResultScore(int score)
    {
        scoreText = GetComponent<Text>();

        int rScore = 0;
        int amount = (score - rScore) / 10; // (int)(3 + Mathf.Log10(score)) / 10;

        while (rScore < score)
        {
            rScore += amount;
            scoreText.text = rScore.ToString();
            yield return null;
        }

        rScore = score;
        scoreText.text = rScore.ToString();

        if (SceneManagement.Instance.currentScene == "EndlessScene" && PlayerPrefs.GetInt("HighScore") < score)
        {
            PlayerPrefs.SetInt("HighScore", score);
            GooglePlayManager.Instance.ReportScore(score);
        }
    }

    public IEnumerator SetLeftTime()
    {
        leftTime.gameObject.SetActive(true);

        TimeSpan time = DateTime.Now.TimeOfDay - UnityAdsHelper.Instance.LastTime;

        while (time <= new TimeSpan(0, 15, 0))
        {
            leftTime.text = time.ToString("mm:ss");

            yield return new WaitForSeconds (1.0f);

            time = DateTime.Now.TimeOfDay - UnityAdsHelper.Instance.LastTime;
        }

        leftTime.gameObject.SetActive(false);
    }
}
