using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall12 : MonoBehaviour
{
    public float speed;
    public float rotateSpeed = 180f;


    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 30f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            coin.instance.PlaySound();
            Destroy(gameObject);
            coin.instance.scoreT++;
        }
    }
}
