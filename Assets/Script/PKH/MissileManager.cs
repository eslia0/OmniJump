using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileManager : MonoBehaviour
{
    private Missile[] missiles;
    private GameObject player;

    bool isLunched = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        missiles = transform.GetComponentsInChildren<Missile>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            Destroy(gameObject);
        else
        {
            if (Creater.Instance.player.transform.position.x > transform.position.x && !isLunched)
            {
                isLunched = true;
                StartCoroutine(LunchMissiles());
            }
        }
    }

    IEnumerator LunchMissiles()
    {
        for (int i = 0; i < missiles.Length; i++)
        {
            if (missiles[i] != null)
            {
                if (missiles[i].lunchDelay > 0)
                {
                    yield return new WaitForSeconds(missiles[i].lunchDelay);
                }

                missiles[i].SetState(true);
            }
        }

        enabled = false;
    }
}
