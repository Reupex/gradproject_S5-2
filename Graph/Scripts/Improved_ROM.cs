using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Improved_ROM : MonoBehaviour
{
    // 내전/외전, 내회전/외회전, 굴곡/신전 - 개선된 관절가동범위
    public List<float> Ordinary_value; // 순서대로 일반인_Max, 일반인_Min, 환자_Max, 환자_Min
    public List<GameObject> obj = new List<GameObject>(2);
    public List<Text> obj_text = new List<Text>(2);
    public string userID, gameID;
    public int type; // 1 : 내전/외전, 2 : 내회전/외회전, 3 : 굴곡/신전, 4 : 원회전

    private void Awake()
    {
        switch(type)
        {
            case 1:
                abc(1);
                break;

            case 2:
                abc(2);
                break;

            case 3:
                abc(3);
                break;

            case 4:
                abc(4);
                break;
        }    
    }

    void abc(int type)
    {
        #region 개선된 관절 가동범위
        int x = 0, y = 0;
        if (type == 1)
        {
            x = 2; y = 3;
            Ordinary_value = new List<float>() { 14f, 178f };
        }
        else if (type == 2)
        {
            x = 4; y = 5;
            Ordinary_value = new List<float>() { 24f, 104f };
        }
        else if (type == 3)
        {
            x = 6; y = 7;
            Ordinary_value = new List<float>() { 180f, 60f };
        } 

        userID = SqlFormat(Data.Instance.UserID); // 나중에 로그인 구현시 Column 클래스로 만들어서 그때 객체화하기
        gameID = SqlFormat(Data.Instance.GameID);     // 각자 게임 이름에 맞게 변경

        if (type < 4) // type -> 1 2 3일때 
        {
            float a = 0;
            DBManager.DbConnectionChecek();
            DBManager.DataBaseRead(string.Format("SELECT * FROM Measurement WHERE userID = {0}", userID));
            while (DBManager.dataReader.Read())
            {
                a = DBManager.dataReader.GetInt32(x) + DBManager.dataReader.GetInt32(y);

                if (a != 0)
                    break;
            }
            float b = a / (Ordinary_value[0] + Ordinary_value[1]);
            obj[0].GetComponent<Image>().fillAmount = b;
            obj_text[0].text = ((int)(b * 100f)).ToString() + "%";


            DBManager.DataBaseRead(string.Format("SELECT * FROM Measurement WHERE userID = {0} ORDER BY date DESC", userID));
            while (DBManager.dataReader.Read())
            {
                a = DBManager.dataReader.GetInt32(x) + DBManager.dataReader.GetInt32(y);

                if (a != 0)
                    break;
            }
            b = a / (Ordinary_value[0] + Ordinary_value[1]);
            obj[1].GetComponent<Image>().fillAmount = b;
            obj_text[1].text = ((int)(b * 100f)).ToString() + "%";
            DBManager.DBClose();
        }
        #endregion

        #region 원회전 - 개선된 관절가동범위
        else {
            float a = 0;
            DBManager.DbConnectionChecek();
            DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0} ORDER BY date", userID));
            while (DBManager.dataReader.Read())
            {
                gameID = DBManager.dataReader.GetString(2);
                Debug.Log("123123 : " + gameID.Substring(0, 2));
                if (gameID.Substring(0, 1) == "3") // 제일 앞자리가 3인 건 원회전으로 따로 분류함
                    if (gameID.Substring(1, 1) == "1")
                        a = DBManager.dataReader.GetInt32(4);

                if (a != 0)
                    break;
            }
            float c = a / 100f;
            obj[0].GetComponent<Image>().fillAmount = c;
            obj_text[0].text = ((int)(c * 100f)).ToString() + "%";

            a = 0;
            DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0} ORDER BY date DESC", userID));
            while (DBManager.dataReader.Read())
            {
                gameID = DBManager.dataReader.GetString(2);
                Debug.Log("123123 : " + gameID.Substring(0, 2));
                if (gameID.Substring(0, 1) == "3") // 제일 앞자리가 3인 건 원회전으로 따로 분류함                        
                    a = DBManager.dataReader.GetInt32(4);

                if (a != 0)
                    break;
            }
            c = a / 100f;
            obj[1].GetComponent<Image>().fillAmount = c;
            obj_text[1].text = ((int)(c * 100f)).ToString() + "%";
            DBManager.DBClose();
        }
        #endregion
    }

    public string SqlFormat(string sql)
    {
        return string.Format("\"{0}\"", sql);
    }
}
