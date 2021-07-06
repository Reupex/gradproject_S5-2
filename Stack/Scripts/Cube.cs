using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cube : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame 
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
            GetComponent<Rigidbody>().isKinematic = true;
        if(collision.gameObject.tag == "cube")
        {
            if (SceneManager.GetActiveScene().name == "StackGame")
                Stack.instance.Check();
            else if (SceneManager.GetActiveScene().name == "StackTutorial")
                StackTuto.instance.Check();
            else if (SceneManager.GetActiveScene().name == "StackPassive")
                StackPassive.instance.Check();
            collision.gameObject.tag = "stack";
        }
    }
}
