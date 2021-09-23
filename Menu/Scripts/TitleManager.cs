using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data.SqlClient;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;

public class TitleManager : MonoBehaviour
{
    public GameObject Title_Panel;
    public GameObject Login_Panel;
    public GameObject Login;
    public GameObject SingUp;
    public Text LoginMessage;

    /* IF == InputField */
    InputField LoginID_IF;
    InputField LoginPW_IF;
    InputField SignUpID_IF;
    InputField SignUpPW_IF;
    InputField Name_IF;
    InputField Birth_IF;
    InputField LongHandle_IF;
    InputField ShortHandle_IF;
    Dropdown Gender_Dropdown;
    
    string Gender; // == Gender_Dropdown.captionText.text;
    int Birth; // == int.Parse(Birth_IF.text);

    bool IDCheck = false;
    bool SignUpCheck = false;

    // Start is called before the first frame update
    void Start()
    {
        DBManager.DbConnectionChecek();
        Setting_Object();
        
        IDCheck = false;
        SignUpCheck = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void Setting_Object() 
    {
        LoginID_IF = Login.transform.Find("ID").Find("InputField").GetComponent<InputField>();
        LoginPW_IF = Login.transform.Find("PW").Find("InputField").GetComponent<InputField>();

        SignUpID_IF = SingUp.transform.Find("ID").Find("InputField").GetComponent<InputField>();
        SignUpPW_IF = SingUp.transform.Find("PW").Find("InputField").GetComponent<InputField>();
        Name_IF = SingUp.transform.Find("Name").Find("InputField").GetComponent<InputField>();
        Birth_IF = SingUp.transform.Find("Birth").Find("InputField").GetComponent<InputField>();
        LongHandle_IF = SingUp.transform.Find("Handle").Find("Long").GetComponent<InputField>();
        ShortHandle_IF = SingUp.transform.Find("Handle").Find("Short").GetComponent<InputField>();
        Gender_Dropdown = SingUp.transform.Find("Gender").Find("Dropdown").GetComponent<Dropdown>();
    }

    public void LoginCheck() // 로그인 가능 유무 확인
    {
        Debug.Log(LoginID_IF.text + " / " + LoginPW_IF.text);
        
        //데이터베이스 데이터와 비교하는 코드
        string order = "SELECT * FROM UserInfo WHERE userID = '" + LoginID_IF.text + "'";
        DBManager.DataBaseRead(order);

        bool OverlapCheck = false; // LoginID_IF.text가 DB에 있는 아이디인지 확인하기 위한 bool 변수
        while (DBManager.dataReader.Read())
        {
            OverlapCheck = true; 
            if (LoginPW_IF.text == DBManager.dataReader.GetString(4)) // reader[4] == UserPW
            {
                Data.Instance.UserID = LoginID_IF.text;
                Data.Instance.UserName = DBManager.dataReader.GetString(1);
                //Data.Instance.LongHandle = DBManager.dataReader.GetString(5);
                //Data.Instance.ShortHadle = DBManager.dataReader.GetString(6);
                SceneManager.LoadScene("Forest"); // 로그인 성공 시 숲 콘텐츠로 씬 이동
            }
            else if (LoginPW_IF.text != DBManager.dataReader.GetString(4)) // 입력한 비밀번호 != DB에 저장된 비번
            {
                LoginMessage.text = "잘못된 비밀번호입니다.";
            }
        }
        if (!OverlapCheck) {
            LoginMessage.text = "가입하지 않은 아이디입니다.";
        }
        DBManager.DBClose();
    }

    void IDOverlapCheck()
    {
        string order = "SELECT * FROM UserInfo WHERE userID = '" + SignUpID_IF.text + "'";
        DBManager.DataBaseRead(order);
        print(order);

        bool OverlapCheck = false; // SignUpID_IF.text가 DB에 있는 아이디인지 확인하기 위한 bool 변수
        if(SignUpID_IF.text.Length > 0)
        {
            while (DBManager.dataReader.Read())
            {
                OverlapCheck = true;
                if (SignUpID_IF.text == DBManager.dataReader.GetString(0)) // reader[0] == UserID
                {
                    IDCheck = false;
                    SingUp.transform.Find("Popup").gameObject.SetActive(true);
                    SingUp.transform.Find("Popup").transform.Find("Message").gameObject.GetComponent<Text>().text = "이미 사용중인 아이디입니다.";
                }
            }
            if (!OverlapCheck)
            {
                IDCheck = true;
                SingUp.transform.Find("Popup").gameObject.SetActive(true);
                SingUp.transform.Find("Popup").transform.Find("Message").gameObject.GetComponent<Text>().text = "사용 가능한 아이디입니다.";
            }
        }
        else
        {
            SingUp.transform.Find("Popup").gameObject.SetActive(true);
            SingUp.transform.Find("Popup").transform.Find("Message").gameObject.GetComponent<Text>().text = "아이디를 입력하세요.";
        }

        DBManager.DBClose();
    }

    void CheckData()
    {
        // 필수항목 입력 확인
        if (IDCheck
            && SignUpPW_IF.text.Length > 0
            && Name_IF.text.Length > 0
            && Gender_Dropdown.captionText.text != "성별"
            && Birth_IF.text.Length == 8
            && LongHandle_IF.text.Length > 0
            && ShortHandle_IF.text.Length > 0)
        {
            SignUpCheck = true;
        }
        else 
            SignUpCheck = false;
    }

    public void SignUpSave() // 회원가입 입력 데이터 체크 및 전송 함수
    {
        CheckData();

        if (!SignUpCheck)
        {
            SingUp.transform.Find("Popup").gameObject.SetActive(true);
            SingUp.transform.Find("Popup").transform.Find("Message").gameObject.GetComponent<Text>().text = "입력 내용을 확인해주세요.";
            Debug.Log("회원가입 실패");
        }
        else
        {
            Gender = Gender_Dropdown.captionText.text;
            Birth = int.Parse(Birth_IF.text);
            // 데이터베이스에 데이터 저장하는 코드!
            string UserInfo = "INSERT INTO UserInfo VALUES ('" + SignUpID_IF.text + "','" + Name_IF.text + "','" + Gender + "'," + Birth + ",'" + SignUpPW_IF.text + "','" + LongHandle_IF.text + "','" + ShortHandle_IF.text + "','" + DateTime.Now.ToString("yyyy년 MM월 dd일") + "')";
            string Forest = "INSERT INTO Forest VALUES ('" + SignUpID_IF.text + "',0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)";
            DBManager.DatabaseSQLAdd(UserInfo);
            DBManager.DatabaseSQLAdd(Forest);
            GameObject.Find("Login_Panel").transform.Find("Complete").gameObject.SetActive(true);
            SingUp.SetActive(false);
        }
    }

    public void ResetText() // 회원가입 입력창 초기화 함수 
    {
        SignUpCheck = false;

        SignUpID_IF.text = "";
        SignUpPW_IF.text = "";
        Name_IF.text = "";
        Birth_IF.text = "";
        LongHandle_IF.text = "";
        ShortHandle_IF.text = "";
        Gender = "";
        Birth = 0;
        Gender_Dropdown.captionText.text = "성별";
    }
}
