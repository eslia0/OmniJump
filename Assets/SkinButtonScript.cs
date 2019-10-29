using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinButtonScript : MonoBehaviour
{
    [SerializeField] private Image playerBody;


    void OnEnable()
    {
        playerBody.sprite = SkinMaster.Instance.Get_SkinInfo(PlayerPrefs.GetInt("BodySkin")).body;
    }
}
