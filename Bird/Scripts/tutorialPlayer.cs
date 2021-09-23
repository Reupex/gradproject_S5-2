using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System; //소수점 자르려고 넣음
using System.Data;
using System.Data.Sql;

public class tutorialPlayer : MonoBehaviour
{
    public float Speed = 0.5f;
    public GameObject endPanel;
    public GameObject wall, wall1;
    public GameObject coin;

    float playTime; //게임 시간
    public Text pt; //게임 시간 표기
    public Text finishTime, score, bestScore; //게임 끝난 시간 표기
    //private int Timer = 0;
    //private int a = 1;

    public Slider slider; //슬라이더
    public float position;
    Vector3 pos;

    public GameObject coinMax;
    public GameObject coinMin; //  생성할 오브젝트 A
    Vector3 coinVes;

    public int scoreT=0;

    void Awake()
    {
        coinVes = coin.gameObject.transform.position;
    }
    void Start()
    {
        print(gameObject.name + "시작");
        //Timer = 0;
        slider.value = 0.5f;
    }

    void Update()
    {
        pos = gameObject.transform.position; //플레이어 백터
        pos.y = slider.value / 240 * 9 - 5; //슬라이더 값을 플레이어의 y값 안에 넣음

        transform.position = pos; //플레이어의 포지션 변경

        playTime += Time.deltaTime;
        pt.text = "점수 : " + Math.Round(playTime); //플레이 타임

    }
    void CreatOBJA()
    {
        GameObject CreatA = Instantiate(coinMax) as GameObject; //생성
        CreatA.transform.position = new Vector3(coinVes.x, 4, pos.z);// 생성될 자표 설정
        CreatA.transform.Translate(5 * Time.deltaTime, 0, 0);
        Destroy(CreatA, 10);//파괴될 시간설정
    }
    void CreatOBJB()
    {
        GameObject CreatB = Instantiate(coinMin) as GameObject; //생성
        CreatB.transform.position = new Vector3(coinVes.x, -4, pos.z);// 생성될 자표 설정
        CreatB.transform.Translate(5 * Time.deltaTime, 0, 0);
        Destroy(CreatB, 10);//파괴될 시간설정
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("wall"))
        {
            Debug.Log("보너스 코인");
            CreatOBJA();
        }
        else if (collision.gameObject.CompareTag("wall1"))
        {
            Debug.Log("보너스 코인");
            CreatOBJB();
        }
        else
        {
            Debug.Log("충돌했다구~");
            pt.gameObject.SetActive(false); //pt 안보이게 하기
            Time.timeScale = 0; //게임 일시정지
            finishTime.text = "게임시간 : " + Math.Truncate(playTime * 100) / 100; //플레이 타임
            score.text = "점수 : " + scoreT;
            endPanel.SetActive(true);
        }
    }
}
