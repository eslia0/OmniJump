using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkinManager : MonoBehaviour
{
    private SceneManagement.SkinInfo skin;

    // Start is called before the first frame update
    void Start()
    {
        skin = SceneManagement.Instance.GetSkin();
    }
}
