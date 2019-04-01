using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize : MonoBehaviour
{
    Creater creater;
    
    // Start is called before the first frame update
    void Start()
    {
        creater = Creater.Instance;
    }
}
