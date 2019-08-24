using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageButtonInput : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private GameObject resultPanel;

    [SerializeField] private Image star;
    [SerializeField] private Sprite clearImage;
    [SerializeField] private Sprite failImage;

    private void Start()
    {
        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(delegate () { Creater.Instance.NextStage(0); });

        buttons[1].onClick.RemoveAllListeners();
        buttons[1].onClick.AddListener(delegate () { Creater.Instance.NextStage(1); });

        buttons[2].onClick.RemoveAllListeners();
        buttons[2].onClick.AddListener(delegate () { ToTitle(); });

        resultPanel.SetActive(false);
    }

    public void SetResultPanel()
    {
        resultPanel.SetActive(true);
        resultPanel.GetComponent<Animation>().Play("PanelApear");

        Image starImage = star.transform.GetChild(0).GetComponent<Image>();

        // 이번에 클리어
        if (SceneManagement.Instance.selectedStage == SceneManagement.Instance.ClearStage)
        {
            star.GetComponent<Animation>().Play("starAnim");
        }
        // 이미 클리어함
        else if (SceneManagement.Instance.selectedStage < SceneManagement.Instance.ClearStage)
        {
            buttons[1].interactable = true;
            starImage.sprite = clearImage;
        }
        // 클리어 실패 시
        else if (SceneManagement.Instance.selectedStage > SceneManagement.Instance.ClearStage)
        {
            buttons[1].interactable = false;
            starImage.sprite = failImage;
        }
    }

    private void ToTitle()
    {
        SceneManager.sceneLoaded -= Creater.Instance.StartStage;
        Creater.Instance.Disable();

        SceneManagement.Instance.LoadScene("TitleScene");
    }
}
