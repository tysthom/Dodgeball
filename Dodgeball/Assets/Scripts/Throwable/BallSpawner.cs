using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ball;
    

    // Start is called before the first frame update
    void Start()
    {
        
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(3);
        
        Instantiate(ball, new Vector3(Random.Range(-40,40), 10, Random.Range(-40, 40)), gameObject.transform.rotation);
        StartCoroutine(Spawn());
    }
}
