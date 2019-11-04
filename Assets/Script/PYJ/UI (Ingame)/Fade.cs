﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    private Image[] fadeImages;
    public bool ended;
    public GameObject fadeCanvas;

    void Awake()
    {
        fadeCanvas = transform.parent.gameObject;

        if (SceneManagement.Instance.fade != this)
        {
            Destroy(fadeCanvas);
            return;
        }

        fadeImages = GetComponentsInChildren<Image>();
        DontDestroyOnLoad(transform.parent.gameObject);
    }

    public IEnumerator FadeOut()
    {
        ended = false;

        for (int i = 0; i < fadeImages.Length; i++)
        {
            fadeImages[i].rectTransform.localScale = new Vector3(0, 0, 1);
        }

        bool[] check = new bool[fadeImages.Length];
        int count = 0;
        int index = 0;
        int cycle = 0;

        while (count < fadeImages.Length)
        {
            while (cycle < 3) // 최대 3개의 사각형을 실행
            {
                if (index > fadeImages.Length - 1)
                {
                    index -= fadeImages.Length;
                    index++;
                }

                if (!check[index]) // 실행 되지 않은 사각형이면 실행
                {
                    check[index] = true;
                    fadeImages[index].GetComponent<Animation>().Play("FadeOut");
                    count++;
                    cycle++;
                }
                index += 7;
            }

            cycle = 0;
            
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        ended = true;
    }

    public IEnumerator FadeIn()
    {
        ended = false;

        bool[] check = new bool[fadeImages.Length];
        int count = 0;
        int index = 0;
        int cycle = 0;

        while (count < fadeImages.Length)
        {
            while (cycle < 3) // 최대 3개의 사각형을 실행
            {
                if (index > fadeImages.Length - 1)
                {
                    index -= fadeImages.Length - 1;
                }

                if (!check[index]) // 실행 되지 않은 사각형이면 실행
                {
                    check[index] = true;
                    fadeImages[index].GetComponent<Animation>().Play("FadeIn");
                    count++;
                    cycle++;
                }

                index += 7;
            }

            cycle = 0;

            yield return null;
        }
        
        ended = true;
    }
}
