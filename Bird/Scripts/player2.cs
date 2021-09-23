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

public class player2 : MonoBehaviour
{
    //public GameObject startNel; //시작할 때 숫자 세기

    public float Speed = 0.5f;
    public GameObject Panel;
    //public GameObject startPanel;
    public GameObject wall;
    public GameObject coin;

    float playTime; //게임 시간
    float startTime; //시작할 때 숫자 세기
    public Text pt; //게임 시간 표기
    public Text debug; //오류 잡기
    public Text finishTime, score; //게임 끝난 시간 표기
    public Text oneTwoThree; //시작할 때 숫자 세기
    //private int Timer = 0;
    //private int a = 1;

    public int scoreT = 0;
    public Text MaxScore_Text;
    public string date;
    public string userID;
    public string gameID;
    public string sql;
    public string handle;
    public int maxScore;
    public int dbSpeed;
    public float sliderMax=359, sliderMin=0;
    public int maxCircle = 6;
    public int UPcircle;  
    public float totalCircle=0;

    public Slider slider;
    public float position;
    public float memoPos;
    Vector3 pos;

    public GameObject coinMax;
    public GameObject coinMin;//  생성할 오브젝트 A
    Vector3 coinVes;

    int middleValue = 0;

    public GameObject circleGraph; //원 그래프

    private void Awake()
    {
        // Serial 운동 설정
        Serial.instance.Active();

        // 오늘날짜 + 현재시간
        date = DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초");
        date = DBManager.SqlFormat(date);

        userID = DBManager.SqlFormat(Data.Instance.UserID); // 나중에 로그인 구현시 Column 클래스로 만들어서 그때 객체화하기
        gameID = DBManager.SqlFormat(Data.Instance.GameID);     // 각자 게임 이름에 맞게 변경
        maxScore = 0;
        handle = DBManager.SqlFormat(Data.Instance.LongHandle); // 핸들번호 저장
        coinVes = coin.gameObject.transform.position;
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
    void Start()
    {
        print(gameObject.name + "시작");
        slider.value = 180f;
        DBManager.DbConnectionChecek();
        UPcircle = 0;

        DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0} and gameID = {1} ORDER BY gameScore DESC ", userID, gameID));
        while (DBManager.dataReader.Read())
        {
            maxScore = DBManager.dataReader.GetInt32(8);
            break;
        }
        DBManager.DBClose();
    }

    IEnumerator StartCountDown()
    {
        oneTwoThree.text = "3";
        yield return new WaitForSeconds(1.0f);
        oneTwoThree.text = "2";
        yield return new WaitForSeconds(1.0f);
        oneTwoThree.text = "1";
        yield return new WaitForSeconds(1.0f);
        oneTwoThree.text = "시작";
    }

    void Update()
    {
        float cul = 0;
        pos = gameObject.transform.position; //플레이어 백터
        cul = (slider.value / 360 * 9) / maxCircle-2;
        what();

        if (UPcircle == 1)
        {
            pos.y = cul + 3;
            totalCircle += 1;
        }
        else if (UPcircle == 0)
        {
            pos.y = cul;
            totalCircle += 1;
        }
        else if (UPcircle == -1)
        {
            pos.y = cul - 3;
            totalCircle += 1;
        }

        transform.position = pos; //플레이어의 포지션 변경
        playTime += Time.deltaTime;
        pt.text = "점수 : " + UPcircle; //coin
        MaxScore_Text.text = slider.value.ToString();

    }

    public void what()
    {
        if (slider.value < 20 && slider.value >= slider.minValue)
        {
            if (middleValue == slider.maxValue)
            {
                UPcircle++;
                print(UPcircle);
            }
            middleValue = 0;
        }
        else if (slider.value < 300 && slider.value > 100)
        {
            middleValue = 180;
        }
        else if (slider.value > 340 && slider.value <= slider.maxValue)
        {
            if (middleValue == slider.minValue)
            {
                UPcircle--;
                print(UPcircle);
            }
            middleValue = 359;
        }
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
            Panel.SetActive(true);
            //-----------------------------DB 연결-----------------------------
            string sql = String.Format("Insert into Game VALUES( {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8} )", date, userID, gameID, handle, totalCircle,
                0, 359, Math.Truncate(playTime * 100) / 100, scoreT);
            DBManager.DatabaseSQLAdd(sql);
            //-----------------------------------------------------------------
        }
    }
}