using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonInput : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private ResultScore resultScore;

    [SerializeField] private Sprite play;
    [SerializeField] private Sprite pause;

    // [SerializeField] Sprite[] sprites;
    // [SerializeField] Image image;

    private void Start()
    {
        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(delegate () { Creater.Instance.NextStage(-1); });

        buttons[1].onClick.RemoveAllListeners();
        buttons[1].onClick.AddListener(delegate () { UnityAdsHelper.Instance.ShowRewardedAd(); });

        buttons[2].onClick.RemoveAllListeners();
        buttons[2].onClick.AddListener(delegate () { ToTitle(); });

        buttons[3].onClick.RemoveAllListeners();
        buttons[3].onClick.AddListener(delegate () { Pause(); });

        resultPanel.SetActive(false);
    }

    public void SetResultPanel()
    {
        resultPanel.SetActive(true);

        resultScore.StartCoroutine(resultScore.SetResultScore(Creater.Instance.Score));
    }

    private void ToTitle()
    {
        Creater.Instance.Disable();
        SceneManager.sceneLoaded -= Creater.Instance.StartStage;

        SceneManagement.Instance.LoadScene("TitleScene");
    }

    private void Pause()
    {
        Creater.Instance.Pause();

        if (Creater.Instance.IsPaused)
        {
            buttons[3].GetComponent<Image>().sprite = play;
        }
        else
        {
            buttons[3].GetComponent<Image>().sprite = pause;
        }
    }
}
