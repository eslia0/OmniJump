using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SkinMaster : MonoBehaviour
{
    private const string COIN_KEY = "COIN_FACTOR?!**";
    private const string KEY = "IS_UNLOCK?!*%";
    private const string BODY_KEY = "_Body";
    private const string FACE_KEY = "_Face";
    private const string TAIL_KEY = "_Tail";
    [SerializeField] private List<bool> accessList = new List<bool>();
    private void Init_ACCESSINFO()
    {
        if (accessList.Count > 0)
        {
            accessList.Clear();
        }

        if (!PlayerPrefs.HasKey("SKINMASTER"))
        {
            string skinAccess = "T";
            accessList.Add(true);

            for (int i = 1; i < xml.GetNodeCount(); i++)
            {
                skinAccess += "/F";
                accessList.Add(false);
            }
            PlayerPrefs.SetString("SKINMASTER", skinAccess);

            return;
        }
        else
        {
            string[] ac = PlayerPrefs.GetString("SKINMASTER").Split('/');

            for (int i = 0; i < xml.GetNodeCount(); i++)
            {
                if(i > ac.Length - 1)
                {
                    accessList.Add(false);
                }
                else
                {
                    accessList.Add((ac[i] == "T") ? true : false);
                }
            }
        }

        SetACCESS();
    }
    private void SetACCESS()
    {
        string s = "";
        for(int i = 0; i< accessList.Count; i++)
        {
            s += (accessList[i]) ? "T" : "F";
            if (i != accessList.Count - 1)
                s += "/";
        }

        PlayerPrefs.SetString("SKINMASTER", s);
    }

    private static SkinMaster instance;
    public static SkinMaster Instance {
        get {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<SkinMaster>();

                if (instance == null)
                {
                    GameObject managment = new GameObject("SKIN MANAGER");
                    instance = managment.AddComponent<SkinMaster>();
                }
            }

            return instance;
        }
    }

    [SerializeField] private static int COIN = 100;
    [SerializeField] private static float purchasMul = 100;
    public void Purchas_Reset(string key)
    {
        if(key == "RESET FOR ONCE!*@&#^&*(")
        {
            purchasMul = 100;
        }
    }
    public void Mul_Purchas()
    {
        purchasMul += 20f;
    }
    public int Get_Purchas()
    {
        return (int)purchasMul;
    }
    public int Get_Coin()
    {
        return COIN;
    }

    [SerializeField] private int SELECTION;
    public int GetSelection()
    {
        return SELECTION;
    }
    [SerializeField] private Sprite sprite999;
    public Sprite Get999()
    {
        return sprite999;
    }



    void Awake()
    {
        instance = FindObjectOfType<SkinMaster>();

        if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        Init_ACCESSINFO();

        if (!PlayerPrefs.HasKey("BodySkin"))
        {
            PlayerPrefs.SetInt("BodySkin", 0);
        }

        COIN = PlayerPrefs.GetInt("Coin");

        xml.LoadSkinXML(skinArray , accessList, lockArray, KEY, BODY_KEY, FACE_KEY, TAIL_KEY);
    }


    private XMLManager xml = new XMLManager();
    [System.Serializable] public class SkinInfo
    {
        public bool LOCK;

        public Sprite body;
        public Sprite face;

        public string tailState;

        public GameObject tailEffect;
        public Color tailColor;
    }
    [SerializeField] private static List<int> lockArray = new List<int>();
    [SerializeField] private static List<SkinInfo> skinArray = new List<SkinInfo>();
    public int Get_SkinArrayLength()
    {
        return skinArray.Count;
    }
    public bool Get_SkinItemLOCK(int i)
    {
        try
        {
            return skinArray[i].LOCK;
        }
        catch (Exception e)
        {
            return true;
        }
    }
    public SkinInfo Get_SkinInfo(int i)
    {
        if (skinArray[i].LOCK)
        {
            return skinArray[0];
        }

        return skinArray[i];
    }


    public bool UNLOCK()
    {
        if (!SceneManagement.Instance.UseCoin(Get_Purchas()))
        {
            Debug.Log("*********NOT ENOUGH MONEY*********");
            // 구매불가 메시지 표시
            return false;
        }

        instance.Mul_Purchas();

        int position = new System.Random().Next(0, lockArray.Count - 1);
        int selection = lockArray[position];

        SELECTION = selection;
        lockArray.RemoveAt(position);
        skinArray[selection].LOCK = false;
        accessList[selection] = true;

        SetACCESS();

        return true;
    }

    // 플레이어가 선택한 body와 effect 저장
    public void SetSkin(int body)
    {
        if(!instance.Get_SkinItemLOCK(body))
            PlayerPrefs.SetInt("BodySkin", (body < skinArray.Count) ? body : 0);
    }

    public GameObject SetTailEffect(GameObject tail, Color color)
    {
        GameObject tmp = GameObject.Instantiate(tail);

        // ColorOverTime 설정
        ParticleSystem.ColorOverLifetimeModule cot = tmp.GetComponent<ParticleSystem>().colorOverLifetime;

        Gradient gradient = new Gradient();
        gradient.SetKeys(new GradientColorKey[] { // 시간에 따라 변화하도록 설정
            new GradientColorKey(color, 0.0f), new GradientColorKey(Color.white, 1.0f) }, // 색상
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 0.75f), new GradientAlphaKey(0.0f, 1.0f) }); // 알파값

        cot.color = gradient;

        return tmp;
    }

}
