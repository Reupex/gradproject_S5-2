using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscapeGame : MonoBehaviour
{
    #region
    public int startLv;
    public int level = 1;
    public Text CodeText;
    private List<int> codeNum = new List<int>();
    private List<int> ShowCode = new List<int>();
    private int randNum;
    private string line;
    public Text firstCode;
    public Text secondCode;
    public Text thirdCode;
    public Text fourthCode;
    public Text LastText;
    public GameObject[] circle;
    public GameObject[] arrow;
    private bool firstCheck = false;
    private bool secondCheck = false;
    private bool thirdCheck = false;
    private bool fourthCheck = false;
    public Text dial_text;
    public Slider slider;
    private float preValue;
    private float nowValue;
    public Text sliderValue;
    public GameObject resultPanel;
    public GameObject[] scene;
    public Text[] emptyText;
    public GameObject Key;
    public GameObject clearPanel;
    public GameObject redPanel;
    public GameObject chose_level;
    public float SliderMinValue;
    public float SliderMaxValue;
    public GameObject circleGraph;
    public float num1;
    public float maxTime = 0;  //다이얼 넘어가기 위한 슬라이더 유지 시간
    public int min = 0;
    public Slider sc0_slider;
    public bool timer;
    public float middleAngle;
    public Text sc0_Text;
    public GameObject mainSlider;
    public Text middle_Text;
    public GameObject maxAngle_obj;
    private int circleNum;
    private float rotateRangeR = 0;
    private float rotateRangeL = 0;
    private int selectRange;
    private float rotateValue = 0;
    private int rand;
    public int UPcircle = 0;
    public int TotalRotate = 0;
    public bool training = false;
    private int doNum = 2;
    private float checkPoint;
    private int trainingAngle=10;
    public Text a;
    private bool auto_;
    public float check_now;
    bool isCali;
    #endregion

    public static EscapeGame instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        Slider_minmax();
        SliderMinValue = 0;
        SliderMaxValue = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Data.Instance.GameID.Substring(0, 2) == "32") // 수평일 때
            training = true;
        else
            training = false;

        // Serial 운동 설정
        if (Data.Instance.GameID.Substring(0, 1) == "1" || Data.Instance.GameID.Substring(0, 1) == "3") // 능동운동
            Serial.instance.Active();
        else if (Data.Instance.GameID.Substring(0, 1) == "4") // 저항운동
            Serial.instance.Resistance();

        Time.timeScale = 0f;
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

        GameSuccess();
        Slider_minmax();
        Timer_();

        if (training == true)
        {
            print("훈련");
            slider.minValue = 0;
            slider.maxValue = 359;
            sc0_slider.minValue = slider.minValue;
            sc0_slider.maxValue = slider.maxValue;
            middleAngle = (sc0_slider.minValue + sc0_slider.maxValue) / 2;
            middle_Text.text = "목표 회전수 : " + doNum + "  각도 : " + trainingAngle;
        }

        else
        {
            print("게임");
            sc0_slider.minValue = slider.minValue;
            sc0_slider.maxValue = slider.maxValue;
            sc0_slider.value = sc0_slider.minValue;
            middleAngle = (sc0_slider.minValue + sc0_slider.maxValue) / 2;
            middle_Text.text = "중앙값 : " + (int)middleAngle;
        }
        if (training==true)
            a.text = "현재 회전수 : " + UPcircle + "  각도 : " + (int)sc0_slider.value;
    }
    public void GamePlay()
    {
        // Serial 운동 설정
        if (Data.Instance.GameID.Substring(0, 1) == "1") // 능동운동
            Serial.instance.Active();
        else if (Data.Instance.GameID.Substring(0, 1) == "4") // 저항운동
            Serial.instance.Resistance();

        EscapeUI.instance.setZero();
        Key.SetActive(true);
        auto_ = true;
        Time.timeScale = 1f;
        slider.value = sc0_slider.value;
        for (int i = 0; i < 5; i++) //암호 정함
        {
            randNum = Random.Range(0, 99);
            codeNum.Add(randNum);
            ShowCode.Add(randNum);
        }
        if (level == 1)
            ShowCode.RemoveRange(0, 2);
        if (level == 2)
            ShowCode.RemoveRange(0, 1);
        line = string.Join(" ", ShowCode.ToArray());
        CodeText.text = "암호 : " + line;

        if (level == 1)
        {
            arrow[2].SetActive(true);
            firstCheck = true;
            secondCheck = true;
            if((0<=codeNum[2])&&(codeNum[2]<=9))
                thirdCode.text = "0" + (int)codeNum[2];
            else thirdCode.text = "" + (int)codeNum[2];
            thirdWrongCode();
            LastText.text = "" + (int)codeNum[4];

            circleRotate(); //난이도 따라 암호나타나는 범위 결정
            circle[2].transform.Rotate(0, 0, rotateValue);
        }
        else if (level == 2)
        {
            firstCheck = true;
            circle[1].SetActive(true);
            arrow[1].SetActive(true);
            if ((0 <= codeNum[1]) && (codeNum[1] <= 9))
                secondCode.text = "0" + (int)codeNum[1];
            else secondCode.text = "" + (int)codeNum[1];
            ScdWrongCode();
            LastText.text = "" + (int)codeNum[4];

            circleRotate();
            circle[1].transform.Rotate(0, 0, rotateValue);
        }
        else if (level == 3)
        {
            circle[0].SetActive(true);
            arrow[0].SetActive(true);
            if ((0 <= codeNum[0]) && (codeNum[0] <= 9))
                firstCode.text = "0" + (int)codeNum[0];
            else firstCode.text = "" + (int)codeNum[0];
            firstWrongCode();
            LastText.text = "" + (int)codeNum[4];

            circleRotate();
            circle[0].transform.Rotate(0, 0, rotateValue);
        }
        else if (level == 4)
        {
            arrow[0].SetActive(true);
            if ((0 <= codeNum[0]) && (codeNum[0] <= 9))
                firstCode.text = "0" + (int)codeNum[0];
            else firstCode.text = "" + (int)codeNum[0];
            firstWrongCode();
            LastText.text = "" + (int)codeNum[4];

            circleRotate();
            circle[0].transform.Rotate(0, 0, rotateValue);
        }
    }
    private void circleRotate() //난이도 따라 암호나타나는 범위 결정
    {
        if (startLv == 1) //쉬움
        {
            rotateRangeR = Random.Range(-45, -20);
            rotateRangeL = Random.Range(20, 45);
        }
        else if (startLv == 2) //보통
        {
            rotateRangeR = Random.Range(-90, -70);
            rotateRangeL = Random.Range(70, 90);
        }
        else if (startLv == 3) //어려움
        {
            rotateRangeR = Random.Range(-135, -115);
            rotateRangeL = Random.Range(115, 135);
        }
        else if (startLv == 4) //매우 어려움
        {
            rotateRangeR = Random.Range(-180, -160);
            rotateRangeL = Random.Range(160, 180);
        }
        selectRange = Random.Range(0, 2);
        if (selectRange == 0)
            rotateValue = rotateRangeR;
        else
            rotateValue = rotateRangeL;
    }
    public int checkCircle() //암호 원판 선택
    {
        if (fourthCheck == false) //네번째 정답 못맞춤
        {
            if (thirdCheck == false) //네번째, 세번째 정답 못 맞춤
            {
                if (secondCheck == false) //넷,셋,두번째 정답 못 맞춤
                {
                    if (firstCheck == false) //아무 비밀번호도 못 맞춤
                        circleNum = 0;
                    else circleNum = 1; //첫번째 비밀번호만 맞춤
                }
                else circleNum = 2; //첫,두번째 비밀번호 맞춤
            }
            else circleNum = 3; //세번째 까지 맞춤
        }
        return circleNum;
    }

    public void MoveSlider()
    {
        float x;
        dial_text.text = ""+(int)slider.value;
        preValue = nowValue;
        nowValue = (float)slider.value;
        sliderValue.text = "" + (float)slider.value;

        if (training == true)
        {
            trainingCheck(slider);
            x = Mathf.Abs(110 / (middleAngle - slider.minValue));
        }
        else  
            x = Mathf.Abs(210 / (middleAngle - slider.minValue));

        checkCircle();
        if (nowValue > preValue) //오른쪽
            circle[circleNum].transform.Rotate(0, 0, -(float)x);
        if (nowValue < preValue) //왼쪽
            circle[circleNum].transform.Rotate(0, 0, (float)x);
        timer = true;
    }

    public void trainingCheck(Slider wSilder) //훈련 상태일 때 회전 수 체크
    {
        if (wSilder.value < 50 && wSilder.value >= wSilder.minValue)
        {
            if (checkPoint == wSilder.maxValue)
            {
                UPcircle++;
                TotalRotate++;
                //Debug.Log("회전수:" + UPcircle);
            }
            checkPoint = 0;
        }
        else if (wSilder.value < 300 && wSilder.value > 100)
            checkPoint = 180;
        else if (wSilder.value > 330 && wSilder.value <= wSilder.maxValue)
        {
            if (checkPoint == wSilder.minValue)
            {
                UPcircle--;
                TotalRotate++;
                //Debug.Log("회전수:" + UPcircle);
            }
            checkPoint = 359;
        }
    }
