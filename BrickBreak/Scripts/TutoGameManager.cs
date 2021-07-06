using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

/*
 * 튜토리얼용 게임 매니저 입니다.
 * 만든이유 : 원 게임에 '성공'이란 개념이 없어져서. 
 * 대부분 게임 매니저랑 똑같습니다.
 */

public class TutoGameManager : MonoBehaviour
{
    public Slider BarCont;
    public GameObject gameOver;
    public GameObject gameClear;
    public GameObject gameStop;
    public GameObject block;
    public GameObject Spon;

    public Text Gscorre;
    public Text Times;
    
    public bool GameOvQ = false;
    public int GameScor = 0;
    public int MainScor = 0;
    public float timer;
    public float blockST = 14.0f;
    public int n;

    public int Stage_S;

    public static TutoGameManager instance;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Serial.instance.Active();
        gameOver.SetActive(false);
        gameClear.SetActive(false);
        gameStop.SetActive(false);

        //Stage_S = 20;
        n = UnityEngine.Random.Range(0, 5);
        Instantiate(block, Spon.transform.position, Quaternion.identity, GameObject.Find("Blocks").transform);
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        this.timer += Time.deltaTime;
        Gscorre.text = string.Concat(GameScor);
        Times.text = string.Concat((object)Mathf.Round(this.timer));

        blockST -= Time.deltaTime;  // 리스폰 시간을 깍음.
        if (blockST < 0)                  // 리스폰 시간이 0이 되었는지 검사
        {
            //b_Move = true;
            Instantiate(block, Spon.transform.position, Quaternion.identity, GameObject.Find("Blocks").transform); // 생성
            blockST = 14.0f;                                                    // 리스폰시간 초기화
        }

        if (GameOvQ)
        {
            Time.timeScale = 0;
            gameOver.SetActive(true);
        }

        if (GameScor == Stage_S)//게임 매니저랑 다르게 성공부분이 있습니다.
        {
            Time.timeScale = 0;
            gameClear.SetActive(true);
        }
    }

    public void ReStart()
    {
        //string n = SceneManager.GetActiveScene().name;
        //SceneManager.LoadScene(n);
        GameOvQ = false;
        gameOver.SetActive(false);
        Time.timeScale = 1;
    }

    public void goMain()
    {
        SceneManager.LoadScene("BrickMain");
        Destroy(StageManager.instance.gameObject);
    }

    public void GameStop()
    {
        BrickSound.instance.StopBack();
        Time.timeScale = 0;
        gameStop.SetActive(true);
    }

    public void backGame()
    {
        BrickSound.instance.playBack();
        Time.timeScale = 1;
        gameStop.SetActive(false);
    }

    public void F_Line()
    {
        Instantiate(block, Spon.transform.position, Quaternion.identity, GameObject.Find("Blocks").transform); // 생성
        blockST = 14.0f;
    }
}
