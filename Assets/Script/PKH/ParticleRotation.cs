using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleRotation : MonoBehaviour
{
    public ParticleSystem[] positive; // 목표 각도와 같은 값의 회전을 가질 파티클
    public ParticleSystem[] negative; // 목표 각도에 음수 값의 회전을 가질 파티클
    public InteractionManager interact;
    public ParticleSystem[] interactArray;

    // Start is called before the first frame update
    void Start()
    {
        float angle = 0;

        if (interact)
        {
            switch (interact.direction)
            {
                case Direction.right:
                    angle = 90 * Mathf.Deg2Rad;
                    break;
                case Direction.up:
                    angle = 0;
                    break;
                case Direction.left:
                    angle = 270 * Mathf.Deg2Rad;
                    break;
                case Direction.down:
                    angle = 180 * Mathf.Deg2Rad;
                    break;
            }
            ParticleRotate(angle, interactArray);
        }

        if (positive.Length != 0 || negative.Length != 0)
        {
            angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;

            ParticleRotate(angle, positive);
            ParticleRotate(angle, negative);
        }
    }

    private void ParticleRotate(float angle, ParticleSystem[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i].startRotation = angle;
        }
    }
}