public void CheckButton()
    {
        checkCircle();
        float zAxis = 360f - circle[circleNum].transform.eulerAngles.z;
        if ((((350 <= zAxis) && ((zAxis <= 0) || (zAxis <= 360))) || (0 <= zAxis) && (zAxis <= 10))) //성공 범위
        {
            if (circleNum == 0) //첫번째 원
            {
                EscapeSound.instance.Success();
                firstCheck = true;
                arrow[0].SetActive(false);
                arrow[1].SetActive(true);
                if ((0 <= codeNum[1]) && (codeNum[1] <= 9))
                    secondCode.text = "0" + (int)codeNum[1];
                else secondCode.text = "" + (int)codeNum[1];
                circleRotate();
                ScdWrongCode();
                circle[1].transform.Rotate(0, 0, rotateValue); //두번째 원에 암호
            }
            if (circleNum == 1) //두번째 원
            {
                EscapeSound.instance.Success();
                secondCheck = true;
                arrow[1].SetActive(false);
                arrow[2].SetActive(true);
                if ((0 <= codeNum[2]) && (codeNum[2] <= 9))
                    thirdCode.text = "0" + (int)codeNum[2];
                else thirdCode.text = "" + (int)codeNum[2];
                circleRotate();
                thirdWrongCode();
                circle[2].transform.Rotate(0, 0, rotateValue); //세번째 원에 암호
            }
            if (circleNum == 2) //세번째 원
            {
                EscapeSound.instance.Success();
                thirdCheck = true;
                arrow[2].SetActive(false);
                arrow[3].SetActive(true);
                if ((0 <= codeNum[3]) && (codeNum[3] <= 9))
                    fourthCode.text = "0" + (int)codeNum[3];
                else fourthCode.text = "" + (int)codeNum[3];
                circleRotate();
                fourthWrongCode();
                circle[3].transform.Rotate(0, 0, rotateValue); //네번째 원에 암호
            }
            if (circleNum == 3) //네번째 원
            {
                EscapeSound.instance.OpenDoor(); //탈출 소리
                fourthCheck = true;
            }
        }
        else //틀림
        {
            if(circleNum==0)
                firstCheck = false;
            if (circleNum == 1)
                secondCheck = false;
            if (circleNum == 2)
                thirdCheck = false;
            if (circleNum == 3)
                fourthCheck = false;
            redPanel.SetActive(true);
            EscapeSound.instance.Cave();
            Invoke("del_red", 1.5f);
        }
    }

    private void del_red() //붉은 패널 출력
    {
        redPanel.SetActive(false);
    }

    private void GameSuccess()
    {
        if ((firstCheck == true) && (secondCheck == true) && (thirdCheck == true) && (fourthCheck == true))
        {
            Time.timeScale = 0f;    //게임 시간 정지

            if ((1 <= level) && (level <= 3))
            {
                resultPanel.SetActive(true);    //결과 패널
                EscapeUI.instance.printTime(); //소요 시간 출력 reTime
                Time.timeScale = 1f;
                Invoke("nextScene", 1.5f);
            }
            if (level == 4)
            {
                EscapeUI.instance.printTime();
                scene[4].SetActive(true);
                Time.timeScale = 1f;
                Invoke("graphPrint", 1.5f);
            }
            Clear();
        }
    }
    private void Clear()
    {
        sc0_slider.value = slider.value; //대기화면 슬라이더에 게임 슬라이더 값 대입
        codeNum.Clear();
        firstCode.text = "";
        secondCode.text = "";
        thirdCode.text = "";
        fourthCode.text = "";
        for (int i = 0; i <= 17; i++)
            emptyText[i].text = "";
      
        codeNum.Clear();
        ShowCode.Clear();

        firstCheck = false;
        secondCheck = false;
        thirdCheck = false;
        fourthCheck = false; 
        auto_ = false;
        isCali = true;

        for (int i = 0; i < 4; i++)
            arrow[i].SetActive(false);

        level++;
        EscapeUI.instance.setZero();
    }

    public void graphPrint()
    {
        clearPanel.SetActive(true);
        EscapeUI.instance.endTime();
        EscapeUI.instance.EndScore();
        //Fill_();
        Device.instance.ResultCircle(circleGraph);
        Serial.instance.End();
    }

    public void Ending()
    {
        clearPanel.SetActive(true);
    }

    public void dialbuttonClick()
    {
        EscapeSound.instance.menu_Click();
        if (level == 1)
            scene[0].SetActive(false);
        if (level == 2)
            scene[1].SetActive(false);
        if (level == 3)
            scene[2].SetActive(false);
        if (level == 4)
            scene[3].SetActive(false);
        mainSlider.SetActive(false);
        GamePlay();
    }

    public void sliderDial()
    {
        trainingCheck(sc0_slider);
        sc0_Text.text = "" + (int)sc0_slider.value;
        if (training == true) //훈련일때
        {
            if ((doNum == UPcircle)&& (trainingAngle == (int)sc0_slider.value)) //회전수와 각도 동일하면
                isCali = false;
            else
                timer = false;
        }
        else
        {
            if (middleAngle == (int)sc0_slider.value)
            {
                isCali = false;
            }
            else
                timer = false;
        }
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
                        dialbuttonClick();
                        maxTime = 0;
                        min = 0;
                    }
                    timer = false;
                }
            }
        }
        else
        {
            maxTime = 0;
            min = 0;
        }
    }

    public void nextScene()
    {
        resultPanel.SetActive(false);
        mainSlider.SetActive(true);
        timer = false;
        if (level == 2)
        {
            Key.SetActive(false);
            scene[1].SetActive(true);
        }
        if (level == 3)
        {
            Key.SetActive(false);
            scene[2].SetActive(true);
        }
        if (level == 4)
        {
            Key.SetActive(false);
            scene[3].SetActive(true);
        }
        else
            Key.SetActive(false);
    }

    public void del_result()
    {
        resultPanel.SetActive(false);
    }
    public void startLv1()
    {
        EscapeSound.instance.menu_Click();
        startLv = 1;
    }
    public void startLv2()
    {
        EscapeSound.instance.menu_Click();
        startLv = 2;
    }
    public void startLv3()
    {
        EscapeSound.instance.menu_Click();
        startLv = 3;
    }
    public void startLv4()
    {
        EscapeSound.instance.menu_Click();
        startLv = 4;
    }
    public void del_choice() //난이도 선택창 없애기
    {
        chose_level.SetActive(false);
    }

    public void Slider_minmax() 
    {
        if (slider.value < SliderMinValue)
            SliderMinValue = slider.value;
        if (slider.value > SliderMaxValue)
            SliderMaxValue = slider.value;
    }
    public void Fill_() 
    {
        num1 = (float)((EscapeGame.instance.SliderMaxValue - EscapeGame.instance.SliderMinValue) * ((double)1 / (double)360));
        num1 = 1f-num1;
        circleGraph.transform.GetComponent<Image>().fillAmount = num1;

        float Rotate = (float)((EscapeGame.instance.SliderMinValue));
        circleGraph.transform.rotation = Quaternion.Euler(0, 0, Mathf.Abs(Rotate)); // Mathf.Abs : 절대값
    }

    private void firstWrongCode()
    {
        for(int i=0; i<=5; i++)
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
    private void ScdWrongCode()
    {
        for (int i = 6; i <= 10; i++)
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
    private void thirdWrongCode()
    {
        for (int i = 11; i <= 14; i++)
        {
            rand = Random.Range(0, 99);
            while (rand == int.Parse(thirdCode.text)) //정답이 두개 이상 있으면 안됨
            {
                rand = Random.Range(0, 99);
                if (rand != int.Parse(thirdCode.text))
                    break;
            }
            if ((0 <= rand) && (rand <= 9))
                emptyText[i].text = "0" + rand;
            else emptyText[i].text = "" + rand;
        }
    }
    private void fourthWrongCode()
    {
        for (int i = 15; i <= 17; i++)
        {
            rand = Random.Range(0, 99);
            while (rand == int.Parse(fourthCode.text)) //정답이 두개 이상 있으면 안됨
            {
                rand = Random.Range(0, 99);
                if (rand != int.Parse(fourthCode.text))
                    break;
            }
            if ((0 <= rand) && (rand <= 9))
                emptyText[i].text = "0" + rand;
            else emptyText[i].text = "" + rand;
        }
    }
    public void highPass() //게임 패스
    {
        firstCheck = true;
        secondCheck = true;
        thirdCheck = true;
        fourthCheck = true;

        Time.timeScale = 0f;    //게임 시간 정지

        if ((1 <= level) && (level <= 3))
        {
            resultPanel.SetActive(true);    //결과 패널
            EscapeUI.instance.printTime(); //소요 시간 출력 reTime
            Time.timeScale = 1f;
            Invoke("nextScene", 1.5f);
        }
        if (level == 4)
        {
            EscapeUI.instance.printTime();
            scene[4].SetActive(true);
            Time.timeScale = 1f;
            Invoke("graphPrint", 1.5f);
        }
        Clear();
    }
    public void warning()
    {
        Serial.instance.End();
        SceneManager.LoadScene("EscapeMain");
        Data.Instance.DeadTree++;
        print(Data.Instance.DeadTree);
    }

    public void endHome()
    {
        Serial.instance.End();
        SceneManager.LoadScene("EscapeMain");
        Data.Instance.FreshTree++;
        print(Data.Instance.FreshTree);
    }

    public void endReplay()
    {
        Serial.instance.End();
        SceneManager.LoadScene("EscapeDial");
        Data.Instance.FreshTree++;
        print(Data.Instance.FreshTree);
    }
}