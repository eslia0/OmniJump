using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResultScore : MonoBehaviour
{
    [SerializeField] Text scoreText;

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
}
