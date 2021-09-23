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

public class player3 : MonoBehaviour
{
    public float Speed = 0.5f;
    public GameObject Panel;

    float playTime; //게임 시간
    public Text pt; //게임 시간 표기
    public Text num; //운동 횟수 표기
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
    float delta = 10.0f; // 위아래로 이동가능한 (y)최대값
    float speed = 1.0f; // 이동속도

    public GameObject coinMax;
    public GameObject coinMin;//  생성할 오브젝트 A
    Vector3 coinVes;

    public GameObject circleGraph; //원 그래프

    private void Awake()
    {
        // 오늘날짜 + 현재시간
        date = DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초");
        date = DBManager.SqlFormat(date);

        userID = DBManager.SqlFormat(Data.Instance.UserID); // 나중에 로그인 구현시 Column 클래스로 만들어서 그때 객체화하기
        gameID = DBManager.SqlFormat(Data.Instance.GameID);     // 각자 게임 이름에 맞게 변경
        maxScore = 0;
        handle = DBManager.SqlFormat(Data.Instance.LongHandle); // 굴곡/신전
        Serial.instance.Passive();
    }
    void Start()
    {
        pos = gameObject.transform.position; //플레이어 백터

        print(gameObject.name + "시작");
        DBManager.DbConnectionChecek();
        slider.value = 180;
        DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0} and gameID = {1} ORDER BY gameScore DESC ", userID, gameID));
        while (DBManager.dataReader.Read())
        {
            maxScore = DBManager.dataReader.GetInt32(8);
            break;
        }
        DBManager.DBClose();
    }

    void Update()
    {
        float direction = 1.0f; //방향 및 속도조절
        //slider.value += delta * Mathf.Sin(Time.time * speed); //부드러운 움직임

        slider.value += delta * Mathf.Sin(Time.time * speed);
        if (slider.value >= 359)
        {
            direction *= -1;
            slider.value = 359;
        }
        else if (slider.value <= 0)
        {
            direction *= -1;
            slider.value = 0;
        }
        pos.y = slider.value / 360 * 9 - 5; //슬라이더 값을 플레이어의 y값 안에 넣음
        transform.position = pos; //플레이어의 포지션 변경
        int a = Mathf.FloorToInt(slider.value);
        Serial.instance.PaMotor("0" + Serial.instance.WriteAngle(a) + "2000");
        playTime += Time.deltaTime;
        pt.text = "횟수 : " + spawner3.instance.number.ToString(); //점수 spawner3.instance.number
        num.text = "시간 : " + Math.Truncate(playTime * 100) / 100; //시간

        if(Math.Truncate(playTime * 100) / 100 > 50)
        {
            Debug.Log("수동 끝~~");
            Device.instance.ResultCircle(circleGraph);
            Serial.instance.End();

            Time.timeScale = 0; //게임 일시정지
            finishTime.text = "운동시간 : " + Math.Truncate(playTime * 100) / 100; //플레이 타임           
            //-----------------------------DB 연결-----------------------------
            sql = String.Format("Insert into Game VALUES( {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8} )", date, userID, gameID, handle, "NULL",
                Device.instance.values[0], Device.instance.values[Device.instance.values.Count - 1], Math.Truncate(playTime * 100) / 100, coin.instance.scoreT);
            DBManager.DatabaseSQLAdd(sql);
            //-----------------------------------------------------------------
            Panel.SetActive(true);
        }

    }
}
