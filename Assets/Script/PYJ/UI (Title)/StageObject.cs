using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObject : MonoBehaviour
{
    SceneManagement sceneManagement;
    [SerializeField] int stageNum;
    [SerializeField] Transform stageImage;

    void Start()
    {
        sceneManagement = SceneManagement.Instance;

        if (stageNum <= sceneManagement.ClearStage)
        {
            stageImage.GetComponent<SpriteRenderer>().color = new Color(0.68f, 0.95f, 0.93f, 0.78f);
        }
        else
        {
            stageImage.GetComponent<SpriteRenderer>().color = new Color(0.17f, 0.44f, 0.42f, 0.78f);
        }
    }
}
