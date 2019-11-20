using UnityEngine;
using UnityEngine.UI;

public class TutorialText : MonoBehaviour
{
    public Text m_text;
    public Animation m_animation;
    
    public void Awake()
    {
        gameObject.SetActive(false);
    }
    
    public void SetText(string text)
    {
        m_text.text = text;
        // m_animation.Play("TutorialPanelIn");

        // Invoke("OutText", 3.0f);
    }

    public void OutText()
    {
        m_animation.Play("TutorialPanelOut");
    }
}
