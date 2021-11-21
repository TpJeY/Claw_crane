using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    Transform temp_prize;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            temp_prize = transform.GetChild(i);
            temp_prize.localPosition = new Vector3(Random.Range(-0.5f, 0), Random.Range(0.75f, 1f), Random.Range(-0.75f, 0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
