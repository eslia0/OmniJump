using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportExit : InteractiveObject
{
    [SerializeField] private ParticleSystem[] exitportalLight;

    protected override void Init()
    {
        Creater.Instance.particleRotation.SetParticlesRotation(transform.eulerAngles.z, exitportalLight);
    }
}
