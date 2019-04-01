using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Direction
{
    right = 0,
    up,
    left,
    down
}
public enum PlatformMode
{
    Active,
    Passive,
    Trigger,
    Distance,
    ParticleRotate
}

public class GameVariables : MonoBehaviour
{
    // 파티클 회전 클래스 객체
    public ParticleRotation particleRotation = new ParticleRotation();
    // 랜덤 방향 제조기
    public DirectionRandomizer randomizer = new DirectionRandomizer();

    // 플레이어 변수
    private PlayerController m_player;
    public PlayerController player {
        get {
            if (m_player == null)
            {
                m_player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            }
            return m_player;
        }
    }
    public LayerMask playerLayer {
        get {
            return LayerMask.GetMask("Player");
        }
    }

    // Circle 및 일반 터지는 효과
    private GameObject[] circlePopParticles;
    private int popCount = 0;

    // 미사일 터짐 효과
    private GameObject[] missilePopParticles;
    private int missilePopCount = 0;

    // 텔레포트 터짐 효과
    private GameObject poofParticles;

    protected void SetPopParticles()
    {
        if (circlePopParticles == null)
        {
            circlePopParticles = new GameObject[3];
            GameObject popGroup = new GameObject("PopGroup");
            DontDestroyOnLoad(popGroup);

            GameObject prefab = Resources.Load("Effects/PopExplosion3") as GameObject;
            for (int i = 0; i < 3; i++)
            {
                circlePopParticles[i] = Instantiate(prefab);
                DontDestroyOnLoad(circlePopParticles[i]);

                circlePopParticles[i].SetActive(false);
                circlePopParticles[i].transform.parent = popGroup.transform;
            }
        }
    }
    public void GetPopPrefab(Transform transform)
    {
        if (circlePopParticles == null)
        {
            SetPopParticles();
        }

        circlePopParticles[popCount].SetActive(false);

        circlePopParticles[popCount].transform.position = transform.position;

        circlePopParticles[popCount].SetActive(true);
        
        if (popCount < circlePopParticles.Length-1)
        {
            popCount++;
        }
        else
        {
            popCount = 0;
        }
    }

    protected void SetMissilePopParticles()
    {
        if (missilePopParticles == null)
        {
            missilePopParticles = new GameObject[8];
            GameObject popGroup = new GameObject("MissilePopGroup");
            DontDestroyOnLoad(popGroup);

            GameObject prefab = Resources.Load("Effects/GlowExplosion 1") as GameObject;
            for (int i = 0; i < 8; i++)
            {
                missilePopParticles[i] = Instantiate(prefab);
                DontDestroyOnLoad(missilePopParticles[i]);

                missilePopParticles[i].SetActive(false);
                missilePopParticles[i].transform.parent = popGroup.transform;
            }
        }
    }
    public void GetMissilePopPrefab(Transform transform)
    {
        if (missilePopParticles == null)
        {
            SetMissilePopParticles();
        }

        missilePopParticles[missilePopCount].SetActive(false);

        missilePopParticles[missilePopCount].transform.position = transform.position;

        missilePopParticles[missilePopCount].SetActive(true);

        if (missilePopCount < missilePopParticles.Length-1)
        {
            missilePopCount++;
        }
        else
        {
            missilePopCount = 0;
        }
    }

    protected void SetPoofPrefab()
    {
        if (poofParticles == null)
        {
            poofParticles = Instantiate(Resources.Load("Effects/Poof 1") as GameObject);
            DontDestroyOnLoad(poofParticles);
            poofParticles.SetActive(false);
        }
    }
    public void GetPoofPrefab(Transform transform)
    {
        if (poofParticles == null)
        {
            SetPoofPrefab();
        }

        poofParticles.SetActive(false);

        poofParticles.transform.position = transform.position;

        poofParticles.SetActive(true);
    }
}
