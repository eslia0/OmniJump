using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public abstract class InteractiveObject : MonoBehaviour
{
    [Header("파티클")]
    [SerializeField] private ParticleSystem[] particles;

    /// 플레이어가 해당 오브젝트와 상호작용할 때,
    /// 오브젝트가 플레이어에게 요구하는 바라보는 방향
    [Header("탐지 방향")]
    [SerializeField] protected Direction direction;

    [Header("동작 가능 횟수")]
    [SerializeField] public uint actionCount = 1;
    [SerializeField] private bool destroyExplosion;
    
    protected bool playerIsOn;

    protected abstract void Init();
    protected virtual void update()
    {
        actionCount--;

        if (actionCount < 1)
        {
            Dispose();
        }
    }

    protected bool FaceCompare()
    {
        return (direction == (Direction)((Creater.Instance.player.rotationZ + Creater.Instance.player.faceDirection + 4) % 4));
    }

    protected virtual void Dispose()
    {
        if (destroyExplosion)
        {
            Creater.Instance.GetPopPrefab(transform);
            Destroy(gameObject);
        }
        enabled = false;
    }

    private void Start()
    {
        Init();
    }

    public void SetParticle(bool set)
    {
        if (particles.Length > 0)
        {
            if (set)
            {
                foreach (ParticleSystem particle in particles)
                {
                    particle.Play();
                }
            }
            else
            {
                foreach (ParticleSystem particle in particles)
                {
                    particle.Stop();
                }
            }
        }
    }
}
