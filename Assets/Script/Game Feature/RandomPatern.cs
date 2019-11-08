using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPatern : MonoBehaviour
{
    [SerializeField] GameObject bossObject;
    
    [SerializeField] List<GameObject> pads;
    [SerializeField] float delay;

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
        float check = 0f;

        while (check <= delay + 5.0f && Creater.Instance) {
            if (!Creater.Instance.isPaused) {
                check += Time.deltaTime;
            }
            yield return null;
        }
        check = 0f;

        while (pads.Count > 0 && Creater.Instance)
        {
            transform.position = bossObject.transform.position;

            int index = Random.Range(0, pads.Count);
            GameObject pad = pads[index];
            pad.SetActive(true);

            while (check <= delay && Creater.Instance)
            {
                if (!Creater.Instance.isPaused)
                {
                    check += Time.deltaTime;
                }
                yield return null;
            }

            check = 0;
            pads.Remove(pad);
            pad.transform.SetParent(null);
            Destroy(pad, 2.0f);
        }
        
        bossObject.transform.Find("BossPad").GetComponent<Lift>().enabled = false;
    }
}
