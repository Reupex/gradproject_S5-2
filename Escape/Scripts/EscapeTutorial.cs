using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EscapeTutorial : MonoBehaviour
{
    #region 
    public Text CodeText;
    private List<int> codeNum = new List<int>();
    private int randNum;
    private string line;
    public Text firstCode;
    public Text secondCode;
    public Text LastText;
    public GameObject[] explains;
    public Slider slider;
    public Text sliderValue;
    private float preValue = 0f;
    private float nowValue = 0f;
    private bool firstCheck = false;
    private bool secondCheck = false;
    public GameObject resultPanel;
    public GameObject retryPanel;
    public GameObject explainPanel;
    public GameObject whitePanel;
    private int i = 0;
    public GameObject _key;
    public GameObject[] arrow;
    public GameObject tutoScene;
    public float maxTime = 0;  //다이얼 넘어가기 위한 슬라이더 유지 시간
    public int min = 0;
    public Slider sc0_slider;
    public bool timer = false;
    public float middleAngleT;
    public Text sc0_Text;
    public GameObject mainSlider;
    public Text middle_Text;
    public GameObject middle_obj;
    public Text[] emptyText;
    private int circleNum;
    private float rotateRangeR = 0;
    private float rotateRangeL = 0;
    private int selectRange;
    private float rotateValue = 0;
    private int rand;
    private bool auto_;
    public float check_now;
    bool isCali;
    public GameObject[] circle;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Serial 운동 설정
        if (Data.Instance.GameID.Substring(0, 1) == "1") // 능동운동
            Serial.instance.Active();
        else if (Data.Instance.GameID.Substring(0, 1) == "4") // 저항운동
            Serial.instance.Resistance();

        EscapeUI.instance.setZero();
        Time.timeScale = 1f;
        timer = false;
        auto_ = false;
        isCali = true;
        check_now = 0f;
    }
    void Update()
    {
        if ((check_now - 2 <= nowValue) && (check_now + 2 >= nowValue) && !isCali)
        {
            timer = true;
            print("nowValue == check_now");
        }
        else
        {
            timer = false;
        }
        check_now = nowValue;

        suc_scene();
        Timer_();
        sc0_slider.minValue = slider.minValue;
        sc0_slider.maxValue = slider.maxValue;
        sc0_slider.value = sc0_slider.minValue;
        middleAngleT = (sc0_slider.minValue + sc0_slider.maxValue) / 2;
        middle_Text.text = "중앙값 : " + (int)middleAngleT;
    }

    private void gamePlay()
    {
        EscapeUI.instance.setZero();
        for (int i = 0; i < 3; i++)
        {
            randNum = Random.Range(0, 99);
            codeNum.Add(randNum);
        }
        auto_ = true;
        line = string.Join(" ", codeNum.ToArray());
        CodeText.text = "암호 : " + line;
        if ((0 <= codeNum[0]) && (codeNum[0] <= 9))
            firstCode.text = "0" + (int)codeNum[0];
        else firstCode.text = "" + (int)codeNum[0];

        circleRotate(); //난이도 따라 암호나타나는 범위 결정
        firstwrongCode();
        circle[0].transform.Rotate(0, 0, rotateValue);
        LastText.text = "" + (int)codeNum[2];
    }

    private void circleRotate() //난이도 따라 암호나타나는 범위 결정
    {      
        rotateRangeR = Random.Range(-45, -15);
        rotateRangeL = Random.Range(15, 45);  
        selectRange = Random.Range(0, 2);
        if (selectRange == 0)
            rotateValue = rotateRangeR;
        else
            rotateValue = rotateRangeL;
    }
    private void firstwrongCode()
    {
        for (int i = 0; i <= 3; i++)
        {
            rand = Random.Range(0, 99);
            while (rand == int.Parse(firstCode.text)) //정답이 두개 이상 있으면 안됨
            {
                rand = Random.Range(0, 99);
                if (rand != int.Parse(firstCode.text))
                    break;
            }
            if ((0 <= rand) && (rand <= 9))
                emptyText[i].text = "0" + rand;
            else emptyText[i].text = "" + rand;
        }
    }
    private void scdWrongCode()
    {
        for (int i = 4; i <= 6; i++)
        {
            rand = Random.Range(0, 99);
            while (rand == int.Parse(secondCode.text)) //정답이 두개 이상 있으면 안됨
            {
                rand = Random.Range(0, 99);
                if (rand != int.Parse(secondCode.text))
                    break;
            }
            if ((0 <= rand) && (rand <= 9))
                emptyText[i].text = "0" + rand;
            else emptyText[i].text = "" + rand;
        }
    }
    public void MoveSlider()
    {
        float x;
        preValue = nowValue;
        nowValue = (float)slider.value;
        sliderValue.text = "" + (int)slider.value;

        //circleNum 정하기 (돌릴 원)
        if (secondCheck == false) //정답 다 못 맞춤
        {
            if (firstCheck == false)
                circleNum = 0;
            else if (firstCheck == true)
                circleNum = 1;
        }
        x = Mathf.Abs(180 / (middleAngleT - slider.minValue));

        if (nowValue > preValue) //오른쪽
            circle[circleNum].transform.Rotate(0, 0, -(float)x);
        if (nowValue < preValue) //왼쪽
            circle[circleNum].transform.Rotate(0, 0, (float)x);
    }
    public void CheckButton()
    {
        if (secondCheck == false) {
            if (firstCheck == false)
                circleNum = 0;
            else if(firstCheck==true)
                circleNum = 1;
        }

        float zAxis = 360f - circle[circleNum].transform.eulerAngles.z;
        if ((((355 <= zAxis) && ((zAxis <= 0) || (zAxis <= 360))) || (0 <= zAxis) && (zAxis <= 5))) //성공 범위
        {         
            if (circleNum == 0) 
            {
                EscapeSound.instance.Success();
                firstCheck = true;
                arrow[0].SetActive(false);
                arrow[1].SetActive(true);
                secondCode.text = "" + (int)codeNum[1];
                circleRotate();
                scdWrongCode();
                circle[1].transform.Rotate(0, 0, rotateValue);
            }
            if (circleNum == 1)
            {
                EscapeSound.instance.OpenDoor(); //탈출 소리
                secondCheck = true;
            }
        }
        else //틀림
        {
            if (circleNum == 0)
                firstCheck = false;
            if (circleNum == 1)
                secondCheck = false;
            retryPanel.SetActive(true);
            EscapeSound.instance.Cave();
            Invoke("DestroyTry", 1.5f);
        }
    }  

    public void suc_scene()
    {
        if ((firstCheck == true) && (secondCheck == true))
        {
            Time.timeScale = 0f;
            resultPanel.SetActive(true);
            EscapeUI.instance.printTuToTime();
        }
    }

    public void DestroyTry()
    {
        retryPanel.SetActive(false);
    }

    public void nextExplain()
    {
        if (i == 0)
        {
            gamePlay();
            explains[i].SetActive(false);
            i++; //i=1
            tutoScene.SetActive(false);
            explains[i].SetActive(true);
            mainSlider.SetActive(false);
            middle_obj.SetActive(false);
            Time.timeScale = 0f;
            EscapeUI.instance.setZero();
        }
        else if (i == 1)
        {
            explains[i].SetActive(false);
            i++; //i=2
            _key.SetActive(true);
            explains[i].SetActive(true);
        }
        else if (i == 2)
        {
            explains[i].SetActive(false);
        }
        else if (i == 3)
        {
            explains[i - 1].SetActive(false);
            explains[i].SetActive(true);
            i++;
        }
        else if (i == 4)
        {
            explains[i - 1].SetActive(false);
            explains[i].SetActive(true);
        }
        else
            for (int i = 0; i < 5; i++)
            {
                explains[i].SetActive(false);
            }
    }

    public void sliderDial()
    {
        sc0_Text.text = "" + (int)sc0_slider.value;
        if (middleAngleT == (int)sc0_slider.value)
        {
            isCali = false;
        }

        else
            timer = false;
    }

    public void Timer_()
    {
        if (timer == true)
        {
            maxTime += Time.deltaTime;
            if (maxTime >= 1)
            {
                min++;
                maxTime -= 1;
                Debug.Log("min : " + min);
                if (min == 3)
                {
                    if (auto_)
                    {
                        CheckButton();
                        maxTime = 0;
                        min = 0;
                    }
                    else
                    {
                        slider.value = sc0_slider.value;
                        nextExplain();
                        maxTime = 0;
                        min = 0;
                    }
                    timer = false;
                }
            }
        }
        else {
            maxTime = 0;
            min = 0;
        }
    }

    public void Des_explain()
    {
        for (int i = 0; i < 5; i++)
        {
            explains[i].SetActive(false);
        }
        whitePanel.SetActive(false);
    }
    public void stopCount()
    {
        if (i == 2)
            i = 3;
        else
            i = -1;
    }
}
