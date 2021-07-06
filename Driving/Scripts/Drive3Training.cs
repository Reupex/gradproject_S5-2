using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityStandardAssets.Utility;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;

public class Drive3Training : MonoBehaviour
{
    public AudioSource backgroundAudio, effectAudio;
    public AudioClip pause;
    public AudioClip clear;
    public AudioClip lose;

    public float timer, count, scoretimer, speedAngle, term = 0;
    public int score = 0, rotateNum = 0, middleValue = 0, maxScore = 0;
    int userRotate = 4; //사용자 입력
    public GameObject player, speedPointer; //차랑, 속도계
    public GameObject savePoint;
    public GameObject obstacle;
    public GameObject pausePanel, endPanel, coursePanel;
    public GameObject circleGraph;
    public GameObject Life;
    public Text scoreText, timeText, limitText, resultTime, resultScore;
    private int[] limitSpeed = { 60, 40, 50, 25, 55, 80, 55, 40, 60, 80, 50, 65, 40, 35, 20, 30, 50, 45, 60, 70, 45, 80, 25, 60, 35, 50, 25, 35, 50, 70, 50 };
    int saveNum = 0, courseNum = 0, life = 3;
    public Slider device;
    public Sprite dead;

    string Querry;

    //--------- DBtest -----------
    string userID = DBManager.SqlFormat("ming");
    string gameID = DBManager.SqlFormat("410driving");
    int dbSpeed = 15;
    //--------------------------

    bool flag = false, isPause = false, isround = false;

    //싱글톤
    private static Drive3Training _instance;
    public static Drive3Training instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        _instance = GetComponent<Drive3Training>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Serial.instance.Active();
        CarController.instance.m_Topspeed = 40;
        Time.timeScale = 1f;
        limitText.text = limitSpeed[0].ToString();
        scoreText.text = "점수 : 0";
        speedPointer.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 0);
        DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0} and gameID = {1} ORDER BY gameScore DESC ", userID, gameID));
        while (DBManager.dataReader.Read())
        {
            maxScore = DBManager.dataReader.GetInt32(8);
            break;
        }
        DBManager.DBClose();
    }

    // Update is called once per frame
    void Update()
    {
        if (limitSpeed[saveNum] - 5 < device.value && limitSpeed[saveNum] + 5 > device.value)
        {
            flag = true;
            scoretimer += Time.deltaTime;
            if (scoretimer >= 1f)
            {
                score++;
                scoretimer = 0;
            }
        }
        else flag = false;

        timer += Time.deltaTime;
        timeText.text = "시간 : " + timer.ToString("N2");
        scoreText.text = "점수 : " + score.ToString();
    }

    //속도 조절
    public void speed()
    {
        if (device.value < 20 && device.value >= device.minValue)
        {
            if (middleValue == device.maxValue&&rotateNum!=userRotate-1)
            {
                rotateNum++;
            }
            middleValue = 0;
        }
        else if(device.value < 300 && device.value > 100)
        {
            middleValue = 180;
        }else if(device.value > 340 && device.value <= device.maxValue)
        {
            if (middleValue == device.minValue&&rotateNum!= -userRotate)
            {
                rotateNum--;
            }
            middleValue = 359;
        }
        speedAngle = -rotateNum*(90.0f/userRotate) - ((90.0f/userRotate) / 360) * device.value;
        CarController.instance.m_Topspeed = 80+rotateNum* (80.0f / userRotate) + ((80.0f/userRotate)/ 360)*device.value;
        print(CarController.instance.m_Topspeed);
        speedPointer.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, speedAngle);
    }

    private void OnTriggerEnter(Collider other)
    {
        //점수 10점 링 통과
        if (other.tag == "save")
        {
            if (flag)
            {
                effectAudio.PlayOneShot(clear);
                score += 10;
            }
            else
            {
                effectAudio.PlayOneShot(lose);
                Life.transform.GetChild(life - 1).gameObject.GetComponent<Image>().sprite = dead;
                life--;
                if (life == 0)
                {
                    EndGame();
                }
            }
            savePoint.transform.GetChild(saveNum).gameObject.SetActive(false);
            Debug.Log("save" + saveNum);
            if (saveNum == 29)
            {
                EndGame();
            }
            saveNum++;
            print(score);
            limitText.text = limitSpeed[saveNum].ToString();
            savePoint.transform.GetChild(saveNum).gameObject.SetActive(true);
        }
        //코스 1바퀴 돌았는지 체크
        if (other.name == "check")
        {
            obstacle.transform.GetChild(courseNum).gameObject.SetActive(false);
            courseNum++;
            obstacle.transform.GetChild(courseNum).gameObject.SetActive(true);
        }
        if (other.name == "courseCheck" && courseNum != 0 && courseNum != 3)
        {
            Time.timeScale = 0f;
            coursePanel.SetActive(true);
            coursePanel.transform.Find("CourseNum").GetComponent<Text>().text = "Course " + courseNum;
            coursePanel.transform.Find("report").GetComponent<Text>().text = "시간 : " + timer.ToString("N2") + "  점수 : " + score;
        }
    }

    //게임 종료
    public void EndGame()
    {
        Time.timeScale = 0f;
        endPanel.SetActive(true);
        resultTime.text = timer.ToString("N2");
        resultScore.text = "" + score;
        string date = DBManager.SqlFormat(DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초"));
        //Device.instance.ResultCircle(circleGraph);
        Serial.instance.End();
        Data.Instance.FreshTree++;
        print(Data.Instance.FreshTree);

        //db에 데이터 저장
        Querry = string.Format("Insert into Game VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})", date, userID, gameID, "'홀번호'", "'x'", 0, 359, timer, score);
        DBManager.DatabaseSQLAdd(Querry);
    }

    //일시정지
    public void PauseGame()
    {
        effectAudio.PlayOneShot(pause);
        if (!isPause)
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            isPause = true;
        }
        else
        {
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
            isPause = false;
        }
    }

    public void NextStage()
    {
        Time.timeScale = 1f;
        coursePanel.SetActive(false);
    }

}
