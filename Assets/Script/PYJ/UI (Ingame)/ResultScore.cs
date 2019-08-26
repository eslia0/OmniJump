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
        int amount = (score - rScore) / (int)(3 + Mathf.Log10(score)) / 10;

        while (rScore < score)
        {
            rScore += amount;

            scoreText.text = rScore.ToString();

            // yield return new WaitForSeconds(0.1f);
            yield return null;
        }

        rScore = score;

        scoreText.text = rScore.ToString();
    }
}
