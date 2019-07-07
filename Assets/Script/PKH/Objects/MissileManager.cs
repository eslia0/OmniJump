using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileManager : MonoBehaviour
{
    public enum MissileState
    {
        distance,
        positionX,
    }
    public MissileState state;

    private Missile[] missiles;
    private List<Missile> missileList = new List<Missile>();


    void Start()
    {
        missiles = transform.GetComponentsInChildren<Missile>();
        
        if (state == MissileState.distance)
        {
            StartCoroutine(DistanceUpdate());
        } else
        {
            StartCoroutine(PositionUpdate());
        }
    }

    IEnumerator DistanceUpdate()
    {
        while (true)
        {
            if (Vector2.Distance(Creater.Instance.player.transform.position, transform.position) < 0.16f)
            {
                StartCoroutine(MissileUpdating());
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    IEnumerator PositionUpdate()
    {
        while (true)
        {
            if (Creater.Instance.player.transform.position.x > transform.position.x)
            {
                StartCoroutine(MissileUpdating());
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    IEnumerator MissileUpdating()
    {
        if(missiles.Length < 1)
        {
            yield return null;
        }

        int num = 0;
        float delay = 0;
        while (true)
        {
            if (num >= missiles.Length && missileList.Count < 1)
            {
                break;
            }

            if (num < missiles.Length)
            {
                if (delay < missiles[num].lunchDelay)
                {
                    delay += Time.deltaTime;
                }
                else
                {
                    missileList.Add(missiles[num].Lunch());
                    num++;
                    delay = 0;
                }
            }
            
            for(int i = missileList.Count-1; i > -1; i--)
            {
                if (missileList[i] != null)
                {
                    missileList[i].Updating();
                }
                else
                {
                    missileList.RemoveAt(i);
                }
            }

            yield return new WaitForEndOfFrame();
        }
        
        Destroy(gameObject);
        yield return null;
    }
}
