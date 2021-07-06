using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class BrickGame : MonoBehaviour
{
    public GameObject Bar;
    public GameObject gameOver;
    public GameObject warning;
    public GameObject gameStop;
    public GameObject block;
    public GameObject Spon;
    public GameObject circleGraph;

    public Text Gscorre; // 점수 text
    public Text Times; // 시간 text

    public Text F_scorre; // endPanel 점수 text
    public Text F_time; // endPanel 시간 text
    public Text MaxScore_Text; // 최고점수

    public Vector2 vec;
    public bool GameOvQ = false;
    public int GameScor = 0; // 게임점수 담는 변수
    public float timer; 
    public float blockST = 14.0f;
    public int n;
    
    int maxScore = 0;
    string date;
    string userID;
    string gameID;

    public static BrickGame instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Serial 운동 설정
        if(Data.Instance.GameID.Substring(0,1) == "1") // 능동운동
            Serial.instance.Active();
        else if(Data.Instance.GameID.Substring(0, 1) == "4") // 저항운동
            Serial.instance.Resistance();

        date = DBManager.SqlFormat(DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초"));
        userID = DBManager.SqlFormat(Data.Instance.UserID);
        gameID = DBManager.SqlFormat(Data.Instance.GameID);

        gameOver.SetActive(false);
        warning.SetActive(false);
        gameStop.SetActive(false);
        
        n = UnityEngine.Random.Range(0, 5);//블록 색깔 랜덤으로 뽑는 문장

        if (StageManager.instance.Stage == 2)//스테이지(난이도)에 따라 바의 길이를 조종합니다.
        {
            Bar.transform.localScale = new Vector3(Bar.transform.localScale.x*7/9, Bar.transform.localScale.y, 0);
        }
        else if(StageManager.instance.Stage == 3)//어려움 난이도는 바 길이가 더 짧습니다.
        {
            Bar.transform.localScale = new Vector3(Bar.transform.localScale.x*5/9, Bar.transform.localScale.y, 0);
        }
        Instantiate(block, Spon.transform.position, Quaternion.identity, GameObject.Find("Blocks").transform);//시작하자 마자 첫 블록 생성
        Time.timeScale = 1;

        DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0} and gameID = {1} ORDER BY gameScore DESC ", userID, gameID));
        while (DBManager.dataReader.Read()) 
        {          
            maxScore = DBManager.dataReader.GetInt32(8);
            MaxScore_Text.text = "최고점수 : " + maxScore;
            break; 
        }
        DBManager.DBClose();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;//게임 시간 측정
        Times.text = "시간 : " + string.Concat((object)Mathf.Round(timer)) + "초";//시간 표시하기
        
        blockST -= Time.deltaTime;  // 리스폰 시간을 깍음.
        if (blockST < 0 || Block.instance.transform.childCount < 0) // 리스폰 시간이 0이 되었는지 검사
        {
            //b_Move = true;
            Instantiate(block, Spon.transform.position, Quaternion.identity, GameObject.Find("Blocks").transform); // 생성
            blockST = 14.0f; // 리스폰시간 초기화
        }

        Gscorre.text = "점수 : " + string.Concat(GameScor); //점수 표시하기
        if ( GameScor > maxScore)
            MaxScore_Text.text = "최고점수 : " + GameScor;
    }
    
    public void ifOver()
    {
        if (GameOvQ)//게임 오버 되었을때 처리해주는 내용들입니다. 다른 스크립트들에서 게임 오버-참 값을 받으면 실행됩니다.
        {
            F_time.text = string.Concat((object)Mathf.Round(this.timer));//실패했을때 시간
            F_scorre.text = string.Concat(GameScor);//점수
            Time.timeScale = 0;//게임 시간 정지
            gameOver.SetActive(true);//게임 오버 화면 보여주기
            GameOverDB((float)Math.Truncate(this.timer * 100) / 100);//데이터베이스 연동 코드 - 시간 넣어줍니다.
            Device.instance.ResultCircle(circleGraph);
            Serial.instance.End();
            Data.Instance.FreshTree++;
            print(Data.Instance.FreshTree);
            GameOvQ = false;
        }
    }

    public void ReStart()//다시 시작 버튼
    {
        string scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene);
    }

    public void goMain()//메인 화면 가는 버튼
    {
        SceneManager.LoadScene("BrickMain");
        Destroy(StageManager.instance.gameObject);
    }

    public void Warning()//경고화면 띄우는 버튼
    {
        Serial.instance.End();
        SceneManager.LoadScene("BrickMain");
        Destroy(StageManager.instance.gameObject);
        Data.Instance.DeadTree++;
        print(Data.Instance.DeadTree);
    }

    public void GameStop()//게임 일시정지 하는 버튼
    {
        //BrickSound.instance.StopBack();
        Time.timeScale = 0;
        gameStop.SetActive(true);
    }

    public void backGame()//게임으로 다시 돌아가는 버튼
    {
        //BrickSound.instance.playBack();
        Time.timeScale = 1;
        gameStop.SetActive(false);
    }

    public void GameOverDB(float playtime_REAL) // 소수점 2째자리까지 나타내주는 코드 - 이하 버림)
    {
        //디비 연동 코드
        string sql = String.Format("Insert into Game VALUES( {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8} )", date, userID, gameID, "'5'", "NULL", 40, 40, playtime_REAL, GameScor);
        DBManager.DatabaseSQLAdd(sql);
    }

    public void F_Line()//다른 스크립트에서 블록을 생성할때 쓰는 함수입니다.
    {
        Instantiate(block, Spon.transform.position, Quaternion.identity, GameObject.Find("Blocks").transform); // 생성
        blockST = 14.0f;
    }
}
