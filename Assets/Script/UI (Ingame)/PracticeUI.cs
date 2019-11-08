using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PracticeUI : MonoBehaviour
{
    [SerializeField] Button[] buttons;
    Transform pausePanel;
    Transform resultPanel;

    [SerializeField] private Text stage;
    [SerializeField] private Text resultScoreText;

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

        stage.text = "Stage " + (SceneManagement.Instance.selectedStage+1).ToString();

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
        if (Creater.Instance)
        {
            Creater.Instance.player.enabled = false;
            Creater.Instance.Disable();
            SceneManager.sceneLoaded -= Creater.Instance.StartStage;
            SceneManagement.Instance.StartCoroutine(SceneManagement.Instance.LoadScene("TitleScene"));
        }
    }

    public void SetResultPanel()
    {
        resultPanel.gameObject.SetActive(true);
        buttons[2].enabled = false;

        StartCoroutine(SetResultScore(Creater.Instance.score));
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

    public IEnumerator SetResultScore(int score) {
        int rScore = 0;
        int amount = (score - rScore) / 10;

        while (rScore < score) {
            rScore += amount;
            resultScoreText.text = rScore.ToString();
            yield return new WaitForSeconds(0.1f);
        }

        rScore = score;
        resultScoreText.text = rScore.ToString();
    }
}
