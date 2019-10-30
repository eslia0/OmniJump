using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PracticeUI : MonoBehaviour
{
    [SerializeField] Button[] buttons;
    Transform pausePanel;
    Transform resultPanel;
    
    [SerializeField] private ResultScore resultScore;

    void Start()
    {
        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(delegate () { Restart(); });
        buttons[1].onClick.RemoveAllListeners();
        buttons[1].onClick.AddListener(delegate () { ReturnTitle(); });
        buttons[2].onClick.RemoveAllListeners();
        buttons[2].onClick.AddListener(delegate () { Pause(); });
        buttons[3].onClick.RemoveAllListeners();
        buttons[3].onClick.AddListener(delegate () { Resume(); });
        buttons[4].onClick.RemoveAllListeners();
        buttons[4].onClick.AddListener(delegate () { Restart(); });
        buttons[5].onClick.RemoveAllListeners();
        buttons[5].onClick.AddListener(delegate () { ReturnTitle(); });

        pausePanel = transform.GetChild(3);
        resultPanel = transform.GetChild(4);
        pausePanel.gameObject.SetActive(false);
        resultPanel.gameObject.SetActive(false);
    }

    void Restart()
    {
        SceneManagement.Instance.StartCoroutine(SceneManagement.Instance.LoadScene("PracticeScene"));
    }

    void ReturnTitle()
    {
        Creater.Instance.player.enabled = false;
        Creater.Instance.Disable();
        SceneManager.sceneLoaded -= Creater.Instance.StartStage;
        SceneManagement.Instance.StartCoroutine(SceneManagement.Instance.LoadScene("TitleScene"));
    }

    public void SetResultPanel()
    {
        resultPanel.gameObject.SetActive(true);
        buttons[2].enabled = false;

        resultScore.StartCoroutine(resultScore.SetResultScore(Creater.Instance.Score));
    }

    private void Pause()
    {
        if (Creater.Instance.player.enabled)
        {
            Creater.Instance.Pause();
            buttons[2].enabled = false;
            pausePanel.gameObject.SetActive(true);
        }
    }

    private void Resume()
    {
        if (Creater.Instance.player.enabled)
        {
            Creater.Instance.Pause();
            buttons[2].enabled = true;
            pausePanel.gameObject.SetActive(false);
        }
    }
}
