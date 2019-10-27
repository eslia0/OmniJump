using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public sealed class XMLManager : MonoBehaviour
{
    public class SKIN_INFO
    {
        public string LOCK;
        public string NAME;
        public string BODY;
        public string FACE;
        public string CATEGORY;
        public string TAIL_EFFECT;
        public string COLOR;
    }

    private static XMLManager instance = null;
    public static XMLManager S {
        get {
            if(instance == null)
            {
                instance = new XMLManager();
            }

            return instance;
        }
    }

    public string LoadToLanguageXML(string _fileName)
    {
        if (_fileName == "") return "";

        TextAsset txtAsset = (TextAsset)Resources.Load("XML/LanguageFile");

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(txtAsset.text);

        XmlNodeList node = xmlDoc.SelectNodes("dataroot/" + _fileName);

        //Debug.Log(node.Item(0).InnerText);
        return node.Item(0).InnerText;
    }

    public List<SceneManagement.SkinInfo> LoadSkinXML()
    {
        List<SceneManagement.SkinInfo> skins = new List<SceneManagement.SkinInfo>();
        TextAsset txtAsset = (TextAsset)Resources.Load("XML/SKIN_XML");

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(txtAsset.text);

        // 전체 아이템 가져오기
        XmlNodeList all_nodes = xmlDoc.SelectNodes("dataroot/SKIN");

        foreach (XmlNode node in all_nodes)
        {
            SceneManagement.SkinInfo skin = new SceneManagement.SkinInfo();

            if (node["LOCK"].InnerText != "IS_UNLOCK?!*%")
            {
                skin.body = "Skin/Body/999_Body";
                skins.Add(skin);
                continue;
            }

            skin.LOCK = false;
            skin.body = "Skin/Body/" + node["BODY"].InnerText + "_Body";
            skin.face = "Skin/Face/" + node["FACE"].InnerText + "_Face";

            //skin.hitEffect = (GameObject)Resources.Load("Skin/HitEffect/" + node["HIT_EFFECT"].InnerText);

            switch (node["CATEGORY"].InnerText)
            {
                case "tail":
                    skin.tailState = SceneManagement.SkinTailState.Effect;
                    skin.tailEffect = "Skin/TailEffect/" + node["TAIL_EFFECT"].InnerText + "_Tail";
                    break;

                case "color":
                    skin.tailState = SceneManagement.SkinTailState.Color;
                    break;
            }

            string[] str = node["COLOR"].InnerText.Split('/');
            Color color = new Color(float.Parse(str[0]), float.Parse(str[1]), float.Parse(str[2]));

            skin.tailColor = color;

            skins.Add(skin);
        }

        return skins;
    }
}
