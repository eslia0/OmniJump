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
    }
}

/*
        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(delegate () { SpriteChanger(0); });
        buttons[0].onClick.AddListener(delegate () { FaceDirectionChanger(0); });
        buttons[0].onClick.AddListener(delegate () { ResetOnClick(); });

        buttons[1].onClick.RemoveAllListeners();
        buttons[1].onClick.AddListener(delegate () { SpriteChanger(1); });
        buttons[1].onClick.AddListener(delegate () { FaceDirectionChanger(1); });
        buttons[1].onClick.AddListener(delegate () { ResetOnClick(); });

        buttons[2].onClick.RemoveAllListeners();
        buttons[2].onClick.AddListener(delegate () { SpriteChanger(2); });
        buttons[2].onClick.AddListener(delegate () { FaceDirectionChanger(2); });
        buttons[2].onClick.AddListener(delegate () { ResetOnClick(); });

        buttons[3].onClick.RemoveAllListeners();
        buttons[3].onClick.AddListener(delegate () { SpriteChanger(3); });
        buttons[3].onClick.AddListener(delegate () { FaceDirectionChanger(3); });
        buttons[3].onClick.AddListener(delegate () { ResetOnClick(); });

    
    private void SpriteChanger(int i)
    {
        image.sprite = sprites[i];
    }

    private void FaceDirectionChanger(int i)
    {
        player.faceDirection = i;
    }

    private void ResetOnClick()
    {
        player.onClick = true;
    }

*/
