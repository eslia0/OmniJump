using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour
{
    public Text m_text;
    public Animation m_animation;

    // Start is called before the first frame update
    public void Init()
    {
        m_text = GetComponent<Text>();
        m_animation = GetComponent<Animation>();
    }

    public void SetText(string text)
    {
        m_text.text = text;
        if (text != "0")
            m_animation.Play("ScoreAnimation");
    }
}
