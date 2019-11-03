using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Timeline;


public class SkinPanel : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject blank;
    [SerializeField] private GameObject skinObject;

    [Space(10)]

    [SerializeField] private Image body;
    [SerializeField] private Image face;
    [SerializeField] private Image tail;
    [SerializeField] private Transform tailEffect;

    [Space(10)]

    [SerializeField] private Text t_Coin;
    [SerializeField] private Text t_Cost;
    [SerializeField] private Text t_SkinCount;
    [SerializeField] private Button b_Purchas;

    [Space(10)]

    [SerializeField] private PlayableDirector director;
    [SerializeField] private BoxCanvasBodyScript box_Body;


    private bool routain = false;
    private int skinCount = 0;


    private void OnEnable()
    {
        t_Coin.text = SkinMaster.Instance.Get_Coin().ToString();
        t_Cost.text = SkinMaster.Instance.Get_Purchas().ToString();
        t_SkinCount.text = SkinMaster.Instance.Get_Purchas().ToString();
    }

    private void Start()
    {
        //PlayerPrefs.DeleteAll();
        SetPanel();

        b_Purchas.onClick.AddListener(() => {
            if (SkinMaster.Instance.UNLOCK())
            {
                t_Coin.text = SkinMaster.Instance.Get_Coin().ToString();
                t_Cost.text = SkinMaster.Instance.Get_Purchas().ToString();
                t_SkinCount.text = SkinMaster.Instance.Get_Purchas().ToString();

                if (skinCount == SkinMaster.Instance.Get_SkinArrayLength())
                {
                    b_Purchas.gameObject.SetActive(false);
                }

                SetPanel();
                director.time = 0;
                director.Play();
            }
        });

        StartCoroutine(EffectTrigger());
        SetPlayer();
    }

    private void SetPanel()
    {
        if(content.childCount > 0)
        {
            foreach (Transform child in content.transform)
            {
                Destroy(child.gameObject);
            }
        }

        skinCount = 0;
        int skinLength = SkinMaster.Instance.Get_SkinArrayLength();
        for (int i = -2; i < skinLength + 2; i++)
        {
            GameObject tmp;

            if (i < 0 || i > skinLength - 1)
            {
                tmp = Instantiate(blank);

                tmp.transform.SetParent(content);
                tmp.transform.localScale = Vector3.one;
                tmp.transform.localPosition = Vector3.zero;
            }
            else
            {
                int num = i;
                if (!SkinMaster.Instance.Get_SkinItemLOCK(num))
                {
                    tmp = Instantiate(skinObject);

                    skinCount++;
                    tmp.GetComponent<Image>().sprite = SkinMaster.Instance.Get_SkinInfo(num).body;
                    tmp.GetComponent<Button>().onClick.AddListener(() => { SetBodySkin(num); });

                    tmp.transform.SetParent(content);
                    tmp.transform.localScale = Vector3.one;
                    tmp.transform.localPosition = Vector3.zero;
                }
            }
        }
        
        if (skinCount == SkinMaster.Instance.Get_SkinArrayLength())
        {
            b_Purchas.gameObject.SetActive(false);
        }

        t_Coin.text = SceneManagement.Instance.coin.ToString();
        t_Cost.text = SkinMaster.Instance.Get_Purchas().ToString();
        t_SkinCount.text = skinCount + "/" + SkinMaster.Instance.Get_SkinArrayLength().ToString();
    }

    // Player 이미지 세팅
    private void SetPlayer()
    {
        SkinMaster.SkinInfo skin = SkinMaster.Instance.Get_SkinInfo(PlayerPrefs.GetInt("BodySkin"));

        body.sprite = skin.body;
        face.sprite = skin.face;

        if (tailEffect.childCount > 0)
        {
            Destroy(tailEffect.transform.GetChild(0).gameObject);
        }

        switch (skin.tailState)
        {
            case "EFFECT":
                tail.enabled = false;

                GameObject tailE = Instantiate(SkinMaster.Instance.SetTailEffect(skin.tailEffect, skin.tailColor));
                tailE.transform.SetParent(tailEffect);
                tailE.transform.localScale = Vector3.one;
                tailE.transform.localPosition = Vector3.zero;

                ParticleSystem particle = tailE.GetComponent<ParticleSystem>();
                ParticleSystem.EmissionModule emission = particle.emission;
                ParticleSystem.ShapeModule shape = particle.shape;

                switch (skin.tailEffect.name)
                {
                    case "0_Tail":

                        particle.startSpeed = -250f;
                        particle.startSize = 100;
                        emission.rateOverTime = 5;
                        shape.arc = 0;

                        break;
                    case "1_Tail":

                        particle.startSpeed = -250f;
                        particle.startSize = 100;
                        emission.rateOverTime = 5;
                        shape.arc = 0;

                        break;
                    case "2_Tail":

                        particle.startSpeed = -250f;
                        particle.startSize = 100;
                        emission.rateOverTime = 5;
                        shape.arc = 0;

                        break;
                    case "3_Tail":

                        particle.startSpeed = -250f;
                        particle.startSize = 100;
                        emission.rateOverTime = 5;
                        shape.arc = 0;

                        break;
                }

                break;

            case "COLOR":
                tail.enabled = true;

                tail.color = skin.tailColor;
                break;
        }
    }

    // 2초마다 이펙트 활성화 및 보는 방향 90도 전환
    private IEnumerator EffectTrigger()
    {
        routain = true;
        while (true)
        {
            yield return new WaitForSeconds(2);

            face.rectTransform.Rotate(0,0,90);
        }
        routain = false;
    }

    public void SetBodySkin(int skin)
    {
        SkinMaster.Instance.SetSkin(skin);
        SetPlayer();
    }
}
