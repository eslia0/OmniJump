using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxCanvasBodyScript : MonoBehaviour
{
    [SerializeField] private Image image;


    public void RestBody()
    {
        Debug.Log("Reset : " + SkinMaster.Instance.Get999());
        image.sprite = SkinMaster.Instance.Get999();
    }

    public void SetBody()
    {
        Debug.Log("Set");
        image.sprite = SkinMaster.Instance.Get_SkinInfo(SkinMaster.Instance.GetSelection()).body;
    }
}
