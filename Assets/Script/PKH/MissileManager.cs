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
    
    // Start is called before the first frame update
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
                LunchMissiles();
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
                LunchMissiles();
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    void LunchMissiles()
    {
        float delay = 0;
        for (int i = 0; i < missiles.Length; i++)
        {
            if (missiles[i] != null)
            {
                delay += missiles[i].lunchDelay;
                missiles[i].InvokeLunch(delay);
            }
        }        
    }
}
