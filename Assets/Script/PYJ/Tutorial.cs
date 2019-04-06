using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Tutorial : MonoBehaviour
{
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        string path = Application.persistentDataPath;

        if (!File.Exists(path + "/data01.bin"))
        {
            File.WriteAllBytes(path + "/data01.bin", new byte[1]);
            text.text = "튜토리얼";
        }
        else
        {
            text.text = "튜토 스킵";
        }
    }
}
