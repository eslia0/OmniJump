using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinPanel : MonoBehaviour
{
    [SerializeField] private Image body;
    [SerializeField] private Image face;
    [SerializeField] private Image tail;
    [SerializeField] private ParticleSystem particle;

    private IEnumerator effectRoutain;

    private void Start()
    {
        effectRoutain = EffectTrigger();
        StartCoroutine(effectRoutain);
        SetPlayer();
    }
    
    // Player 이미지 세팅
    private void SetPlayer()
    {
        //body.sprite = SceneManagement.Instance.GetPlayerBody();
        //SceneManagement.EffectInfo effect = SceneManagement.Instance.GetPlayerEffect();

        //face.sprite = effect.face;
        //particle = effect.hitEffect;
        //tail.color = effect.tailColor;
    }

    // 2초마다 이펙트 활성화 및 보는 방향 90도 전환
    private IEnumerator EffectTrigger()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);

            particle.Play();
            face.rectTransform.Rotate(0,0,90);
        }
    }

    public void StopRoutain()
    {
        StopCoroutine(effectRoutain);
    }

    public void SetBodySkin(int skin)
    {
        //SceneManagement.Instance.SetPlayerBody(skin);
        SetPlayer();
    }
}
