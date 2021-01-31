using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Flag flag;
    private void Start()
    {
        flag = GameObject.FindGameObjectWithTag("Flag").GetComponent<Flag>();
    }
    private void Update()
    { 
        if (flag.hasPassedLevel)
        {
            // we show a UI congratulating/switch levels/etc.
        }
    }
}
