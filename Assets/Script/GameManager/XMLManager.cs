using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public sealed class XMLManager : MonoBehaviour
{
    private XmlDocument xmlDoc;
    private XmlNodeList all_nodes;
    public int GetNodeCount()
    {
        if (xmlDoc == null)
        {
            TextAsset txtAsset = (TextAsset)Resources.Load("XML/SKIN_XML");

            xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(txtAsset.text);

            // 전체 아이템 가져오기
            all_nodes = xmlDoc.SelectNodes("dataroot/SKIN");
        }

        return all_nodes.Count;
    }

    public void LoadSkinXML(List<SkinMaster.SkinInfo> skins, List<bool> Access, List<int> lockArray, string KEY, string BODY_KEY, string FACE_KEY, string TAIL_KEY)
    {
        if (KEY != "IS_UNLOCK?!*%" ||
            BODY_KEY != "_Body" ||
            FACE_KEY != "_Face" ||
            TAIL_KEY != "_Tail")
        {
            return;
        }

        if (xmlDoc == null)
        {
            GetNodeCount();
        }

        int num = 0;
        int i = 0;

        foreach (XmlNode node in all_nodes)
        {
            SkinMaster.SkinInfo skin = new SkinMaster.SkinInfo();

            if (!Access[i]) // 잠겼을때
            {
                lockArray.Add(num);
                skin.LOCK = true;
            }
            else
            {
                skin.LOCK = false;
            }

            skin.body = Resources.Load<Sprite>("Skin/Body/" + node["BODY"].InnerText + BODY_KEY);
            skin.face = Resources.Load<Sprite>("Skin/Face/" + node["FACE"].InnerText + FACE_KEY);

            num++;
            i++;

            switch (node["CATEGORY"].InnerText)
            {
                case "tail":
                    skin.tailState = "EFFECT";
                    skin.tailEffect = Resources.Load<GameObject>("Skin/TailEffect/" + node["TAIL_EFFECT"].InnerText + TAIL_KEY);
                    break;

                case "color":
                    skin.tailState = "COLOR";
                    break;
            }

            string[] str = node["COLOR"].InnerText.Split('/');
            Color color = new Color(float.Parse(str[0]), float.Parse(str[1]), float.Parse(str[2]));

            skin.tailColor = color;

            skins.Add(skin);
        }
    }
}
