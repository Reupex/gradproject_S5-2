using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;

public class Drive3Tuto : MonoBehaviour
{
    public AudioSource backgroundAudio, effectAudio;
    public AudioClip pause;
    public AudioClip clear;
    public AudioClip lose;

    public float timer, count, scoretimer;
    public int score = 0;
    public GameObject player, speedPointer; //차랑, 속도계
    public GameObject savePoint;
    public GameObject obstacle;
    public GameObject Tutorial;
    public GameObject pausePanel, endPanel;
    public GameObject Life;
    public GameObject circleGraph;
    public Text scoreText, timeText, limitText, resultTime, resultScore;
    private int[] limitSpeed = { 60, 40, 50, 30, 55, 80, 55, 40, 60, 80, 50, 65, 40, 35, 20, 30, 50, 55, 60, 70, 45, 80, 15, 60, 35, 50, 25, 40, 50, 60, 50 };
    int saveNum = 0, courseNum = 0, tutoPage = 0, life = 3;
    public Slider device;
    public Sprite dead;
    float speedAngle;
    bool flag = false, isPause = false;

    private static Drive3Tuto _instance;
    public static Drive3Tuto instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = GetComponent<Drive3Tuto>();
    }
    // Start is called before the first frame update
    void Start()
    {
        limitText.text = limitSpeed[0].ToString();
        scoreText.text = "점수 : 0";
        Time.timeScale = 0f;
        Tutorial.transform.GetChild(tutoPage).gameObject.SetActive(true);

        Serial.instance.Active();
    }

    // Update is called once per frame
    void Update()
    {
        if (limitSpeed[saveNum] - 5 < (CarController.instance.m_Topspeed / 2) && limitSpeed[saveNum] + 5 > (CarController.instance.m_Topspeed / 2))
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
        timer += Time.deltaTime;
        timeText.text = "시간 : " + timer.ToString("N2");
        scoreText.text = "점수 : " + score.ToString();
    }

    public void speed()
    {
        speedAngle = 90 - (180 / (device.maxValue - device.minValue) * (device.value - device.minValue));
        CarController.instance.m_Topspeed = (device.value - device.minValue) * (160.0f / (device.maxValue - device.minValue));
        speedPointer.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, speedAngle);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "next")
        {
            effectAudio.PlayOneShot(pause);
            Time.timeScale = 0f;
            Tutorial.transform.GetChild(tutoPage).gameObject.SetActive(false);
            tutoPage++;
            Tutorial.transform.GetChild(tutoPage).gameObject.SetActive(true);
        }
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

            if (saveNum == 0)
            {
                NextPage();
            }
            if (saveNum == 3)
            {
                EndGame();
            }
            savePoint.transform.GetChild(saveNum).gameObject.SetActive(false);
            print(savePoint.transform.GetChild(saveNum).GetComponent<GameObject>());
            Debug.Log("save" + saveNum);
            if (saveNum == 29)
            {
                EndGame();
            }
            saveNum++;
            print(score);
            limitText.text = limitSpeed[saveNum].ToString();
            savePoint.transform.GetChild(saveNum).gameObject.SetActive(true);
            print(savePoint.transform.GetChild(saveNum).GetComponent<GameObject>());
        }
        if (other.name == "check")
        {
            obstacle.transform.GetChild(courseNum).gameObject.SetActive(false);
            courseNum++;
            if (courseNum > 3)
            {
                courseNum = 1;
            }
            obstacle.transform.GetChild(courseNum).gameObject.SetActive(true);
        }
    }

    public void EndGame()
    {
        Device.instance.ResultCircle(circleGraph);
        Serial.instance.End();
        Time.timeScale = 0f;
        endPanel.SetActive(true);
        resultTime.text = timer.ToString("N2");
        resultScore.text = "" + score;
    }

    public void NextPage()
    {
        effectAudio.PlayOneShot(pause);
        Tutorial.transform.GetChild(tutoPage).gameObject.SetActive(false);
        if (tutoPage == 4)
        {
            Time.timeScale = 1f;
        }
        if (tutoPage == 5)
        {
            Time.timeScale = 0f;
        }
        //페이지 증가
        tutoPage++;
        if (tutoPage == 3)
        {
            Time.timeScale = 1f;
        }
        Tutorial.transform.GetChild(tutoPage).gameObject.SetActive(true);
        if (tutoPage == 5)
        {
            Tutorial.transform.GetChild(tutoPage).gameObject.SetActive(false);
        }
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

}
