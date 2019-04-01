using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public Vector3 speed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Time.deltaTime * speed.x, Time.deltaTime * speed.y, Time.deltaTime * speed.z);
    }
}
