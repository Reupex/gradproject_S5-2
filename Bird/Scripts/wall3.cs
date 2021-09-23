using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class wall3 : MonoBehaviour
{
    public float speed;


    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 30f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            print("닿았지~");
            Destroy(gameObject);
            spawner3.instance.number += 1;
        }
    }
}
