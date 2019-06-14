using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBG : MonoBehaviour
{
    public float parallaxSpeed;
    public float yOffset;
    public float length = 0;

    private Transform cam;
    private float startPos;
    private float lastPos;


    void Start()
    {
        cam = Camera.main.transform;
        startPos = transform.position.x;

        if(length == 0)
        {
            length = GetComponent<SpriteRenderer>().bounds.size.x;
        }
        else
        {
            length *= transform.localScale.x;
        }
    }

    void FixedUpdate()
    {
        float par = (cam.position.x * (1 - parallaxSpeed));
        float dist = (cam.position.x * parallaxSpeed);
        
        transform.position = new Vector2(startPos + dist,
            cam.position.y + yOffset);

        if (par > startPos + length)
        {
            startPos += length;
        }
        else if(par < startPos - length)
        {
            startPos -= length;
        }

        lastPos = cam.position.x;
    }
}
