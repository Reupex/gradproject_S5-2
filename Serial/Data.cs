using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Data : MonoBehaviour
{
    public string UserName;
    public string Date;
    public string UserID;
    public string GameID;
    public string LongHandle; // 손잡이 태그
    public string ShortHadle; // 손잡이 태그

    public int adduction; // 내전
    public int abduction; // 외전
    public int internalR; // 내회전
    public int externalR ; // 외회전
    public int flextion ; // 굴곡
    public int extension; // 신전

    public int DeadTree = 0;
    public int FreshTree = 0;
    
    private static Data instance;
    public static Data Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<Data>();
            return instance;
        }
    }

    void Awake()
    {
        MaxData();
        DontDestroyOnLoad(this);    
    }

    public void MaxData()
    {
        Date = DateTime.Now.ToString("yyMMdd");
        string[] pose = { "adduction", "abduction", "internalR", "externalR", "flextion", "extension" };
        for(int i = 0; i < pose.Length; i++)
        {
            DBManager.DataBaseRead(string.Format("SELECT * FROM Measurement WHERE userID = {0} ORDER BY {1} DESC", DBManager.SqlFormat(UserID), pose[i]));
            while (DBManager.dataReader.Read())
            {
                switch (pose[i])
                {
                    case "adduction":
                        adduction = DBManager.dataReader.GetInt32(2);
                        break;
                    case "abduction":
                        abduction = DBManager.dataReader.GetInt32(3);
                        break;
                    case "internalR":
                        internalR = DBManager.dataReader.GetInt32(4);
                        break;
                    case "externalR":
                        externalR = DBManager.dataReader.GetInt32(5);
                        break;
                    case "flextion":
                        flextion = DBManager.dataReader.GetInt32(6);
                        break;
                    case "extension":
                        extension = DBManager.dataReader.GetInt32(7);
                        break;
                }
                break;
            }
            DBManager.DBClose();
        }
    }

    public void SettingCenter()
    {
        Serial.instance.SettingM(GameID.Substring(1,1));
    }
}
