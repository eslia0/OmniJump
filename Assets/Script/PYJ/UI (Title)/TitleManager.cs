﻿using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    private static TitleManager instance;
    public static TitleManager Instance {
        get {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<TitleManager>();

                if (instance == null)
                {
                    GameObject managment = new GameObject("TitleManager");
                    instance = managment.AddComponent<TitleManager>();
                }
            }

            return instance;
        }
    }

    private GameObject titlePanel;
    private GameObject stagePanel;
    private GameObject endlessPanel;

    Camera mainCam;

    [SerializeField] private Button[] buttons;
    [SerializeField] private Button[] levelButtons;
    private GameObject[] levelPanel;
    [SerializeField] private InteractionUI[] actions;
    private PlayerUIController playerUI;
    private Transform[] stages;
    private int selectedLevel;
    private int selected { get { return (selectedLevel / 10) * 3 + (selectedLevel % 10 + 2) / 3; } }

    private void Start()
    {
        InitTitle();
        InitStage();

        if (SceneManagement.Instance.prevScene == "StageScene")
        {
            ToStagePanel();
        }
    }

    public void InitTitle()
    {
        titlePanel = transform.GetChild(0).gameObject;
        stagePanel = transform.GetChild(1).gameObject;
        endlessPanel = transform.GetChild(2).gameObject;

        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(delegate () { ToStagePanel(); });
        buttons[1].onClick.RemoveAllListeners();
        buttons[1].onClick.AddListener(delegate () { ToEndlessPanel(); });
        buttons[2].onClick.RemoveAllListeners();
        buttons[2].onClick.AddListener(delegate () { StartEndless(); });
        buttons[3].onClick.RemoveAllListeners();
        buttons[3].onClick.AddListener(delegate () { ToTitlePanel(); });

        titlePanel.SetActive(true);
        stagePanel.SetActive(false);
        endlessPanel.SetActive(false);

        mainCam = Camera.main;
        mainCam.transform.position = new Vector3(-6, 0, -100);
    }

    private void InitStage()
    {
        playerUI = GameObject.Find("PlayerUI").GetComponent<PlayerUIController>();

        // 스테이지 UI버튼 설정
        levelPanel = new GameObject[stagePanel.transform.childCount];
        for (int i = 0; i < levelPanel.Length; i++)
        {
            levelPanel[i] = stagePanel.transform.GetChild(i).gameObject;

            if (i != 0)
            {
                levelPanel[i].SetActive(false);
            }
        }

        // 스테이지 내 오브젝트 설정
        GameObject stage = GameObject.Find("Stages");
        stages = new Transform[stage.transform.childCount];

        for (int i = 0; i < stages.Length; i++)
        {
            stages[i] = stage.transform.GetChild(i);
        }

        selectedLevel = 0;
        SetStage();
        playerUI.action.Add(actions[0]);
        levelPanel[0].SetActive(true);

        // 타이틀 UI 버튼 초기화
        levelButtons[0].onClick.RemoveAllListeners();
        levelButtons[1].onClick.RemoveAllListeners();
        levelButtons[2].onClick.RemoveAllListeners();
        levelButtons[3].onClick.RemoveAllListeners();
        levelButtons[4].onClick.RemoveAllListeners();
        levelButtons[5].onClick.RemoveAllListeners();
        levelButtons[6].onClick.RemoveAllListeners();
        levelButtons[7].onClick.RemoveAllListeners();

        levelButtons[0].onClick.AddListener(delegate () {
            ToTitle();
        });
        levelButtons[1].onClick.AddListener(delegate () {
            ToStageLevel(1);
        });
        levelButtons[2].onClick.AddListener(delegate () {
            ToStageLevel(11);
        });
        levelButtons[3].onClick.AddListener(delegate ()
        {
            ToStageLevel(21);
        });
        levelButtons[4].onClick.AddListener(delegate ()
        {
            ToStageLevel(31);
        });
        levelButtons[5].onClick.AddListener(delegate ()
        {
            ToStageLevel(41);
        });
        levelButtons[6].onClick.AddListener(delegate ()
        {
            ToStageLevel(51);
        });
        levelButtons[7].onClick.AddListener(delegate ()
        {
            ToStageLevel(61);
        });
    }

    // 스테이지 이동
    public void SetStage ()
    {
        // 캐릭터 위치 설정
        playerUI.transform.position = stages[selected].GetChild(1).position;
        actions = stages[selected].GetComponentsInChildren<InteractionUI>();

        for (int i = 0; i < actions.Length; i++)
        {
            if(actions[i].action == InteractionUI.UIInteraction.Moving)
            {
                actions[i].GetComponent<MovingPlatformUI>().Init();
            }
        }
    }

    // 스테이지 내 레벨 버튼 설정
    public void SetLevelButton()
    {
        Transform level = levelPanel[selected].transform;
        int length = level.childCount;
        Button button;

        button = level.GetChild(0).GetComponent<Button>();
        button.onClick.RemoveAllListeners();

        // Prev 버튼 설정
        if (selected % 3 == 1)
        {
            button.onClick.AddListener(delegate () {
                ToStageLevel(0);
            });
        }
        else
        {
            button.onClick.AddListener(delegate () {
                ToStageLevel(selectedLevel - 3);
            });
        }

        // 각각의 스테이지 버튼 설정
        for (int i = 1; i < length - 1; i++)
        {
            button = level.GetChild(i).GetComponent<Button>();
            int num = selectedLevel + i - 1;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate () {
                ToLevel(num);
            });
            
            if (num > SceneManagement.Instance.ClearStage + 1)
            {
                button.enabled = false;
            }
        }

        // Next버튼 설정
        button = level.GetChild(length - 1).GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(delegate ()
        {
            ToStageLevel(selectedLevel + 3);
        });

        if (length - 1 > SceneManagement.Instance.ClearStage + 1)
        {
            button.enabled = false;
        }
    }

    // 타이틀
    private void ToTitlePanel()
    {
        titlePanel.SetActive(true);
        stagePanel.SetActive(false);
        endlessPanel.SetActive(false);
    }

    // 스테이지
    private void ToStagePanel()
    {
        titlePanel.SetActive(false);
        stagePanel.SetActive(true);

        mainCam.transform.position = new Vector3(0, 0, -100);
    }

    // 스테이지에 타이틀로 돌아가기
    private void ToTitle()
    {
        titlePanel.SetActive(true);
        stagePanel.SetActive(false);

        mainCam.transform.position = new Vector3(-6, 0, -100);
    }

    // 엔드리스
    private void ToEndlessPanel()
    {
        titlePanel.SetActive(false);
        endlessPanel.SetActive(true);
    }
    
    // 엔드리스 시작
    private void StartEndless()
    {
        SceneManagement.Instance.LoadScene("EndlessScene");
    }

    // 각 레벨 선택 단계로 이동
    public void ToStageLevel(int level)
    {
        if (playerUI.action.Count > 0)
        {
            return;
        }

        if (level == 0)
        {
            StartCoroutine(ToNext(0));
            return;
        }
        else if (selectedLevel == 0)
        {
            if (level == 1)
            {
                playerUI.action.Add(actions[1]);
            }
            else if (level == 11)
            {
                playerUI.action.Add(actions[4]);
            }
            else if (level == 21)
            {
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[5]);
            }
            else if (level == 31)
            {
                playerUI.action.Add(actions[6]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[8]);
            }
            else if (level == 41)
            {
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[9]);
                playerUI.action.Add(actions[10]);
                playerUI.action.Add(actions[11]);
            }
            else if (level == 51)
            {
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[9]);
                playerUI.action.Add(actions[10]);
                playerUI.action.Add(actions[12]);
            }
            else if (level == 61)
            {
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[9]);
                playerUI.action.Add(actions[10]);
                playerUI.action.Add(actions[13]);
                playerUI.action.Add(actions[14]);
                playerUI.action.Add(actions[15]);
            }
            else if (level == 71)
            {
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[9]);
                playerUI.action.Add(actions[10]);
                playerUI.action.Add(actions[13]);
                playerUI.action.Add(actions[14]);
                playerUI.action.Add(actions[16]);
                playerUI.action.Add(actions[17]);
                playerUI.action.Add(actions[18]);
            }
        }
        else if (selectedLevel == 1 && level == 4)
        {
            playerUI.action.Add(actions[2]);
            playerUI.action.Add(actions[3]);
            playerUI.action.Add(actions[6]);
        }
        else if (selectedLevel == 4 && level == 7)
        {
            playerUI.action.Add(actions[6]);
        }
        // 1레벨 마지막
        else if (selectedLevel == 7 && level == 10)
        {
            playerUI.action.Add(actions[6]);
            playerUI.action.Add(actions[7]);
            playerUI.action.Add(actions[9]);
            
            StartCoroutine(ToNext(0));
            return;
        }
        else if (selectedLevel == 11 && level == 14)
        {
            playerUI.action.Add(actions[4]);
            playerUI.action.Add(actions[2]);
            playerUI.action.Add(actions[5]);
            playerUI.action.Add(actions[7]);
        }
        else if (selectedLevel == 14 && level == 17)
        {
            playerUI.action.Add(actions[2]);
            playerUI.action.Add(actions[3]);
            playerUI.action.Add(actions[5]);
            playerUI.action.Add(actions[7]);
        }
        // 2레벨 마지막
        else if (selectedLevel == 17 && level == 20)
        {
            playerUI.action.Add(actions[1]);
            playerUI.action.Add(actions[2]);
            playerUI.action.Add(actions[8]);
            playerUI.action.Add(actions[10]);
            playerUI.action.Add(actions[11]);
            playerUI.action.Add(actions[12]);
            playerUI.action.Add(actions[14]);
            
            StartCoroutine(ToNext(0));
            return;
        }
        else if (selectedLevel == 21 && level == 24)
        {
            playerUI.action.Add(actions[5]);
        }
        else if (selectedLevel == 24 && level == 27)
        {
            playerUI.action.Add(actions[2]);
            playerUI.action.Add(actions[3]);
            playerUI.action.Add(actions[5]);
        }
        // 3레벨 마지막
        else if (selectedLevel == 27 && level == 30)
        {
            playerUI.action.Add(actions[2]);
            playerUI.action.Add(actions[3]);
            playerUI.action.Add(actions[4]);
            playerUI.action.Add(actions[5]);
            playerUI.action.Add(actions[7]);
            playerUI.action.Add(actions[8]);
            playerUI.action.Add(actions[9]);
            playerUI.action.Add(actions[10]);
            playerUI.action.Add(actions[12]);
            playerUI.action.Add(actions[13]);
            playerUI.action.Add(actions[14]);
            playerUI.action.Add(actions[15]);
            playerUI.action.Add(actions[17]);
            
            StartCoroutine(ToNext(0));
            return;
        }
        else if (selectedLevel == 31 && level == 34)
        {
            playerUI.action.Add(actions[6]);
        }
        else if (selectedLevel == 34 && level == 37)
        {
            playerUI.action.Add(actions[3]);
            playerUI.action.Add(actions[4]);
            playerUI.action.Add(actions[5]);
            playerUI.action.Add(actions[6]);
            playerUI.action.Add(actions[8]);
            playerUI.action.Add(actions[9]);
            playerUI.action.Add(actions[10]);
            playerUI.action.Add(actions[11]);
            playerUI.action.Add(actions[12]);
        }
        // 4레벨 마지막
        else if (selectedLevel == 37 && level == 40)
        {
            playerUI.action.Add(actions[1]);
            playerUI.action.Add(actions[2]);
            playerUI.action.Add(actions[4]);
            playerUI.action.Add(actions[5]);
            playerUI.action.Add(actions[6]);
            playerUI.action.Add(actions[7]);
            playerUI.action.Add(actions[9]);
            playerUI.action.Add(actions[10]);
            playerUI.action.Add(actions[11]);
            playerUI.action.Add(actions[12]);
            playerUI.action.Add(actions[13]);
            playerUI.action.Add(actions[14]);
            playerUI.action.Add(actions[15]);
            playerUI.action.Add(actions[17]);
            playerUI.action.Add(actions[18]);
            playerUI.action.Add(actions[20]);
            
            StartCoroutine(ToNext(0));
            return;
        }
        else if (selectedLevel == 41 && level == 44)
        {
            playerUI.action.Add(actions[2]);
            playerUI.action.Add(actions[3]);
            playerUI.action.Add(actions[4]);
            playerUI.action.Add(actions[5]);
            playerUI.action.Add(actions[6]);
            playerUI.action.Add(actions[7]);
            playerUI.action.Add(actions[8]);
            playerUI.action.Add(actions[9]);
            playerUI.action.Add(actions[14]);
        }
        else if (selectedLevel == 44 && level == 47)
        {
            playerUI.action.Add(actions[1]);
            playerUI.action.Add(actions[3]);
            playerUI.action.Add(actions[4]);
            playerUI.action.Add(actions[5]);
            playerUI.action.Add(actions[6]);
            playerUI.action.Add(actions[7]);
            playerUI.action.Add(actions[10]);
            playerUI.action.Add(actions[11]);
            playerUI.action.Add(actions[13]);
            playerUI.action.Add(actions[14]);
            playerUI.action.Add(actions[15]);
        }
        // 5레벨 마지막
        else if (selectedLevel == 47 && level == 50)
        {
            playerUI.action.Add(actions[2]);
            playerUI.action.Add(actions[3]);
            playerUI.action.Add(actions[14]);

            StartCoroutine(ToNext(0));
            return;
        }
        else if (selectedLevel == 51 && level == 54)
        {
            playerUI.action.Add(actions[2]);
            playerUI.action.Add(actions[3]);
            playerUI.action.Add(actions[4]);
            playerUI.action.Add(actions[5]);
            playerUI.action.Add(actions[7]);
            playerUI.action.Add(actions[8]);
            playerUI.action.Add(actions[9]);
            playerUI.action.Add(actions[10]);
            playerUI.action.Add(actions[11]);
            playerUI.action.Add(actions[5]);
            playerUI.action.Add(actions[13]);
        }
        else if (selectedLevel == 54 && level == 57)
        {
            playerUI.action.Add(actions[1]);
            playerUI.action.Add(actions[2]);
            playerUI.action.Add(actions[4]);
            playerUI.action.Add(actions[5]);
            playerUI.action.Add(actions[6]);
            playerUI.action.Add(actions[8]);
            playerUI.action.Add(actions[13]);
        }
        else if (selectedLevel == 57 && level == 60)
        {
            playerUI.action.Add(actions[1]);
            playerUI.action.Add(actions[2]);
            playerUI.action.Add(actions[4]);
            playerUI.action.Add(actions[5]);
            playerUI.action.Add(actions[14]);

            StartCoroutine(ToNext(0));
            return;
        }
        else if (selectedLevel == 61 && level == 64)
        {
            playerUI.action.Add(actions[1]);
            playerUI.action.Add(actions[2]);
            playerUI.action.Add(actions[3]);
            playerUI.action.Add(actions[4]);
            playerUI.action.Add(actions[5]);
            playerUI.action.Add(actions[7]);
            playerUI.action.Add(actions[8]);
            playerUI.action.Add(actions[9]);
            playerUI.action.Add(actions[10]);
            playerUI.action.Add(actions[13]);
        }
        else if (selectedLevel == 64 && level == 67)
        {
            playerUI.action.Add(actions[1]);
            playerUI.action.Add(actions[2]);
            playerUI.action.Add(actions[3]);
            playerUI.action.Add(actions[4]);
            playerUI.action.Add(actions[5]);
            playerUI.action.Add(actions[6]);
            playerUI.action.Add(actions[7]);
            playerUI.action.Add(actions[8]);
            playerUI.action.Add(actions[9]);
            playerUI.action.Add(actions[10]);
            playerUI.action.Add(actions[11]);
            playerUI.action.Add(actions[12]);
            playerUI.action.Add(actions[13]);
            playerUI.action.Add(actions[14]);
            playerUI.action.Add(actions[15]);
            playerUI.action.Add(actions[16]);
            playerUI.action.Add(actions[17]);
            playerUI.action.Add(actions[18]);
            playerUI.action.Add(actions[19]);
            playerUI.action.Add(actions[20]);
            playerUI.action.Add(actions[21]);
            playerUI.action.Add(actions[22]);
        }
        else if (selectedLevel == 67 && level == 70)
        {
            playerUI.action.Add(actions[22]);
            playerUI.action.Add(actions[23]);
            playerUI.action.Add(actions[2]);
            playerUI.action.Add(actions[6]);
            playerUI.action.Add(actions[10]);
            playerUI.action.Add(actions[14]);
            playerUI.action.Add(actions[18]);
            playerUI.action.Add(actions[25]);
            playerUI.action.Add(actions[26]);
            playerUI.action.Add(actions[3]);
            playerUI.action.Add(actions[7]);
            playerUI.action.Add(actions[11]);
            playerUI.action.Add(actions[15]);
            playerUI.action.Add(actions[19]);
            playerUI.action.Add(actions[28]);
            playerUI.action.Add(actions[29]);
            playerUI.action.Add(actions[4]);
            playerUI.action.Add(actions[8]);
            playerUI.action.Add(actions[12]);
            playerUI.action.Add(actions[16]);
            playerUI.action.Add(actions[20]);
            playerUI.action.Add(actions[31]);
            playerUI.action.Add(actions[32]);
            playerUI.action.Add(actions[5]);
            playerUI.action.Add(actions[9]);
            playerUI.action.Add(actions[13]);
            playerUI.action.Add(actions[17]);
            playerUI.action.Add(actions[21]);
            playerUI.action.Add(actions[33]);

            StartCoroutine(ToNext(0));
            return;
        }

        StartCoroutine(ToNext(level));
    }

    // 레벨 입장
    private void ToLevel(int level)
    {
        if (playerUI.action.Count > 0)
        {
            return;
        }

        switch (level)
        {
            default:
                break;

            case 1:
                playerUI.action.Add(actions[1]);
                break;

            case 2:
                playerUI.action.Add(actions[4]);
                break;

            case 3:
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[5]);
                break;

            case 4:
                playerUI.action.Add(actions[1]);
                break;

            case 5:
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                break;

            case 6:
                playerUI.action.Add(actions[5]);
                break;

            case 7:
                playerUI.action.Add(actions[1]);
                break;

            case 8:
                playerUI.action.Add(actions[2]);
                break;

            case 9:
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                break;

            case 10:
                playerUI.action.Add(actions[6]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[8]);
                break;

            case 11:
                playerUI.action.Add(actions[1]);
                break;

            case 12:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                break;

            case 13:
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[6]);
                break;

            case 14:
                playerUI.action.Add(actions[1]);
                break;

            case 15:
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                break;

            case 16:
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[6]);
                break;

            case 17:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                break;

            case 18:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[6]);
                playerUI.action.Add(actions[7]);
                break;

            case 19:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[8]);
                playerUI.action.Add(actions[9]);
                break;

            case 20:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[8]);
                playerUI.action.Add(actions[10]);
                playerUI.action.Add(actions[11]);
                playerUI.action.Add(actions[12]);
                playerUI.action.Add(actions[13]);
                break;

            case 21:
                playerUI.action.Add(actions[1]);
                break;

            case 22:
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                break;

            case 23:
                playerUI.action.Add(actions[4]);
                break;

            case 24:
                playerUI.action.Add(actions[1]);
                break;

            case 25:
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[1]);
                break;

            case 26:
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                break;

            case 27:
                playerUI.action.Add(actions[1]);
                break;

            case 28:
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[6]);
                break;

            case 29:
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[8]);
                playerUI.action.Add(actions[9]);
                playerUI.action.Add(actions[10]);
                playerUI.action.Add(actions[11]);
                break;

            case 30:
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[8]);
                playerUI.action.Add(actions[9]);
                playerUI.action.Add(actions[10]);
                playerUI.action.Add(actions[12]);
                playerUI.action.Add(actions[13]);
                playerUI.action.Add(actions[14]);
                playerUI.action.Add(actions[15]);
                playerUI.action.Add(actions[16]);
                break;

            case 31:
                playerUI.action.Add(actions[1]);
                break;

            case 32:
                playerUI.action.Add(actions[2]);
                break;

            case 33:
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                break;

            case 34:
                playerUI.action.Add(actions[1]);
                break;

            case 35:
                playerUI.action.Add(actions[2]);
                break;

            case 36:
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[6]);
                playerUI.action.Add(actions[7]);
                break;

            case 37:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                break;

            case 38:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[6]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[8]);
                break;

            case 39:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[6]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[9]);
                playerUI.action.Add(actions[10]);
                playerUI.action.Add(actions[11]);
                playerUI.action.Add(actions[12]);
                playerUI.action.Add(actions[13]);
                playerUI.action.Add(actions[14]);
                playerUI.action.Add(actions[15]);
                playerUI.action.Add(actions[16]);
                break;

            case 40:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[6]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[9]);
                playerUI.action.Add(actions[10]);
                playerUI.action.Add(actions[11]);
                playerUI.action.Add(actions[12]);
                playerUI.action.Add(actions[13]);
                playerUI.action.Add(actions[14]);
                playerUI.action.Add(actions[15]);
                playerUI.action.Add(actions[17]);
                playerUI.action.Add(actions[18]);
                playerUI.action.Add(actions[19]);
                break;

            case 41:
                playerUI.action.Add(actions[1]);
                break;

            case 42:
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[6]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[8]);
                playerUI.action.Add(actions[9]);
                playerUI.action.Add(actions[10]);
                break;

            case 43:
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[6]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[8]);
                playerUI.action.Add(actions[9]);
                playerUI.action.Add(actions[11]);
                playerUI.action.Add(actions[12]);
                playerUI.action.Add(actions[13]);
                break;

            case 44:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[2]);
                break;

            case 45:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[6]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[8]);
                break;

            case 46:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[6]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[9]);
                playerUI.action.Add(actions[11]);
                playerUI.action.Add(actions[12]);
                break;

            case 47:
                playerUI.action.Add(actions[1]);
                break;

            case 48:
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                break;

            case 49:
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[6]);
                break;

            case 50:
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[8]);
                playerUI.action.Add(actions[9]);
                playerUI.action.Add(actions[10]);
                playerUI.action.Add(actions[11]);
                playerUI.action.Add(actions[12]);
                playerUI.action.Add(actions[13]);
                break;

            case 51:
                playerUI.action.Add(actions[1]);
                break;

            case 52:
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[6]);
                break;

            case 53:
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[8]);
                playerUI.action.Add(actions[9]);
                playerUI.action.Add(actions[10]);
                playerUI.action.Add(actions[11]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[12]);
                break;

            case 54:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                break;

            case 55:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[9]);
                break;

            case 56:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[8]);
                playerUI.action.Add(actions[10]);
                playerUI.action.Add(actions[11]);
                playerUI.action.Add(actions[12]);
                break;

            case 57:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                break;

            case 58:
                playerUI.action.Add(actions[6]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[8]);
                playerUI.action.Add(actions[9]);
                break;

            case 59:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[12]);
                break;

            case 60:
                playerUI.action.Add(actions[6]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[8]);
                playerUI.action.Add(actions[10]);
                playerUI.action.Add(actions[11]);
                playerUI.action.Add(actions[13]);
                break;

            case 61:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[6]);
                break;

            case 62:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[8]);
                playerUI.action.Add(actions[9]);
                playerUI.action.Add(actions[10]);
                playerUI.action.Add(actions[11]);
                break;

            case 63:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[8]);
                playerUI.action.Add(actions[9]);
                playerUI.action.Add(actions[10]);
                playerUI.action.Add(actions[12]);
                break;

            case 64:
                playerUI.action.Add(actions[1]);
                break;

            case 65:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[6]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[8]);
                playerUI.action.Add(actions[9]);
                playerUI.action.Add(actions[10]);
                break;

            case 66:
                playerUI.action.Add(actions[1]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[5]);
                playerUI.action.Add(actions[6]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[8]);
                playerUI.action.Add(actions[9]);
                playerUI.action.Add(actions[10]);
                playerUI.action.Add(actions[11]);
                playerUI.action.Add(actions[12]);
                playerUI.action.Add(actions[13]);
                playerUI.action.Add(actions[14]);
                playerUI.action.Add(actions[15]);
                playerUI.action.Add(actions[16]);
                playerUI.action.Add(actions[17]);
                playerUI.action.Add(actions[18]);
                playerUI.action.Add(actions[19]);
                break;

            case 67:
                playerUI.action.Add(actions[1]);
                break;

            case 68:
                playerUI.action.Add(actions[22]);
                playerUI.action.Add(actions[23]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[6]);
                playerUI.action.Add(actions[10]);
                playerUI.action.Add(actions[14]);
                playerUI.action.Add(actions[18]);
                playerUI.action.Add(actions[24]);
                break;

            case 69:
                playerUI.action.Add(actions[22]);
                playerUI.action.Add(actions[23]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[6]);
                playerUI.action.Add(actions[10]);
                playerUI.action.Add(actions[14]);
                playerUI.action.Add(actions[18]);
                playerUI.action.Add(actions[25]);
                playerUI.action.Add(actions[26]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[11]);
                playerUI.action.Add(actions[15]);
                playerUI.action.Add(actions[19]);
                playerUI.action.Add(actions[27]);
                break;

            case 70:
                playerUI.action.Add(actions[22]);
                playerUI.action.Add(actions[23]);
                playerUI.action.Add(actions[2]);
                playerUI.action.Add(actions[6]);
                playerUI.action.Add(actions[10]);
                playerUI.action.Add(actions[14]);
                playerUI.action.Add(actions[18]);
                playerUI.action.Add(actions[25]);
                playerUI.action.Add(actions[26]);
                playerUI.action.Add(actions[3]);
                playerUI.action.Add(actions[7]);
                playerUI.action.Add(actions[11]);
                playerUI.action.Add(actions[15]);
                playerUI.action.Add(actions[19]);
                playerUI.action.Add(actions[28]);
                playerUI.action.Add(actions[29]);
                playerUI.action.Add(actions[4]);
                playerUI.action.Add(actions[8]);
                playerUI.action.Add(actions[12]);
                playerUI.action.Add(actions[16]);
                playerUI.action.Add(actions[20]);
                playerUI.action.Add(actions[30]);
                break;
        }

        StartCoroutine(StartStage(level));
    }

    // 레벨 단계 이동
    public IEnumerator ToNext(int level)
    {
        while (playerUI.action.Count > 0)
        {
            yield return null;
        }
        
        levelPanel[selected].SetActive(false);

        selectedLevel = level;
        levelPanel[selected].SetActive(true);
        mainCam.transform.position = new Vector3(5.6f * selected, 0, -100);

        SetStage();
        GameObject effect = Instantiate(Resources.Load<GameObject>("Effects/Poof 1"), playerUI.transform.position, Quaternion.identity);
        Destroy(effect, 1.0f);

        playerUI.revertGravity = false;
        playerUI.moveRight = true;
        playerUI.action.Add(actions[0]);

        if (selectedLevel != 0)
        {
            SetLevelButton();
        }
    }
    
    public IEnumerator StartStage(int level)
    {
        while (playerUI.action.Count > 0)
        {
            yield return null;
        }
        
        SceneManagement.Instance.SelectStage(level);

        yield return new WaitForSeconds(0.2f);
        SceneManagement.Instance.LoadScene("StageScene");
    }
}
