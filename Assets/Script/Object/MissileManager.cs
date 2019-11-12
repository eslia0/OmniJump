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
    public int actionCount = 1;

    [SerializeField] private Missile[] missiles;
    [SerializeField] private List<Missile> missileList = new List<Missile>();

    void OnEnable()
    {
        missiles = transform.GetComponentsInChildren<Missile>();
        InitMissiles();
    }

    private void InitMissiles()
    {
        for (int i = 0; i < missiles.Length; i++)
        {
            missiles[i].gameObject.SetActive(true);
            missiles[i].Init();
        }

        if (state == MissileState.distance)
        {
            StartCoroutine(DistanceUpdate());
        }
        else
        {
            StartCoroutine(PositionUpdate());
        }
    }

    IEnumerator DistanceUpdate()
    {
        while (Creater.Instance)
        {
            if (Vector2.Distance(Creater.Instance.player.transform.position, transform.position) < 0.16f)
            {
                StartCoroutine(MissileUpdating());
                break;
            }

            foreach (Missile missile in missiles)
            {
                if (missile)
                {
                    missile.OffsetUp();
                }
            }
            yield return null;
        }

        yield return null;
    }

    IEnumerator PositionUpdate()
    {
        while (Creater.Instance)
        {
            if (Creater.Instance.player.transform.position.x > transform.position.x)
            {
                Debug.Log("Position");
                StartCoroutine(MissileUpdating());
                break;
            }

            foreach (Missile missile in missiles)
            {
                missile.OffsetUp();
            }
            yield return null;
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

        while (Creater.Instance)
        {
            if (!Creater.Instance.isPaused)
            {
                // 미사일 발사 딜레이 체크
                if (!missiles[num].isLunched && !missiles[num].isDead)
                {
                    // 시간 측정 및 발사
                    if (delay < missiles[num].lunchDelay)
                    {
                        delay += Time.deltaTime;
                    }
                    else
                    {
                        delay = 0;
                        missileList.Add(missiles[num].Lunch());
                        num++;
                    }
                }
                else
                {
                    num++;
                }

                if (num > missiles.Length - 1)
                {
                    num = 0;
                    if (missileList.Count == 0)
                    {
                        break;
                    }
                }

                // Missile Position Updating
                for (int i = missileList.Count - 1; i > -1; i--)
                {
                    if (missileList[i].isLunched)
                    {
                        missileList[i].Updating();
                    }
                    else
                    {
                        missileList.RemoveAt(i);
                    }
                }
            }

            yield return null;
        }

        actionCount--;

        if (actionCount == 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(1.0f);
            InitMissiles();
        }
    }
}
