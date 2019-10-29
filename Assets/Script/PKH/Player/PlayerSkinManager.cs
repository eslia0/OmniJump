
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkinManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer body;
    [SerializeField] private SpriteRenderer face;
    [SerializeField] private Transform tailEffect;
    //[SerializeField] private Transform hitEffect;
    [SerializeField] private TrailRenderer tail;

    [Space(10)]

    [SerializeField] private Transform tailPivot;

    private GameObject tailParticle;
    private SkinMaster.SkinInfo skin;

    private void Start()
    {
        skin = SkinMaster.Instance.Get_SkinInfo(PlayerPrefs.GetInt("BodySkin"));

        if (skin.body != null)
            body.sprite = skin.body;
        if (skin.face != null)
            face.sprite = skin.face;

        switch (skin.tailState)
        {
            case "EFFECT":
                tail.enabled = false;

                tailParticle = Instantiate(SkinMaster.Instance.SetTailEffect(skin.tailEffect, skin.tailColor));
                tailParticle.transform.SetParent(tailEffect);
                tailParticle.transform.localScale = Vector3.one;
                tailParticle.transform.localPosition = Vector3.zero;
                break;

            case "COLOR":
                tail.enabled = true;

                tail.startColor = skin.tailColor;
                tail.endColor = skin.tailColor * new Color(1, 1, 1, 0.7f);
                break;
        }
    }

    public void FlipEffect(bool isFlip)
    {
        if(skin.tailState == "COLOR")
        {
            return;
        }

        int num = (isFlip) ? 180 : 0;
        tailPivot.localRotation = Quaternion.Euler(0, 0, num);
        tailParticle.GetComponent<ParticleSystem>().startRotation = num;
    }
}
