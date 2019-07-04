using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPatern : MonoBehaviour
{
    [SerializeField] GameObject bossObject;
    
    [SerializeField] List<GameObject> pads;

    // Start is called before the first frame update
    void Start()
    {
        int count = transform.childCount;
        pads = new List<GameObject>();

        for (int i = 0 ; i < count; i++)
        {
            GameObject pad = transform.GetChild(i).gameObject;
            pads.Add(pad);
            pad.SetActive(false);
        }
        
        StartCoroutine(EnableRandomPad());
    }

    IEnumerator EnableRandomPad()
    {
        yield return new WaitForSeconds(10.0f);

        while (pads.Count > 0)
        {
            transform.position = bossObject.transform.position;

            int index = Random.Range(0, pads.Count);
            GameObject pad = pads[index];
            pad.SetActive(true);

            yield return new WaitForSeconds(5.0f);

            pads.Remove(pad);
            pad.transform.SetParent(null);
            Destroy(pad, 2.0f);
        }
    }
}
