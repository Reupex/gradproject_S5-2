using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeUI : MonoBehaviour
{
    public float GameTime = 0;
    public Text GameTimeText;
    public GameObject stopPanel;
    public Text resultTimeText;
    public Text endTimeText;
    private float startTime;
    private float total = 0;
    public Text scoreText;
    public Text endScore;
    public Text countUp_text;
    private int gameScore = 0;
    public Text MaxScore_Text; // 최고점수
    public Text test;          // 디버깅용 - 게임씬 - 화면 상단에 text오브젝트 두고 상태나 오류확인
    public Text test_Result;   // 빌드시 디버깅용 - 게임씬 - 결과패널에 GAMEOVER 텍스트 오브젝트 유니티에서 넣기 - 상태나 오류 확인
    public int maxScore;
    public string date;
    public string userID;
    public string gameID;
    public string sql;
    public string handle;

    //싱글톤
    public static EscapeUI instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        date = DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초");
        date = DBManager.SqlFormat(date);
        userID = DBManager.SqlFormat(Data.Instance.UserID);
        gameID = DBManager.SqlFormat(Data.Instance.GameID);
        maxScore = 0;
        MaxScore_Text.text = "최고점수 : " + maxScore;

        // 핸들번호 저장
        if (Data.Instance.GameID.Substring(1, 1) == "2") // 내/외회전, 수평 회전
            handle = DBManager.SqlFormat(Data.Instance.ShortHadle);
        else // 내/외전, 굴곡/신전, 수직 회전
            handle = DBManager.SqlFormat(Data.Instance.LongHandle);
    }


    void Start()
    {
        startTime = 0f;

        DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0} and gameID = {1} ORDER BY gameScore DESC ", userID,gameID));

        while (DBManager.dataReader.Read())                            // 쿼리로 돌아온 레코드 읽기
        {
            maxScore = DBManager.dataReader.GetInt32(8);
            MaxScore_Text.text = "최고점수 : " + maxScore + "점";
            break; // 내림차순 정렬이므로 처음에 한 번만 레코드값을 가져오면 된다.
        }
        DBManager.DBClose();

    }
    void Update()   //시간 출력 소수 둘째자리까지
    {
        GameTime = GameTime + Time.deltaTime;
        if (GameTime < 10)
        {
            GameTimeText.text = "시간 : 0" + (System.Math.Truncate(GameTime * 100) / 100);
            //text는 string 타입으로 (int)GameTime만 넣으면 안됨
        }
        else
            GameTimeText.text = "시간 : " + (System.Math.Truncate(GameTime * 100) / 100);
    }
    public void GamePause() //게임 일시정지
    {
        stopPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Restart()   //게임 계속하기
    {
        EscapeSound.instance.menu_Click();
        stopPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void printTuToTime()
    {
        Time.timeScale = 0f;
        total = total + (float)GameTime;
        if (GameTime < 10)
        {
            resultTimeText.text = "소요 시간 : 0" + (System.Math.Truncate(GameTime * 100) / 100) + "초";
            //text는 string 타입으로 (int)GameTime만 넣으면 안됨
        }
        else
        {
            resultTimeText.text = "소요 시간 : " + (System.Math.Truncate(GameTime * 100) / 100) + "초";
        }
    }

    public void printTime() //라운드 끝나면 소요시간 출력, 점수 입력
    {
        Time.timeScale = 0f;
        total = total + (float)GameTime;
        if (GameTime < 10)
        {
            resultTimeText.text = "소요 시간 : 0" + (System.Math.Truncate(GameTime * 100) / 100) + "초";
            //text는 string 타입으로 (int)GameTime만 넣으면 안됨
        }
        else
        {
            resultTimeText.text = "소요 시간 : " + (System.Math.Truncate(GameTime * 100) / 100) + "초";
        }
        if (EscapeGame.instance.startLv == 1) //쉬움
        {
            if((EscapeGame.instance.level == 1) || (EscapeGame.instance.level == 2))
            {
                if ((GameTime > 0) && (GameTime <= 18))
                    gameScore += 30;
                else if ((GameTime > 18) && (GameTime <= 25))
                    gameScore += 20;
                else if (GameTime > 25)
                    gameScore += 10;
            }
            if ((EscapeGame.instance.level == 3) || (EscapeGame.instance.level == 4))
            {
                if ((GameTime > 0) && (GameTime <= 20))
                    gameScore += 35;
                else if ((GameTime > 20) && (GameTime <= 25))
                    gameScore += 25;
                else if (GameTime > 25)
                    gameScore += 10;
            }
        }
        if (EscapeGame.instance.startLv == 2) //보통
        {
            if ((EscapeGame.instance.level == 1) || (EscapeGame.instance.level == 2))
            {
                if ((GameTime > 0) && (GameTime <= 18))
                    gameScore += 40;
                else if ((GameTime > 18) && (GameTime <= 25))
                    gameScore += 30;
                else if (GameTime > 25)
                    gameScore += 10;
            }
            if ((EscapeGame.instance.level == 3) || (EscapeGame.instance.level == 4))
            {
                if ((GameTime > 0) && (GameTime <= 20))
                    gameScore += 45;
                else if ((GameTime > 20) && (GameTime <= 25))
                    gameScore += 35;
                else if (GameTime > 25)
                    gameScore += 15;
            }
        }
        if (EscapeGame.instance.startLv == 3) //어려움
        {
            if ((EscapeGame.instance.level == 1) || (EscapeGame.instance.level == 2))
            {
                if ((GameTime > 0) && (GameTime <= 20))
                    gameScore += 70;
                else if ((GameTime > 20) && (GameTime <= 27))
                    gameScore += 60;
                else if (GameTime > 27)
                    gameScore += 20;
            }
            if ((EscapeGame.instance.level == 3) || (EscapeGame.instance.level == 4))
            {
                if ((GameTime > 0) && (GameTime <= 22))
                    gameScore += 75;
                else if ((GameTime > 22) && (GameTime <= 27))
                    gameScore += 65;
                else if (GameTime > 27)
                    gameScore += 25;
            }
        }
        if (EscapeGame.instance.startLv == 4) //매우 어려움
        {
            if ((EscapeGame.instance.level == 1) || (EscapeGame.instance.level == 2))
            {
                if ((GameTime > 0) && (GameTime <= 22))
                    gameScore += 80;
                else if ((GameTime > 22) && (GameTime <= 27))
                    gameScore += 70;
                else if (GameTime > 27)
                    gameScore += 30;
            }
            if ((EscapeGame.instance.level == 3) || (EscapeGame.instance.level == 4))
            {
                if ((GameTime > 0) && (GameTime <= 22))
                    gameScore += 100;
                else if ((GameTime > 22) && (GameTime <= 27))
                    gameScore += 75;
                else if (GameTime > 27)
                    gameScore += 35;
            }
        }

        scoreText.text = "점수 : " + gameScore + "점";
    }

    public void endTime()
    {
        Debug.Log(total);
        endTimeText.text = "게임시간 : " + (System.Math.Truncate(total * 100) / 100) + "초";
    }

    public void EndScore()  //게임 모두 끝났을 때 시간 당 점수 추가, 디비 입력
    {
        endScore.text = "점수 : " + gameScore + "점";

        float playtime_REAL = (float)(Math.Truncate(total * 100) / 100);

        // 게임 결과 저장 코드
        if (Data.Instance.GameID.Substring(0, 1) == "3") // 수평 회전
        {
            string sql = String.Format("Insert into Game VALUES( {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8} )", date, userID, gameID, handle, EscapeGame.instance.TotalRotate,
                0, 359, playtime_REAL, gameScore);
        }
        else // 내/외전, 굴곡/신전
        {
            string sql = string.Format("Insert into Game VALUES( {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8} )", date, userID, gameID, handle, "NULL",
            Device.instance.values[0], Device.instance.values[Device.instance.values.Count - 1], playtime_REAL, gameScore);
        }
        Debug.Log("sqlite.Instance.date : " + date);

        DBManager.DatabaseSQLAdd(sql);
    }

    public void setZero()   //게임 시간 0으로 초기화
    {
        GameTime = startTime;
    }
    public void Restartbtn()    //재시작 버튼
    {
        EscapeScene.instance.GameStart();
        stopPanel.SetActive(false);
    }
}
