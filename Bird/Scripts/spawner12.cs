using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner12 : MonoBehaviour
{
    public GameObject wallPrefab;
    public float interval;
    public float range = 3.0f;
    public float DestroyXPos; // 닭이 사라지는 지점

    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            transform.position = new Vector3(Random.Range(-range, range), 5.5f, 86.55f);
            Instantiate(wallPrefab, transform.position, transform.rotation);
            //yield return new WaitForSeconds(interval);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
        // 만약에 닭의 위치가 DestroyYPos를 넘어서면
        if (transform.position.y <= DestroyXPos)
        {
            // 닭을 제거

        }
    }
}
