using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawn : MonoBehaviour 
{
    public GameObject[] prefabs;
    // Start is called before the first frame update
    void Start()
    {
        spawnNext();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void spawnNext() {
        // Random Index
        int i = Random.Range(0, prefabs.Length);

        // Spawn Group at current Position
        Instantiate(prefabs[i], transform.position, Quaternion.identity);
    }
}
