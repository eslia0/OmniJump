using UnityEngine;
using UnityEngine.UI;

public class StageText : MonoBehaviour
{
    public Text m_text;
    public Animation m_animation;
    
    public void Init()
    {
        m_text = transform.GetChild(0).GetComponent<Text>();
        m_animation = GetComponent<Animation>();
    }
    
    public void SetText(string text)
    {
        m_text.text = text;
        m_animation.Play("PanelIn");
    }
}
