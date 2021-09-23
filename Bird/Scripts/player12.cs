using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System; //소수점 자르려고 넣음
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using System.Data.Sql;
using System.Data.SqlClient; //sqlite DB 사용
using Mono.Data.Sqlite; //db 사용
using UnityEngine.Networking; //네트워크 사용
using SqliteConnection = Mono.Data.Sqlite.SqliteConnection;
using Random = UnityEngine.Random;

public class player12 : MonoBehaviour
{
    public GameObject startNel; //시작할 때 숫자 세기

    public float Speed = 0.5f;
    public GameObject Panel;
    public GameObject startPanel;
    public GameObject wall;

    float playTime; //게임 시간
    float startTime; //시작할 때 숫자 세기
    public Text Gscore; //게임 시간 표기
    public Text debug; //오류 잡기
    public Text finishTime, score; //게임 끝난 시간 표기
    public Text MaxScore_Text;

    public string date;
    public string userID;
    public string gameID;
    public string sql;
    public string handle;
    public int maxScore;
    public int dbSpeed;

    public Slider slider;
    public float position;
    Vector3 pos;

    public GameObject coinMax;
    public GameObject coinMin;//  생성할 오브젝트 A

    public GameObject circleGraph; //원 그래프

    private void Awake()
    {
        // 오늘날짜 + 현재시간
        date = DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초");
        date = DBManager.SqlFormat(date);

        userID = DBManager.SqlFormat(Data.Instance.UserID); // 나중에 로그인 구현시 Column 클래스로 만들어서 그때 객체화하기
        gameID = DBManager.SqlFormat(Data.Instance.GameID);     // 각자 게임 이름에 맞게 변경
        maxScore = 0;
        MaxScore_Text.text = "최고점수 : " + maxScore;

        // 핸들번호 저장
        if (Data.Instance.GameID.Substring(1, 1) == "2") // 내/외회전
            handle = DBManager.SqlFormat(Data.Instance.ShortHadle);
        else // 내/외전
            handle = DBManager.SqlFormat(Data.Instance.LongHandle);

        // Serial 운동 설정
        if (Data.Instance.GameID.Substring(0, 1) == "1") // 능동운동
            Serial.instance.Active();
        else if (Data.Instance.GameID.Substring(0, 1) == "4") // 저항운동
            Serial.instance.Resistance();
    }
    void CreatOBJA()
    {
        GameObject CreatA = Instantiate(coinMax) as GameObject; //생성
        CreatA.transform.position = new Vector3(-9.1f, 5.5f, 86.55f);// 생성될 자표 설정
        CreatA.transform.Translate(5 * Time.deltaTime, 0, 0);
        Destroy(CreatA, 10);//파괴될 시간설정
    }
    void CreatOBJB()
    {
        GameObject CreatB = Instantiate(coinMin) as GameObject; //생성
        CreatB.transform.position = new Vector3(8.96f, 5.5f, 86.55f);// 생성될 자표 설정
        CreatB.transform.Translate(5 * Time.deltaTime, 0, 0);
        Destroy(CreatB, 10);//파괴될 시간설정
    }
    void Start()
    {
        print(gameObject.name + "시작");
        //StartCoroutine(StartCountDown());
        //Timer = 0;
        DBManager.DbConnectionChecek();

        DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0} and gameID = {1} ORDER BY gameScore DESC ", userID, gameID));
        while (DBManager.dataReader.Read())
        {
            maxScore = DBManager.dataReader.GetInt32(8);
            break;
        }
        DBManager.DBClose();
        slider.value = 0;

    }

    void Update()
    {
        pos = gameObject.transform.position; //플레이어 백터
        pos.y = -3.67f;
        pos.x = slider.value/180*16f+1; //슬라이더 값을 플레이어의 x값 안에 넣음

        transform.position = pos; //플레이어의 포지션 변경

        playTime += Time.deltaTime;
        Gscore.text = "점수 : " + coin12.instance.scoreT.ToString();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "wall")
        {
            Debug.Log("보너스 코인");
            CreatOBJA();
        }
        else if (other.tag == "wall1")
        {
            Debug.Log("보너스 코인");
            CreatOBJB();
        }
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
            Device.instance.ResultCircle(circleGraph);
            Serial.instance.End();

            Time.timeScale = 0; //게임 일시정지
            finishTime.text = "게임시간 : " + Math.Truncate(playTime * 100) / 100; //플레이 타임
            score.text = "점수 : " + coin12.instance.scoreT.ToString();
            Panel.SetActive(true);
            //-----------------------------DB 연결-----------------------------
            sql = String.Format("Insert into Game VALUES( {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8} )", date, userID, gameID, handle, "NULL",
                Device.instance.values[0], Device.instance.values[Device.instance.values.Count - 1], Math.Truncate(playTime * 100) / 100, coin.instance.scoreT);
            DBManager.DatabaseSQLAdd(sql);
            //-----------------------------------------------------------------
        }
    }
}