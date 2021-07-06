using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;

public class Drive12Tuto : MonoBehaviour
{
    public AudioSource backgroundAudio, effectAudio;
    public AudioClip pause; //일시정지 효과음
    public AudioClip clear; //10점 획득 효과음
    public AudioClip coin;  //1점 획득 효과음

    public Slider device;
    public Text timeText, scoreText, resultTime, resultScore;
    public GameObject player;
    public GameObject pausePanel, endPanel;
    public GameObject savePoint;
    public GameObject obstacle;
    public GameObject gainpoint;
    public GameObject Tutorial;
    public GameObject fail;
    public GameObject circleGraph;

    public float timer = 0f;
    public int score = 0, maxScore = 0;
    public bool isPause = false;
    int saveNum = 0, courseNum = 0, tutoPage = 0;

    private static Drive12Tuto _instance;
    public static Drive12Tuto instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = GetComponent<Drive12Tuto>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
        Tutorial.transform.GetChild(tutoPage).gameObject.SetActive(true);
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
        CarUserControl.instance.handle(-0.3f + 0.6f / (device.maxValue - device.minValue) * (device.value - device.minValue));
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
            if (saveNum == 0)
            {
                NextPage();
            }
            if (saveNum == 3)
            {
                EndGame();
            }
            savePoint.transform.GetChild(saveNum).gameObject.SetActive(false);
            Debug.Log("save" + saveNum);
            if (saveNum == 29)
            {
                EndGame();
            }
            saveNum++;
            print(score);
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
            Time.timeScale = 0f;
            fail.SetActive(true);
        }
    }
    public void FailGame()
    {
        fail.SetActive(false);
        player.transform.position = new Vector3(-26.66f, -0.5500001f, -78.74f);
        player.transform.localEulerAngles = new Vector3(0f, 270f, 0f);
        for (int i = 0; i < gainpoint.transform.GetChild(courseNum).gameObject.transform.childCount; i++)
        {
            gainpoint.transform.GetChild(courseNum).gameObject.transform.GetChild(i).gameObject.SetActive(true);
        }
        timer = 0;
        score = 0;
        device.value = 0f;
        Time.timeScale = 1f;
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

    public void PauseGame()
    {
        effectAudio.PlayOneShot(pause);
        if (!isPause)
        {
            Tutorial.transform.GetChild(tutoPage).gameObject.SetActive(false);
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

    public void NextPage()
    {
        effectAudio.PlayOneShot(pause);
        Tutorial.transform.GetChild(tutoPage).gameObject.SetActive(false);
        if (tutoPage == 4)
        {
            Time.timeScale = 1f;
        }
        tutoPage++;
        Tutorial.transform.GetChild(tutoPage).gameObject.SetActive(true);

    }

}
