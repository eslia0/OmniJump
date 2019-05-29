using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageButtonInput : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private GameObject resultPanel;

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
    }

    private void ToTitle()
    {
        SceneManager.sceneLoaded -= Creater.Instance.StartStage;
        Creater.Instance.Disable();

        SceneManagement.Instance.LoadScene("TitleScene");
    }
}
