using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    PlayerUIController player;
    private Camera mainCam;

    [SerializeField] private Button[] buttons;

    int tutorialIndex;
    [SerializeField] private Transform stepParent;
    private Transform[] steps;

    public TutorialText tutorialText;
    
    private void Start()
    {
        mainCam = Camera.main;

        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(delegate () {
            PrevTutorial();
            PlayerSet();
        });

        buttons[1].onClick.RemoveAllListeners();
        buttons[1].onClick.AddListener(delegate () {
            NextTutorial();
            PlayerSet();
        });

        player = FindObjectOfType<PlayerUIController>();
        InitTutorialUIMaps();
        PlayerSet();
    }

    private void InitTutorialUIMaps()
    {
        steps = new Transform[stepParent.childCount];
        for (int i = 0; i < steps.Length; i++)
        {
            steps[i] = stepParent.GetChild(i);
        }
    }

    public void PrevTutorial()
    {
        if (tutorialIndex > 0)
        {
            tutorialIndex--;
        }
    }

    public void NextTutorial()
    {
        if (tutorialIndex < 14)
        {
            tutorialIndex++;
        }
    }

    public void PlayerSet()
    {
        tutorialText.gameObject.SetActive(true);
        tutorialText.SetText(steps[tutorialIndex].GetComponent<TutorialTextContainer>().text);

        player.SelectUIMap(steps[tutorialIndex]);

        if (tutorialIndex > 1)
        {
            mainCam.transform.position = new Vector3(-5.12f + (9.92f * tutorialIndex), 0, -1f);
        }
        else
        {
            mainCam.transform.position = new Vector3(4.8f, 0, -1f);
        }

        mainCam.orthographicSize = 6f;

        if (tutorialIndex == 0)
        {
            mainCam.GetComponent<Animation>().Play("ZoomIn");
            player.GetComponent<Animation>().Play("FaceBlink");
        }
        else
        {
            player.GetComponent<Animation>().Stop();
        }

        player.isTeleporting = false;
        player.transform.position = player.action[0].transform.position;
        player.StartCoroutine(player.HoldPlayer(player.action[0].transform, 0.15f));
    }
}
