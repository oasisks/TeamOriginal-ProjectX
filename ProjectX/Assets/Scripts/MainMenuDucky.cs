using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDucky : MonoBehaviour
{
    [SerializeField] Transform startLocation;
    [SerializeField] Transform endLocation;
    [SerializeField] GameObject duckPrefab;
    [SerializeField] float speed;

    public AudioSource walkingSound;
    private GameObject duck;

    private void Start()
    {
        duck = Instantiate(duckPrefab, startLocation.position, Quaternion.identity);
    }

    private void Update()
    {
        MoveDuck();
    }

    private void MoveDuck()
    {
        if (duck != null)
        {
            // continuously move towards the end location
            duck.transform.Translate(Vector3.right * Time.deltaTime * speed);
            
            if (duck.transform.position.x >= endLocation.position.x)
            {
                Debug.Log("I hit location");

                Destroy(duck);
            }
        }
        else
        {
            duck = Instantiate(duckPrefab, startLocation.position, Quaternion.identity);
        }
    }
}
