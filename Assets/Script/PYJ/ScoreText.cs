using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour
{
    public Text text;
    public Animation m_animation;

    // Start is called before the first frame update
    public void Init()
    {
        text = GetComponent<Text>();
        m_animation = GetComponent<Animation>();
    }

    public void SetText(string text)
    {
        this.text.text = text;
        m_animation.Play();
    }
}
