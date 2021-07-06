using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class ArrowUI: MonoBehaviour
{
    public GameObject ResultPanel;
    public GameObject PausePanel;
    public GameObject StagePanel;
    public GameObject Border_Life_Panel;
    public Text Monster_Spawn_Slot_Up;
    public bool pause = false; // 일시정지시 몬스터 움직임을 없애기 위해서 선언
    public bool check = false;
    // public GameObject origin;

    public Text MaxScore_Text; // 최고점수
    public Text test;          // 디버깅용 - 게임씬 - 화면 상단에 text오브젝트 두고 상태나 오류확인
    public Text test_Result;   // 빌드시 디버깅용 - 게임씬 - 결과패널에 GAMEOVER 텍스트 오브젝트 유니티에서 넣기 - 상태나 오류 확인
    public int maxScore;
    public string date;
    public string userID;
    public string gameID;
    public string sql;
    public Dictionary<string, float> Dic = new Dictionary<string, float>();
    public List<string> series1Data2;
    public GameObject circleGraph;

    // 싱글톤 패턴
    #region Singleton
    private static ArrowUI _Instance;    // 싱글톤 패턴을 사용하기 위한 인스턴스 변수, static으로 선언하여 어디서든 접근 가능
    
    // 인스턴스에 접근하기 위한 프로퍼티
    public static ArrowUI Instance
    {
        get { return _Instance; }          // ArrowUI 인스턴스 변수를 리턴
    }

    // 인스턴스 변수 초기화
    void Awake()
    {
        _Instance = this;  // _uiManager에 UIManager의 컴포넌트(자기 자신)에 대한 참조를 얻음

        // 오늘날짜 + 현재시간
        date = DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초");
        date = SqlFormat(date);

        userID = SqlFormat("KTT"); // 나중에 로그인 구현시 Column 클래스로 만들어서 그때 객체화하기
        gameID = SqlFormat("21arrow");     // 각자 게임 이름에 맞게 변경

        DBManager.DbConnectionChecek();

        // 최고 점수 가져오는 코드
        DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0} and gameID = {1} ORDER BY gameScore DESC ", userID, gameID));
        while (DBManager.dataReader.Read())
        {
            maxScore = DBManager.dataReader.GetInt32(8);
            MaxScore_Text.text = "최고점수 : " + maxScore;
            break;
        }
        DBManager.DBClose();
    }
    public string SqlFormat(string sql)
    {
        return string.Format("\"{0}\"", sql);
    }
    #endregion

    public void Player_damage_on()  // 데미지 입었을때
    {
        Border_Life_Panel.GetComponent<Image>().color = ArrowGame.Instance.HexToColor("EF2D2378");
        Invoke("Player_damage_off", 0.3f);
    }
    public void Player_damage_off() // 원상태로 되돌리기
    {
        Border_Life_Panel.GetComponent<Image>().color = ArrowGame.Instance.HexToColor("5E5E0C31");
    }

    public void ui()
    {
        var Life = ArrowGame.Instance.Life;
        var life = ArrowGame.Instance.life;
        //틀린경우 라이프 감소
        life--;                          // 인덱스 1 감소
        if (life >= 0)
            Life[life+1].SetActive(false);     // 아까 life를 2로 초기화한 이유는 인덱스 값이 0 1 2 이므로 3번째 값을 호출할 때 2를 사용해서 2로 초기화 시킴.
        ArrowSound.Instance.Player_damage();
        Player_damage_on();
        ArrowGame.Instance.life = life;
        //life == 0이 되면 게임오버
        if (life < 0)
        {
            GameOver();                  // 생명이 없으므로 게임 오버메소드 호출
        }
    }
    
    public void GameOver() //게임오버+결과화면 함수
    {
        float num1 = (float)((ArrowGame.Instance.SliderMaxValue - ArrowGame.Instance.SliderMinValue) * ((Double)1 / (Double)140));
        //origin.transform.GetComponent<Image>().fillAmount = num1;

        //float Rotate = (float)((90 - ArrowGame.Instance.SliderMinValue) * ((Double)18 / (Double)7));
        //origin.transform.rotation = Quaternion.Euler(0, 0, Mathf.Abs(Rotate)); // Mathf.Abs : 절대값

        Time.timeScale = 0f;
        // 캔버스의 서브카메라가 결과 패널을 가려서 false 시킴
        ArrowSound.Instance.Gameover();
        int score = ArrowGame.Instance.count * 10;
        string playtime = ArrowGame.Instance.time.ToString("N2") + "초";
        float playtime_REAL = (float)(Math.Truncate(ArrowGame.Instance.time*100)/100); // 소수점 2째자리까지 나타내주는 코드 - 이하 버림
        ArrowGame.Instance.RESULT_score.text = "점수 : " + score.ToString();
        ArrowGame.Instance.RESULT_time.text = "게임시간 : " + playtime;
        ResultPanel.SetActive(true);

        // 게임 결과 저장 코드

        // 디바이스 x
        string sql = string.Format("Insert into Game(date, userID, gameID, hole, numOfRotation, minRotationAngle, maxRotationAngle," +
            "gamePlayTime, gameScore) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
            date, userID, gameID, SqlFormat("홀번호"), SqlFormat("x"), ArrowGame.Instance.SliderMinValue, ArrowGame.Instance.SliderMaxValue, playtime_REAL, score);
        
        // 디바이스 o
        //string sql1 = string.Format("Insert into Game(date, userID, gameID, hole, numOfRotation, minRotationAngle, maxRotationAngle," +
        //    "gamePlayTime, gameScore)" + "VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})", 
        //    date, userID, gameID, "'홀번호'", "'x'", Device.instance.values[0], Device.instance.values[-1], playtime_REAL, score);

        //Debug.Log("date : " + date);
        //Debug.Log("userID : " + userID);
        //Debug.Log("gameID : " + gameID);
        DBManager.DatabaseSQLAdd(sql);

        // 게임 끝나는 부분 스크립트에 아래 코드 추가
        Device.instance.ResultCircle(circleGraph);
        Serial.instance.End();
    }

    public void Menu() // 일시정지 버튼을 눌렀을때
    {
        Time.timeScale = 0f;
        ArrowSound.Instance.Btn_Click();
        pause = true;
        PausePanel.SetActive(true);
    }

    public void Play() // 계속하기 버튼을 눌렀을때
    {
        ArrowSound.Instance.Btn_Click();
        Time.timeScale = 1f;
        pause = false;
        PausePanel.SetActive(false);
    }

    public void StagePanel_on(int i)
    {
        ArrowSound.Instance.stageUp();
        StagePanel.SetActive(true);

        if (i == 1 || i == 3) // 3스테이지 or 5스테이지
        {
            Monster_Spawn_Slot_Up.gameObject.SetActive(true);
            check = true;
            Invoke("StagePanel_off", 1.5f);
        }
        else
        {
            Invoke("StagePanel_off", 1.2f);
        }
        ArrowUI.Instance.pause = true;
    }

    public void StagePanel_off()
    {
        ArrowUI.Instance.pause = false;
        if (check)
            Monster_Spawn_Slot_Up.gameObject.SetActive(false);
        
        StagePanel.SetActive(false);
    }

 
}