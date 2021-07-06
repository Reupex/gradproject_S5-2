using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;
using System;

public class Drive12Game : MonoBehaviour
{
    public AudioSource backgroundAudio, effectAudio;
    public AudioClip pause;
    public AudioClip clear;
    public AudioClip coin;

    public Slider device;
    public Text timeText, scoreText, resultTime, resultScore;
    public GameObject pausePanel, endPanel, coursePanel;
    public GameObject savePoint;
    public GameObject obstacle;
    public GameObject gainpoint;
    public GameObject circleGraph;

    public float timer = 0f;
    public int score = 0, maxScore = 0;
    public bool isPause = false;
    int saveNum = 0, courseNum = 0;
    float speedAngle;
    string Querry;

    //--------- test -----------
    string userID = DBManager.SqlFormat("ming");
    string gameID = DBManager.SqlFormat("110driving");
    int dbSpeed = 15;
    //--------------------------

    private static Drive12Game _instance;
    public static Drive12Game instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = GetComponent<Drive12Game>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Serial 운동 설정
        if (Data.Instance.GameID.Substring(0, 1) == "1") // 능동운동
            Serial.instance.Active();
        else if (Data.Instance.GameID.Substring(0, 1) == "4") // 저항운동
            Serial.instance.Resistance();

        device.value = (device.maxValue + device.minValue) / 2;
        Time.timeScale = 1f;
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
        timer += Time.deltaTime;
        timeText.text = "시간 : " + timer.ToString("N2");
        scoreText.text = "점수 : " + score.ToString();

    }

    public void Rotate()
    {
        CarUserControl.instance.handle(-0.3f+0.6f/(device.maxValue - device.minValue)*(device.value -  device.minValue));
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "gain")
        {
            effectAudio.PlayOneShot(coin);
            score++;
            other.gameObject.SetActive(false);

        }
        if (other.tag == "save")
        {
            effectAudio.PlayOneShot(clear);
            score += 10;

            savePoint.transform.GetChild(saveNum).gameObject.SetActive(false);
            Debug.Log("save" + saveNum);
            if (saveNum == 29)
            {
                EndGame();
            }
            saveNum++;
            savePoint.transform.GetChild(saveNum).gameObject.SetActive(true);
        }
        if (other.name == "check")
        {
            obstacle.transform.GetChild(courseNum).gameObject.SetActive(false);
            gainpoint.transform.GetChild(courseNum).gameObject.SetActive(false);
            courseNum++;
            obstacle.transform.GetChild(courseNum).gameObject.SetActive(true);
            gainpoint.transform.GetChild(courseNum).gameObject.SetActive(true);
        }
        if (other.tag == "obstacle")
        {
            EndGame();
        }
        if (other.name == "courseCheck" && courseNum != 0 && courseNum != 3)
        {
            Time.timeScale = 0f;
            coursePanel.SetActive(true);
            coursePanel.transform.Find("CourseNum").GetComponent<Text>().text = "Course " + courseNum;
            coursePanel.transform.Find("report").GetComponent<Text>().text = "시간 : " + timer.ToString("N2") + "  점수 : " + score;
        }
    }
    public void EndGame()
    {
        Time.timeScale = 0f;
        endPanel.SetActive(true);
        resultTime.text = timer.ToString("N2");
        resultScore.text = "" + score;
        Device.instance.ResultCircle(circleGraph);
        Serial.instance.End();
        Data.Instance.FreshTree++;
        print(Data.Instance.FreshTree);

        string date = DBManager.SqlFormat(DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초"));

        Querry = string.Format("Insert into Game VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})", date, userID, gameID, "'홀번호'", "'x'", Device.instance.values[0], Device.instance.values[Device.instance.values.Count - 1], timer, score);
        DBManager.DatabaseSQLAdd(Querry);
    }

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
