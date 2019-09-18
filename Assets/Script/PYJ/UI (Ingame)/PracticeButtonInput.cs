using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PracticeButtonInput : MonoBehaviour
{
    [SerializeField] Button[] buttons;
    Transform resultPanel;

    void Start()
    {
        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(delegate () { Restart(); });
        buttons[1].onClick.RemoveAllListeners();
        buttons[1].onClick.AddListener(delegate () { ReturnTitle(); });

        resultPanel = transform.GetChild(2);
        resultPanel.gameObject.SetActive(false);
    }

    void Restart()
    {
        SceneManagement.Instance.LoadScene("PracticeScene");
    }

    void ReturnTitle()
    {
        Creater.Instance.Disable();
        SceneManager.sceneLoaded -= Creater.Instance.StartStage;
        SceneManagement.Instance.LoadScene("TitleScene");
    }

    public void SetResultPanel()
    {
        resultPanel.gameObject.SetActive(true);
    }
}
