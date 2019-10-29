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
    [SerializeField] private static float purchasMul = 68;
    public void Mul_Purchas()
    {
        purchasMul *= 1.48f;
    }
    public int Get_Purchas()
    {
        return (int)purchasMul;
    }
    public int Get_Coin()
    {
        return COIN;
    }

    void Awake()
    {
        instance = FindObjectOfType<SkinMaster>();

        if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        if (!PlayerPrefs.HasKey("BodySkin"))
        {
            PlayerPrefs.SetInt("BodySkin", 0);
        }

        //COIN = PlayerPrefs.GetInt(COIN_KEY);

        xml.LoadSkinXML(skinArray, lockArray, KEY, BODY_KEY, FACE_KEY, TAIL_KEY);
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


    public void UNLOCK()
    {
        int coin = Get_Coin();
        //int coin = 1000;
        Debug.Log("COIN : " + Get_Coin() + ", Purchas : " + Get_Purchas());
        if (Get_Purchas() > Get_Coin())
        {
            Debug.Log("*********NOT ENOUGH MONEY*********");
            // 구매불가 메시지 표시
            return;
        }

        PlayerPrefs.SetInt(COIN_KEY, coin - (int)purchasMul);
        instance.Mul_Purchas();

        int position = new System.Random().Next(0, lockArray.Count - 1);
        int selection = lockArray[position];

        lockArray.RemoveAt(position);
        skinArray[selection].LOCK = false;
        xml.XMLWrite(KEY, selection);
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
