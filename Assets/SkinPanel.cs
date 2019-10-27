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

    [SerializeField] private float purchasMul = 1;

    private void Start()
    {
        for (int i = -2; i < SceneManagement.Instance.GetSkinArray().Count + 2; i++)
        {
            GameObject tmp;

            if (i < 0 || i > SceneManagement.Instance.GetSkinArray().Count -1)
            {
                tmp = Instantiate(blank);
            }
            else
            {
                int num = i;
                tmp = Instantiate(skinObject);
                tmp.GetComponent<Image>().sprite = Resources.Load<Sprite>(SceneManagement.Instance.GetSkinArray()[num].body);

                if (!SceneManagement.Instance.GetSkinArray()[num].LOCK)
                {
                    purchasMul *= 1.4f;
                    tmp.GetComponent<Button>().onClick.AddListener(() => { SetBodySkin(num); });
                }
            }

            tmp.transform.SetParent(content);
            tmp.transform.localScale = Vector3.one;
            tmp.transform.localPosition = Vector3.zero;
        }

        StartCoroutine(EffectTrigger());
        SetPlayer();
    }

    // Player 이미지 세팅
    private void SetPlayer()
    {
        SceneManagement.SkinInfo skin = SceneManagement.Instance.GetSkin();

        body.sprite = SceneManagement.Instance.GetBody();
        face.sprite = SceneManagement.Instance.GetFace();

        if (tailEffect.childCount > 0)
        {
            Destroy(tailEffect.transform.GetChild(0).gameObject);
        }

        switch (skin.tailState)
        {
            case SceneManagement.SkinTailState.Effect:
                tail.enabled = false;

                GameObject tailE = Instantiate(SceneManagement.Instance.SetTailEffect(skin.tailEffect, skin.tailColor));
                tailE.transform.SetParent(tailEffect);
                tailE.transform.localScale = Vector3.one * 150;
                tailE.transform.localPosition = Vector3.zero;

                ParticleSystem particle = tailE.GetComponent<ParticleSystem>();
                particle.startSpeed = -3f;
                ParticleSystem.EmissionModule emission = particle.emission;
                emission.rateOverTime = 5;
                ParticleSystem.ShapeModule shape = particle.shape;
                shape.arc = 0;

                break;

            case SceneManagement.SkinTailState.Color:
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
        SceneManagement.Instance.SetSkin(skin);
        SetPlayer();
    }

    public void PurchasSkin()
    {
        if(PlayerPrefs.GetInt() > purchasMul)
    }
}
