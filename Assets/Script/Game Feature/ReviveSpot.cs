using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReviveSpot : MonoBehaviour
{
    private PlayerController m_player;
    private TutorialText tutorialText;
    [TextArea]
    public string text;

    // Start is called before the first frame update
    void Awake()
    {
        m_player = Creater.Instance.player;

        tutorialText = FindObjectOfType<TutorialText>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_player.reviveSpot != transform)
        {
            if (m_player.transform.position.x > transform.position.x)
            {
                m_player.reviveSpot = transform;

                if (text != "")
                {
                    tutorialText.gameObject.SetActive(true);
                    tutorialText.SetText(text);
                }
                else
                {
                    tutorialText.gameObject.SetActive(false);
                }

                this.enabled = false;
            }
        }
    }
}
