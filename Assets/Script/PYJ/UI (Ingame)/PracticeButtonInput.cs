using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PracticeButtonInput : MonoBehaviour
{
    [SerializeField] Button[] buttons;
    Transform resultPanel;
    [SerializeField] private Sprite play;
    [SerializeField] private Sprite pause;

    void Start()
    {
        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(delegate () { Restart(); });
        buttons[1].onClick.RemoveAllListeners();
        buttons[1].onClick.AddListener(delegate () { ReturnTitle(); });
        buttons[2].onClick.RemoveAllListeners();
        buttons[2].onClick.AddListener(delegate () { Pause(); });

        resultPanel = transform.GetChild(3);
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

    private void Pause()
    {
        Creater.Instance.Pause();

        if (Creater.Instance.IsPaused)
        {
            buttons[2].GetComponent<Image>().sprite = play;
        }
        else
        {
            buttons[2].GetComponent<Image>().sprite = pause;
        }
    }
}
