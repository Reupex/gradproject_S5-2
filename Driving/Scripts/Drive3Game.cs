using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityStandardAssets.Utility;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;

public class Drive3Game : MonoBehaviour
{
    public AudioSource backgroundAudio, effectAudio;
    public AudioClip pause;
    public AudioClip clear;
    public AudioClip lose;

    public float timer, count, scoretimer;
    public int score = 0, maxScore = 0;
    public GameObject player, speedPointer; //차랑, 속도계
    public GameObject savePoint;
    public GameObject obstacle;
    public GameObject pausePanel, endPanel, coursePanel;
    public GameObject Life, circleGraph;
    public Text scoreText, timeText, limitText, resultTime, resultScore;
    private int[] limitSpeed = { 60, 40, 50, 30, 55, 80, 55, 40, 60, 80, 50, 65, 40, 35, 20, 30, 50, 55, 60, 70, 45, 80, 25, 60, 35, 50, 25, 40, 50, 60, 50 };
    int saveNum = 0, courseNum = 0, life = 3;
    public Slider device;
    public Sprite dead;
    float speedAngle;
    string Querry;

    //--------- DBtest -----------
    string userID = DBManager.SqlFormat("ming");
    string gameID = DBManager.SqlFormat("310driving");
    int dbSpeed = 15;
    //--------------------------

    bool flag = false, isPause = false;

    //싱글톤
    private static Drive3Game _instance;
    public static Drive3Game instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        _instance = GetComponent<Drive3Game>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Serial 운동 설정
        if (Data.Instance.GameID.Substring(0, 1) == "1") // 능동운동
            Serial.instance.Active();
        else if (Data.Instance.GameID.Substring(0, 1) == "4") // 저항운동
            Serial.instance.Resistance();

        device.value = device.minValue;
        Time.timeScale = 1f;
        limitText.text = limitSpeed[0].ToString();
        scoreText.text = "점수 : 0";

        //최고점수
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
        if (limitSpeed[saveNum] - 5 < (CarController.instance.m_Topspeed/2)&& limitSpeed[saveNum] + 5 > (CarController.instance.m_Topspeed / 2))
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

        if (device.value == 0)
        {
            count += Time.deltaTime;
        }
        else count = 0;
        if (count > 5)
        {
            Time.timeScale = 0f;
            endPanel.SetActive(true);
        }
        timer += Time.deltaTime;
        timeText.text = "시간 : " + timer.ToString("N2");
        scoreText.text = "점수 : " + score.ToString();
    }


    //속도 조절
    public void speed()
    {
        speedAngle = 90 - (180 / (device.maxValue-device.minValue) * (device.value - device.minValue));
        CarController.instance.m_Topspeed = (device.value - device.minValue)*(160.0f/(device.maxValue - device.minValue));
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
        if(other.name == "courseCheck" && courseNum != 0 && courseNum !=3)
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
        print("Left" + 1.0f / 360.0f * (Mathf.Abs(device.minValue)) + " Right" + 1.0f / 360.0f * (Mathf.Abs(device.maxValue)));
        string date = DBManager.SqlFormat(DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초"));
        Device.instance.ResultCircle(circleGraph);
        Serial.instance.End();
        Data.Instance.FreshTree++;
        print(Data.Instance.FreshTree);

        //db에 데이터 저장
        Querry = string.Format("Insert into Game VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})", date, userID, gameID, "'홀번호'", "'x'", Device.instance.values[0], Device.instance.values[Device.instance.values.Count - 1], timer, score);
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
