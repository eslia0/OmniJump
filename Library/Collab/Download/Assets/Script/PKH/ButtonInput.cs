using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInput : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    // [SerializeField] Sprite[] sprites;
    // [SerializeField] Image image;

    private void Start()
    {
        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(delegate () { Creater.Instance.NextStage(-1); });
        buttons[0].onClick.AddListener(delegate () { SetUIButton(false); });

        buttons[1].onClick.RemoveAllListeners();
        buttons[1].onClick.AddListener(delegate () { UnityAdsHelper.Instance.ShowRewardedAd(); });
        buttons[1].onClick.AddListener(delegate () { SetUIButton(false); });

        SetUIButton(false);
    }

    public void SetUIButton(bool isActive)
    {
        buttons[0].gameObject.SetActive(isActive);
        buttons[1].gameObject.SetActive(isActive);
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
