using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {
    public Vector2 size;
    [SerializeField]
    public int score { get; private set; }
    public Transform highPoint;
    public Transform lowPoint;
    public Transform endPoint;

    void Awake()
    {
        Transform start = transform.GetChild(0);
        endPoint = transform.GetChild(1);
        highPoint = transform.GetChild(2);
        lowPoint = transform.GetChild(3);
        size = endPoint.position - start.position;
    }
}
