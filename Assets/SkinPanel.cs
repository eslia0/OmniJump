using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinPanel : MonoBehaviour
{
    public void SetBodySkin(Sprite skin)
    {
        Creater.Instance.playerinfo.body = skin;
    }

    public void SetEffectSkin(Sprite skin, GameObject effect)
    {
        Creater.Instance.playerinfo.face = skin;
        Creater.Instance.playerinfo.effect = skin;
    }
}
