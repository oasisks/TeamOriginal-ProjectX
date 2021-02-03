using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{

    public float delayTime;

    // Start is called before the first frame update
    void Start()
    {
        Object.Destroy(gameObject, delayTime);
    }
}
