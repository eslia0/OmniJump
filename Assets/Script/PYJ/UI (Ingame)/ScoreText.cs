using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour
{
    private int m_score;
    public Text m_text;
    public Animation m_animation;

    // Start is called before the first frame update
    public void Init()
    {
        m_text = GetComponent<Text>();
        m_animation = GetComponent<Animation>();
    }

    public void SetText (int score)
    {
        if (score == 0 || m_score == 0)
        {
            m_score = score;
            m_text.text = m_score.ToString();
        }
        else
        {
            StartCoroutine(ScoreAnimation(score));
        }
    }

    public IEnumerator ScoreAnimation(int score)
    {
        m_animation.Play("ScoreAnimation");

        int amount = score - m_score;

        while (m_score < score)
        {
            m_score += (int)(amount * 0.3f);
            yield return new WaitForSeconds(0.1f);

            m_text.text = m_score.ToString();
        }

        m_score = score;
        m_text.text = m_score.ToString();
    }
}
