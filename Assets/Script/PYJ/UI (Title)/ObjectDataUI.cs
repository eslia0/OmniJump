using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectDataUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    Transform[] objectUI;
    
    Text[] levelText;
    Text[] currentScoreText;
    Text[] nextScoreText;
    Text[] costText;
    Button[] upgradeButton;

    private void Awake()
    {
        objectUI = new Transform[content.childCount];
        levelText = new Text[content.childCount];
        currentScoreText = new Text[content.childCount];
        nextScoreText = new Text[content.childCount];
        costText = new Text[content.childCount];
        upgradeButton = new Button[content.childCount];

        InitObjectDataUI();
    }

    public void InitObjectDataUI()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            objectUI[i] = content.GetChild(i);
            levelText[i] = objectUI[i].GetChild(0).GetComponent<Text>();
            currentScoreText[i] = objectUI[i].GetChild(1).GetComponent<Text>();
            nextScoreText[i] = objectUI[i].GetChild(2).GetComponent<Text>();
            costText[i] = objectUI[i].GetChild(3).GetComponent<Text>();
            upgradeButton[i] = objectUI[i].GetChild(6).GetComponent<Button>();
        }

        upgradeButton[0].onClick.RemoveAllListeners();
        upgradeButton[1].onClick.RemoveAllListeners();
        upgradeButton[2].onClick.RemoveAllListeners();
        upgradeButton[3].onClick.RemoveAllListeners();
        upgradeButton[4].onClick.RemoveAllListeners();
        upgradeButton[5].onClick.RemoveAllListeners();
        upgradeButton[6].onClick.RemoveAllListeners();
        upgradeButton[7].onClick.RemoveAllListeners();
        upgradeButton[8].onClick.RemoveAllListeners();

        upgradeButton[0].onClick.AddListener(delegate () {
            if (SceneManagement.Instance.PurchaseObjectScoreLevel(0))
            {
                StartAnim(0);
                SetObjectDataUI(0);
            }
        });
        upgradeButton[1].onClick.AddListener(delegate () {
            if (SceneManagement.Instance.PurchaseObjectScoreLevel(1))
            {
                StartAnim(1);
                SetObjectDataUI(1);
            }
        });
        upgradeButton[2].onClick.AddListener(delegate () {
            if (SceneManagement.Instance.PurchaseObjectScoreLevel(2))
            {
                StartAnim(2);
                SetObjectDataUI(2);
            }
        });
        upgradeButton[3].onClick.AddListener(delegate () {
            if (SceneManagement.Instance.PurchaseObjectScoreLevel(3))
            {
                StartAnim(3);
                SetObjectDataUI(3);
            }
        });
        upgradeButton[4].onClick.AddListener(delegate () {
            if (SceneManagement.Instance.PurchaseObjectScoreLevel(4))
            {
                StartAnim(4);
                SetObjectDataUI(4);
            }
        });
        upgradeButton[5].onClick.AddListener(delegate () {
            if (SceneManagement.Instance.PurchaseObjectScoreLevel(5))
            {
                StartAnim(5);
                SetObjectDataUI(5);
            }
        });
        upgradeButton[6].onClick.AddListener(delegate () {
            if (SceneManagement.Instance.PurchaseObjectScoreLevel(6))
            {
                StartAnim(6);
                SetObjectDataUI(6);
            }
        });
        upgradeButton[7].onClick.AddListener(delegate () {
            if (SceneManagement.Instance.PurchaseObjectScoreLevel(7))
            {
                StartAnim(7);
                SetObjectDataUI(7);
            }
        });
        upgradeButton[8].onClick.AddListener(delegate () {
            if (SceneManagement.Instance.PurchaseObjectScoreLevel(8))
            {
                StartAnim(8);
                SetObjectDataUI(8);
            }
        });
    }

    public void SetAllObjectData()
    {
        for (int i = 0; i < objectUI.Length; i++)
        {
            SetObjectDataUI(i);
        }
    }

    public void SetObjectDataUI(int type)
    {
        ObjectData data = SceneManagement.Instance.GetObjectData((ObjectType)type);

        if (data.maxLevel == data.level)
        {
            levelText[type].text = "Lv " + data.level.ToString();
            currentScoreText[type].text = data.currentScore.ToString();
            nextScoreText[type].text = data.currentScore.ToString();
            costText[type].text = "Max";
            upgradeButton[type].enabled = false;
        }
        else
        {
            levelText[type].text = "Lv " + data.level.ToString();
            currentScoreText[type].text = data.currentScore.ToString();
            nextScoreText[type].text = data.nextScore.ToString();
            costText[type].text = data.cost.ToString();
        }
    }

    public void StartAnim(int type)
    {
        levelText[type].GetComponent<Animation>().Play();
        currentScoreText[type].GetComponent<Animation>().Play();
        nextScoreText[type].GetComponent<Animation>().Play();
        costText[type].GetComponent<Animation>().Play();
    }
}
