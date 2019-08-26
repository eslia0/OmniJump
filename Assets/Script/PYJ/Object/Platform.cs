using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {
    [SerializeField]
    public int score { get; private set; }
    public Transform highPoint;
    public Transform lowPoint;
    public Transform EndPoint {
        get {
            return transform.GetChild(1);
        }
    }

    [TextArea]
    public string stageText;

    void Awake()
    {
        highPoint = transform.GetChild(2);
        lowPoint = transform.GetChild(3);
    }
}
