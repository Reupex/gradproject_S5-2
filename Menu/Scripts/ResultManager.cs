using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public GameObject ResultPanel;
    public GameObject List;
    GameObject Pose1;
    GameObject Pose2;

    GameObject WheelL1;
    GameObject WheelR1;
    GameObject WheelL2;
    GameObject WheelR2;

    float max1 = 0f;
    float max2 = 0f;
    int startIndex = 0;
    
    string Name = null;
    string Birth = null;
    string Gender = null;

    float[] ResultData = new float[4];

    // Start is called before the first frame update
    void Start()
    {
        Pose1 = ResultPanel.transform.Find("Pose1").gameObject;
        Pose2 = ResultPanel.transform.Find("Pose2").gameObject;

        WheelL1 = Pose1.transform.Find("Wheel_Left").gameObject.transform.Find("L1").gameObject;
        WheelR1 = Pose1.transform.Find("Wheel_Right").gameObject.transform.Find("R1").gameObject;
        WheelL2 = Pose2.transform.Find("Wheel_Left").gameObject.transform.Find("L2").gameObject;
        WheelR2 = Pose2.transform.Find("Wheel_Right").gameObject.transform.Find("R2").gameObject;
        ChangeColor(List.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Button>());
        SettingInfo(); // 사용자 데이터 세팅
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SettingInfo()
    {
        string order = "SELECT * FROM UserInfo WHERE userID = '" + /*Data.Instance.UserID*/"test" + "'";
        DBManager.DataBaseRead(order);
        while (DBManager.dataReader.Read())
        {
            Name = DBManager.dataReader.GetString(1);
            Birth = DBManager.dataReader.GetInt32(3).ToString().Substring(2); // 8자 -> 6자
            Gender = DBManager.dataReader.GetString(2).Substring(0, 1);
        }
        DBManager.DBClose();
        
        ResultPanel.transform.Find("User").gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = "이 름 : " + Name;
        ResultPanel.transform.Find("User").gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = "성 별 : " + Gender;
        ResultPanel.transform.Find("User").gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = "성 별 : " + Gender;
        ResultPanel.transform.Find("User").gameObject.transform.GetChild(2).gameObject.GetComponent<Text>().text = "생 년 월 일 : " + Birth;
    }

    void ShowResultGraph() // 검사 결과 그래프 가져오는 함수 
    {
        string order = string.Format("SELECT * FROM Measurement WHERE userID = {0}", /*Data.Instance.UserID*/ "'test'");
        DBManager.DataBaseRead(order);
        while (DBManager.dataReader.Read())
        {
            // 검사 결과 목록 불러오기
        }
        DBManager.DBClose();
    }

    void Wheel(GameObject image, float data, float max)
    {
        image.GetComponent<Image>().fillAmount = data * (0.75f / max);

        Text angle = image.gameObject.transform.parent.gameObject.transform.Find("Angle").gameObject.GetComponent<Text>();
        angle.text = data.ToString();
    }

    public void MeasureData() // 선택한 검사 결과 데이터 세팅 함수
    {
        string order = string.Format("SELECT * FROM Measurement WHERE userID = {0} AND date = {1}", "'test'", "'210304'");
        DBManager.DataBaseRead(order);

        while (DBManager.dataReader.Read())
        {
            ResultPanel.transform.Find("User").gameObject.transform.GetChild(3).gameObject.GetComponent<Text>().text 
                = "검 사 일 : " + DBManager.dataReader.GetString(1);
            for (int i = startIndex; i < startIndex+4; i++)
            {
                ResultData[i - startIndex] = float.Parse(DBManager.dataReader.GetValue(i).ToString());
            }
        }
        DBManager.DBClose();

        Wheel(WheelL1, ResultData[0], max1);
        Wheel(WheelR1, ResultData[1], max1);
        Wheel(WheelL2, ResultData[2], max2);
        Wheel(WheelR2, ResultData[3], max2);
    }

    public void ChangeColor(Button button) // [Measuer의 Main 상단바 자세] / [Result의 자세] onclick()으로 버튼+텍스트 색상 변경
    {
        string pose1 = null; // 자세1 이름
        string pose2 = null; // 자세2 이름
        string info1 = null; // 자세1 설명
        string info2 = null; // 자세2 설명

        Transform Parent = button.gameObject.transform.parent;
        List<Button> pose = new List<Button>();
        for (int i = 0; i < 3; i++)
        {
            pose.Add(Parent.GetChild(i).GetComponent<Button>()); // pose 버튼 3개 리스트에 저장
        }

        switch (button.name)
        {
            case "1":
                pose1 = "내 전 (Adduction)";
                pose2 = "외 전 (Abduction)";
                info1 = "팔을 몸 옆으로 내리면서\n최대한 멀게 몸의 중앙을 가로지르게 함";
                info2 = "팔을 몸 옆쪽으로 머리 위를\n향해 멀리 그리고 크게 벌림";
                startIndex = 2;
                max1 = 60f;
                max2 = 180f;
                break;
            case "2":
                pose1 = "내 회 전 (Internal Rotation)";
                pose2 = "외 회 전 (External Rotation)";
                info1 = "엄지가 안쪽으로 향하도록 하여 손등이 나올 때까지\n팔꿈치를 구부린 채 팔을 움직여서 어깨를 회전시킴";
                info2 = "엄지가 위쪽을 향하고 손의 윗부분이 바깥쪽을 향하게 될 때까지 팔꿈치를 구부린\n채 팔을 움직이는 것";
                startIndex = 6;
                max1 = 90f;
                max2 = 90f;
                break;
            case "3":
                pose1 = "굴 곡 (Flextion)";
                pose2 = "신 전 (Extension)";
                info1 = "팔을 몸 앞쪽으로 머리 위를\n향해 올리는 것";
                info2 = "팔꿈치를 바르게 유지하고\n몸 뒤쪽으로 팔을 젖히는 것";
                startIndex = 10;
                max1 = 180f;
                max2 = 60f;
                break;
        }

        for (int i = 0; i < pose.Count; i++) // 버튼 색상 변경
        {
            if (pose[i] == button) // 선택한 버튼이라면
            {
                pose[i].gameObject.GetComponent<Image>().color = new Color(0.3019608f, 0.3137255f, 0.3803922f);
                button.gameObject.transform.GetChild(0).GetComponent<Text>().color = Color.white;
                Pose1.transform.Find("PoseName").GetComponent<Text>().text = pose1;
                Pose2.transform.Find("PoseName").GetComponent<Text>().text = pose2;
                Pose1.transform.Find("PoseName").transform.Find("Info").GetComponent<Text>().text = info1;
                Pose2.transform.Find("PoseName").transform.Find("Info").GetComponent<Text>().text = info2;
                pose[i].enabled = false;
            }
            else // 선택한 버튼 외 버튼
            {
                pose[i].gameObject.GetComponent<Image>().color = Color.white;
                pose[i].gameObject.transform.GetChild(0).GetComponent<Text>().color = Color.black;
                pose[i].enabled = true;
            }
        }
    }
}
