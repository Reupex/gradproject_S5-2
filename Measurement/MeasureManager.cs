using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MeasureManager : MonoBehaviour
{
    public GameObject MeasurePanel; 
    public GameObject PausePanel;
    public GameObject EndPanel;
    public GameObject Main;
    GameObject start;

    bool isMeasure = false; // 측정 시작여부
    public float stayTime = 0; // 최대 가동 범위에서 유지한 시간
    bool timer;
    public float Angle = 0;
    float pastAngle = 0;
    public string poseindex = "";
    public int index = 0;
    public int SIndex = 0;

    Text angle;
    Text Pose;
    Image color;
    Slider nowSilder;
    Slider lastSilder;
    GameObject show;
    String Date = DateTime.Now.ToString("yyyy년 MM월 dd일 HH:mm");

    string[] poseName = { "내 전", "외 전", "굴 곡", "신 전" , "내 회 전", "외 회 전" };
    int[] poseAngle = { 60, 180, 90, 60 , 180, 60 };
    public List<Sprite> photo = new List<Sprite>();
    public float[] Mangle = {0f, 0f, 0f, 0f, 0f, 0f};

    // Start is called before the first frame update
    void Start()
    {
        string order = "SELECT * FROM Measurement WHERE userID = '" + Data.Instance.UserID + "'";
        DBManager.DataBaseRead(order);
        bool isbeing = false;
        while (DBManager.dataReader.Read())
        {
            isbeing = true;
            MeasurePanel.transform.Find("Select").gameObject.SetActive(true);
            this.tag = "NotFirst";
        }
        DBManager.DBClose();
        if (!isbeing) // 처음이면 자세 선택 없이 모든 자세 측정
        {
            MeasurePanel.transform.Find("First").gameObject.SetActive(true);
            this.tag = "First";
            //Serial.instance.SettingM("1");
        }

        timer = false;
        isMeasure = false;
        Main.transform.Find("Panel").gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text 
            = Data.Instance.UserName + " 님 환영합니다!";
        // GameObject 세팅
        color = Main.transform.Find("measureWheel").transform.Find("color").gameObject.GetComponent<Image>();
        start = Main.transform.Find("measureWheel").transform.Find("start").gameObject;
        angle = Main.transform.Find("measureWheel").transform.Find("AngleText").gameObject.GetComponent<Text>();
        Pose = Main.transform.Find("measureWheel").transform.Find("pose").gameObject.GetComponent<Text>();
        nowSilder = Main.transform.Find("Panel").gameObject.transform.GetChild(2).gameObject.GetComponent<Slider>();
        lastSilder = Main.transform.Find("Panel").gameObject.transform.GetChild(4).gameObject.GetComponent<Slider>();
        show = Main.transform.Find("Panel").gameObject.transform.GetChild(5).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer)
        {
            stayTime += Time.deltaTime;
            if(stayTime >= 3.0f) // 3초 유지
            {
                isMeasure = false;
                Serial.instance.End();
                SaveAngle();
            }
        }
        else
        {
            stayTime = 0;
        }
        WheelFilled();
    }

    void WheelFilled() // 중앙에 배치된 무지개 wheel
    {
        if(isMeasure)
        {
            pastAngle = Angle;
            if (index == 0 || index == 2)
            {
                color.gameObject.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 0);
                Angle = Serial.instance.angle - Serial.instance.start;
                print(Serial.instance.angle + "     " + Serial.instance.start);
            }
            else if (index == 4) // 내회전
            {
                color.gameObject.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 180, 180);
                Angle = 360 - Serial.instance.angle;
                if (Angle == 360)
                    Angle = 0;
            }
            else if(index == 5) // 외회전
            {
                color.gameObject.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 180);
                Angle = Serial.instance.angle;
            }
            else
            {
                color.gameObject.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 180, 0);
                Angle = Serial.instance.start - Serial.instance.angle;
            }
            if (pastAngle == Angle )
                timer = true;
            else
                timer = false;
            angle.text = ((int)Angle).ToString();            
        }
        color.fillAmount = Angle / 360; // Wheel 색 있는 부분
        nowSilder.value = Angle;
        nowSilder.transform.GetChild(5).transform.GetChild(0).GetComponent<Text>().text = angle.text;
    }

    public void ChangeColor(GameObject text) // [Measuer의 Main 상단바 자세] / [Result의 자세] onclick()으로 버튼+텍스트 색상 변경
    {
        Transform Parent = text.gameObject.transform.parent;

        List<GameObject> pose = new List<GameObject>();
        for (int i = 0; i < 3; i++)
        {
            pose.Add(Parent.GetChild(i).gameObject); // pose 버튼 3개 리스트에 저장
        }

        switch (text.name)
        {
            case "1":
                index = 0;
                break;
            case "2":
                index = 4;
                break;
            case "3":
                index = 2;
                break;
        }
        SIndex = index;

        for (int i = 0; i < pose.Count; i++) // 자세 색상 변경
        {
            if (pose[i] == text) // 선택한 버튼이라면 특정 색상으로 변경하도록(텍스트도 변경)
            {
                pose[i].gameObject.GetComponent<Image>().color = Color.white;
                text.gameObject.transform.GetChild(0).GetComponent<Text>().color = Color.white;
                Pose.text = poseName[index];
            }
            else // 선택한 버튼 외 버튼
            {
                pose[i].gameObject.GetComponent<Image>().color = Color.black;
                pose[i].gameObject.transform.GetChild(0).GetComponent<Text>().color = Color.black;
            }
        }
    }

    public void StartPose()
    {
        Serial.instance.Active();
        GameObject start = EventSystem.current.currentSelectedGameObject;
        Serial.instance.isEnd = false;
        start.SetActive(false);
        isMeasure = true;
        Pose.text = poseName[index];
        if(index >= 4)
            FillData(index-2);
        else if(index >= 2)
            FillData(index+2);
        else
            FillData(index);
    }

    public void SaveAngle()
    {
        timer = false;
        Mangle[index] = int.Parse(Angle.ToString());
        if ((this.CompareTag("First") && index == 5) || (this.CompareTag("NotFirst") && index == SIndex + 1))
        {
            print("끝");
            Serial.instance.End();
            Serial.instance.SettingM(poseindex);
            EndPanel.SetActive(true);
            string order = string.Format("INSERT INTO Measurement VALUES ({0},{1},{2},{3},{4},{5},{6},{7})"
                , DBManager.SqlFormat(Data.Instance.UserID), DBManager.SqlFormat(Date), Mangle[0], Mangle[1], Mangle[4], Mangle[5], Mangle[2], Mangle[3]);
            DBManager.DatabaseSQLAdd(order);
        }
        else if (this.CompareTag("First") && index == 3 || index == 4)
        {
            poseindex = "2";
            Serial.instance.SettingM(poseindex);
            start.SetActive(true);
            Pose.text = poseName[++index];
        }
        else
        {
            Serial.instance.SettingM(poseindex);
            start.SetActive(true);
            Pose.text = poseName[++index];
        }
        if (index == 2)
            ChangeColor(Main.transform.GetChild(2).gameObject);
        else if (index == 4)
            ChangeColor(Main.transform.GetChild(1).gameObject);
    }

    public void FillData(int index)
    {
        string lastdate = "";
        int lastAngle = 0;

        if (this.CompareTag("First"))
        {
            nowSilder.gameObject.SetActive(false);
            lastSilder.gameObject.SetActive(false);
            Main.transform.Find("Panel").gameObject.transform.GetChild(3).gameObject.SetActive(false); // - 이미지
            show.SetActive(true);
            show.transform.GetChild(0).GetComponent<Image>().sprite = photo[index];
            show.transform.GetChild(1).GetComponent<Text>().text = "현재 측정 기록 : " + Date.Substring(6, 13);
        }
        else
        {
            DBManager.DataBaseRead(string.Format("SELECT * FROM Measurement WHERE userID = {0} ORDER BY date DESC ", DBManager.SqlFormat(Data.Instance.UserID)));// 다시 생각해보기
            while (DBManager.dataReader.Read())
            {
                lastdate = DBManager.dataReader.GetString(1);
                lastAngle = DBManager.dataReader.GetInt32(index + 2);
                if(lastAngle != 0)
                    break;
            }
            DBManager.DBClose();

            nowSilder.transform.GetChild(0).transform.GetComponent<Text>().text = "현재 측정 기록 : " + Date.Substring(6, 13);
            nowSilder.maxValue = poseAngle[index];
            nowSilder.transform.GetChild(2).GetComponent<Text>().text = poseAngle[index].ToString(); // max값 text
            lastSilder.transform.GetChild(0).transform.GetComponent<Text>().text = "마지막 측정 기록 : " + lastdate.Substring(6, 13);
            lastSilder.maxValue = poseAngle[index];
            lastSilder.value = lastAngle;
            lastSilder.transform.GetChild(5).transform.GetChild(0).GetComponent<Text>().text = lastAngle.ToString();
            lastSilder.transform.GetChild(2).GetComponent<Text>().text = poseAngle[index].ToString(); // max값 text
        }
    }

    #region Button Onclick 함수들  
    public void Setting()
    {
        GameObject Button = EventSystem.current.currentSelectedGameObject;
        poseindex = Button.name;

        MeasurePanel.transform.Find("Select").gameObject.SetActive(false);
        MeasurePanel.transform.Find("Setting").gameObject.SetActive(true);
        MeasurePanel.transform.Find("Setting").transform.Find(poseindex).gameObject.SetActive(true);

        ChangeColor(Main.transform.Find(poseindex).gameObject);
        switch(poseindex)
        {
            case "1": // 내/외전
                Serial.instance.dir = "0";
                FillData(0);
                break;
            case "3": // 굴곡/신전
                Serial.instance.dir = "0";
                FillData(4);
                break;
            case "2": // 내/외회전
                Serial.instance.dir = "1";
                FillData(2);
                break;
        }
        Serial.instance.Active();
    }
    public void Setting0()
    {
        Serial.instance.Setting0(poseindex);
    }
    public void SettingDevice()
    {
        Serial.instance.SettingM(poseindex);
    }
    public void OFf()
    {
        Main.SetActive(true);
    }

    public void Done()
    {
        Serial.instance.Active();
    }

    public void Pause()
    {
        Time.timeScale = 0;
        PausePanel.SetActive(true);
        MeasurePanel.SetActive(false);
    }

    public void Restart()
    {
        //SceneManager.LoadScene("Measurement");
        ChangeColor(Main.transform.Find(poseindex).gameObject);
        start.SetActive(true);
    }

    public void Result()
    {
        Serial.instance.SettingM(poseindex);
        Serial.instance.End();
        SceneManager.LoadScene("Graph");
    }

    public void XButton()
    {
        Time.timeScale = 1;
        PausePanel.SetActive(false);
        MeasurePanel.SetActive(true);
    }

    public void Forest()
    {
        SceneManager.LoadScene("Forest");
    }

    public void Exit() //OnApplicationQuit()
    {
        Serial.instance.SerialClose();
        // 종료하기 버튼 클릭 -> 확인 메시지 팝업창에서 예 눌렀을 경우 실행
        Application.Quit();
        print("종료");
    }
}
#endregion
