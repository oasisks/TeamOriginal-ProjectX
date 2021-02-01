using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBehavior : MonoBehaviour
{
    [SerializeField] private float stabPeriod;
    private Animator animator;

    public bool dangerous;
    private float stabcounter = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        stabcounter += Time.deltaTime;
        if(stabcounter >= stabPeriod) {
            stabcounter = 0;
            animator.SetTrigger("Stab");
        }
    }

    public void setDangerous() { //called by animation
        dangerous = true; 
    }

    public void setSafe() { //called by animation
        dangerous = false; 
    }
}
