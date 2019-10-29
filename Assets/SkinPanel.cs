using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Button b_Purchas;


    private void Start()
    {
        int skinCount = 0;
        for (int i = -2; i < SkinMaster.Instance.Get_SkinArrayLength() + 2; i++)
        {
            GameObject tmp;

            if (i < 0 || i > SkinMaster.Instance.Get_SkinArrayLength() - 1)
            {
                tmp = Instantiate(blank);
            }
            else
            {
                int num = i;
                tmp = Instantiate(skinObject);

                if (!SkinMaster.Instance.Get_SkinItemLOCK(num))
                {
                    skinCount++;
                    SkinMaster.Instance.Mul_Purchas();
                    tmp.GetComponent<Image>().sprite = SkinMaster.Instance.Get_SkinInfo(num).body;
                }
                tmp.GetComponent<Button>().onClick.AddListener(() => { SetBodySkin(num); });
            }

            tmp.transform.SetParent(content);
            tmp.transform.localScale = Vector3.one;
            tmp.transform.localPosition = Vector3.zero;
        }

        if (skinCount == SkinMaster.Instance.Get_SkinArrayLength())
        {
            b_Purchas.gameObject.SetActive(false);
        }
        else
        {
            b_Purchas.onClick.AddListener(() => {
                SkinMaster.Instance.UNLOCK();
                t_Coin.text = SkinMaster.Instance.Get_Coin().ToString();
                t_Cost.text = SkinMaster.Instance.Get_Purchas().ToString();
            });
        }

        t_Coin.text = SkinMaster.Instance.Get_Coin().ToString();
        t_Cost.text = SkinMaster.Instance.Get_Purchas().ToString();

        StartCoroutine(EffectTrigger());
        SetPlayer();
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
                tailE.transform.localScale = Vector3.one * 2;
                tailE.transform.localPosition = Vector3.zero;

                ParticleSystem particle = tailE.GetComponent<ParticleSystem>();
                particle.startSpeed = -3f;
                ParticleSystem.EmissionModule emission = particle.emission;
                emission.rateOverTime = 5;
                ParticleSystem.ShapeModule shape = particle.shape;
                shape.arc = 0;

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
        while (true)
        {
            yield return new WaitForSeconds(2);

            face.rectTransform.Rotate(0,0,90);
        }
    }

    public void SetBodySkin(int skin)
    {
        SkinMaster.Instance.SetSkin(skin);
        SetPlayer();
    }
}
