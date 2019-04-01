using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleRotation : MonoBehaviour
{
    public void SetParticlesFourWayDirection(Direction direction, ParticleSystem[] particle)
    {
        float angle = 0;

        switch (direction)
        {
            case Direction.up:
                break;
            case Direction.right:
                angle = 90 * Mathf.Deg2Rad;
                break;
            case Direction.left:
                angle = 270 * Mathf.Deg2Rad;
                break;
            case Direction.down:
                angle = 180 * Mathf.Deg2Rad;
                break;
        }

        ParticleRotate(angle, particle);
    }

    public void SetParticlesRotation(float eulerAngleZ, ParticleSystem[] particle)
    {
        ParticleRotate(-eulerAngleZ * Mathf.Deg2Rad, particle);
    }

    private void ParticleRotate(float angle, ParticleSystem[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i].startRotation = angle;
        }
    }
}
