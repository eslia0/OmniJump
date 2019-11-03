using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxCanvasBodyScript : MonoBehaviour
{
    [SerializeField] private Image image;

    private void OnEnable()
    {
        image.sprite = SkinMaster.Instance.Get999();
    }

    public void OnDisable()
    {
        image.sprite = SkinMaster.Instance.Get_SkinInfo(SkinMaster.Instance.GetSelection()).body;
    }
}
