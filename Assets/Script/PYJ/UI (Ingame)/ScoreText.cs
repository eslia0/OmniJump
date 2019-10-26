using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour
{
    private int m_score;
    public Text m_text;
    public Animation m_animation;

    private void Awake()
    {
        m_text = GetComponent<Text>();
        m_animation = GetComponent<Animation>();

        m_score = Creater.Instance.Score;
        m_text.text = m_score.ToString();
        StartCoroutine(ScoreAnimation());
    }

    public IEnumerator ScoreAnimation()
    {
        while (Creater.Instance)
        {
            if (m_score < Creater.Instance.Score)
            {
                int amount = Creater.Instance.Score - m_score;

                if (amount >= 10)
                {
                    m_animation.Play("ScoreAnimation");
                }
                
                if (amount * 0.3f >= 1)
                {
                    m_score += (int)(amount * 0.3f);
                }
                else
                {
                    m_score += amount;
                }

                m_text.text = m_score.ToString();
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
