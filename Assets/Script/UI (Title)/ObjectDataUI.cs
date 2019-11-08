using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectDataUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private Text objectCoin;
    Transform[] objectUI;
    
    Text[] levelText;
    Text[] currentScoreText;
    Text[] nextScoreText;
    Text[] costText;
    GameObject[] coinImage;
    Button[] upgradeButton;

    private void Awake()
    {
        int size = content.childCount;
        objectUI = new Transform[size];
        levelText = new Text[size];
        currentScoreText = new Text[size];
        nextScoreText = new Text[size];
        costText = new Text[size];
        upgradeButton = new Button[size];
        coinImage = new GameObject[size];

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
            coinImage[i] = objectUI[i].GetChild(4).gameObject;
            upgradeButton[i] = objectUI[i].GetChild(7).GetComponent<Button>();
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
                SetObjectCoin();
            }
        });
        upgradeButton[1].onClick.AddListener(delegate () {
            if (SceneManagement.Instance.PurchaseObjectScoreLevel(1))
            {
                StartAnim(1);
                SetObjectDataUI(1);
                SetObjectCoin();
            }
        });
        upgradeButton[2].onClick.AddListener(delegate () {
            if (SceneManagement.Instance.PurchaseObjectScoreLevel(2))
            {
                StartAnim(2);
                SetObjectDataUI(2);
                SetObjectCoin();
            }
        });
        upgradeButton[3].onClick.AddListener(delegate () {
            if (SceneManagement.Instance.PurchaseObjectScoreLevel(3))
            {
                StartAnim(3);
                SetObjectDataUI(3);
                SetObjectCoin();
            }
        });
        upgradeButton[4].onClick.AddListener(delegate () {
            if (SceneManagement.Instance.PurchaseObjectScoreLevel(4))
            {
                StartAnim(4);
                SetObjectDataUI(4);
                SetObjectCoin();
            }
        });
        upgradeButton[5].onClick.AddListener(delegate () {
            if (SceneManagement.Instance.PurchaseObjectScoreLevel(5))
            {
                StartAnim(5);
                SetObjectDataUI(5);
                SetObjectCoin();
            }
        });
        upgradeButton[6].onClick.AddListener(delegate () {
            if (SceneManagement.Instance.PurchaseObjectScoreLevel(6))
            {
                StartAnim(6);
                SetObjectDataUI(6);
                SetObjectCoin();
            }
        });
        upgradeButton[7].onClick.AddListener(delegate () {
            if (SceneManagement.Instance.PurchaseObjectScoreLevel(7))
            {
                StartAnim(7);
                SetObjectDataUI(7);
                SetObjectCoin();
            }
        });
        upgradeButton[8].onClick.AddListener(delegate () {
            if (SceneManagement.Instance.PurchaseObjectScoreLevel(8))
            {
                StartAnim(8);
                SetObjectDataUI(8);
                SetObjectCoin();
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

    public void SetObjectCoin() {
        objectCoin.text = SceneManagement.Instance.coin.ToString();
    }

    public void SetObjectDataUI(int type)
    {
        ObjectData data = SceneManagement.Instance.GetObjectData((ObjectType)type);

        if (data.maxLevel == data.level)
        {
            levelText[type].text = "Lv " + data.level.ToString();
            currentScoreText[type].text = data.currentScore.ToString();
            nextScoreText[type].text = data.currentScore.ToString();
            coinImage[type].SetActive(false);
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
