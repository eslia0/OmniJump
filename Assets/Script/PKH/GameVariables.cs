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


public class GameVariables : MonoBehaviour
{
    // 파티클 회전 클래스 객체
    private ParticleRotation m_pr;
    public ParticleRotation particleRotation {
        get {
            if (m_pr == null)
                m_pr = new ParticleRotation();

            return m_pr;
        }
    }
    // 랜덤 방향 제조기
    private DirectionRandomizer m_randomizer;
    public DirectionRandomizer randomizer {
        get {
            if (m_randomizer == null)
                m_randomizer = new DirectionRandomizer();

            return m_randomizer;
        }
    }

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

    // MovePlatform의 Trigger 효과
    private GameObject triggerBlowParticle;
    private int triggerBlowpCount = 0;
    
    public delegate void ObjectUpdate();
    private ObjectUpdate UpdateDelegate;


    protected virtual void GameVariablesInit()
    {
        SetPoofPrefab();
        SetPopParticles();
        SetMissilePopParticles();
        SetTriggerBlowParticles();
    }

    public virtual void Disable()
    {
        Object.Destroy(circlePopParticles[0].transform.parent.gameObject);
        Object.Destroy(missilePopParticles[0].transform.parent.gameObject);
        Object.Destroy(poofParticles);
        Object.Destroy(triggerBlowParticle);

        circlePopParticles = null;
        missilePopParticles = null;
        poofParticles = null;
        triggerBlowParticle = null;
    }

    protected void SetPopParticles()
    {
        if (circlePopParticles == null)
        {
            circlePopParticles = new GameObject[3];
            GameObject popGroup = new GameObject("PopGroup");
            Object.DontDestroyOnLoad(popGroup);

            GameObject prefab = Resources.Load("Effects/PopExplosion3") as GameObject;
            for (int i = 0; i < 3; i++)
            {
                circlePopParticles[i] = Object.Instantiate(prefab);
                Object.DontDestroyOnLoad(circlePopParticles[i]);

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
            Object.DontDestroyOnLoad(popGroup);

            GameObject prefab = Resources.Load("Effects/GlowExplosion 1") as GameObject;
            for (int i = 0; i < 8; i++)
            {
                missilePopParticles[i] = Object.Instantiate(prefab);
                Object.DontDestroyOnLoad(missilePopParticles[i]);

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
            poofParticles = Object.Instantiate(Resources.Load("Effects/Poof 1") as GameObject);
            Object.DontDestroyOnLoad(poofParticles);
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

    protected void SetTriggerBlowParticles()
    {
        if (triggerBlowParticle == null)
        {
            triggerBlowParticle = Object.Instantiate(Resources.Load("Effects/TriggerBlow") as GameObject);
            Object.DontDestroyOnLoad(triggerBlowParticle);
            triggerBlowParticle.SetActive(false);
        }
    }
    public void GetTriggerBlowParticles(Direction direction, Transform transform)
    {
        if (triggerBlowParticle == null)
        {
            SetTriggerBlowParticles();
        }

        triggerBlowParticle.SetActive(false);

        triggerBlowParticle.transform.position = transform.position;
        switch (direction)
        {
            case Direction.right:
                triggerBlowParticle.GetComponent<ParticleSystem>().startRotation = 90 * Mathf.Deg2Rad;
                break;
            case Direction.left:
                triggerBlowParticle.GetComponent<ParticleSystem>().startRotation = 270 * Mathf.Deg2Rad;
                break;
            case Direction.down:
                triggerBlowParticle.GetComponent<ParticleSystem>().startRotation = 180 * Mathf.Deg2Rad;
                break;
        }

        triggerBlowParticle.SetActive(true);
    }

    public void UpdateDelegateAdd(ObjectUpdate objectUpdate)
    {
        if (UpdateDelegate == null)
        {
            UpdateDelegate = new ObjectUpdate(objectUpdate);
            StartCoroutine(Updating());
        }
        else
        {
            UpdateDelegate += objectUpdate;
        }
    }
    public void UpdateDelegateRemove(ObjectUpdate objectUpdate)
    {
        UpdateDelegate -= objectUpdate;
    }

    public IEnumerator Updating()
    {
        while (true)
        {
            if (UpdateDelegate == null)
            {
                break;
            }
            UpdateDelegate();

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

}
