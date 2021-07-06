using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Text;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class Serial : MonoBehaviour
{
    private SerialPort sp;
    byte[] by = new byte[1];

    public bool debug = false;
    bool startM = false;
    bool isInit = false;
    bool repeat = false;
    public bool isRevise = false;
    public string[] ecText;
    public List<int> values = new List<int>();
    string str;

    int a = 0;
    int current = 0;
    int temp = 0;
    public int offsetAngle = 0;
    public int angle = 0; // 현재 디바이스 각도
    public int start = 0; // 측정 시작 절대 각도
    public bool isEnd = false;

    public string dir = "";

    public static Serial instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
        by[0] = 0x02;
        string[] ports = SerialPort.GetPortNames();
        foreach (string p in ports)
        {
            sp = new SerialPort(p, 115200, Parity.None, 8, StopBits.One); // 초기화
            try
            {
                sp.Open(); // 프로그램 시작시 포트 열기

                sp.Write(by, 0, 1); // 0x02 값을 받으면 데이터 출력 시작 --> by 배열 이용
            }
            catch (TimeoutException e) //예외처리
            {
                Debug.Log("timeout");
                continue;
            }
            catch (IOException ex) //예외처리
            {
                Debug.Log("io exception");
                continue;
            }
            Debug.Log("send message");
            Debug.Log(p);
            if (sp.ReadLine().Equals("")) continue;
            else break;
        }
        ResetBuffer();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (debug)
        {
            try
            {
                sp.DiscardInBuffer();
                angle = int.Parse(sp.ReadLine());
                temp = angle;
            }
            catch (FormatException) //예외처리
            {
                angle = temp;
            }
            print(angle);
        }
        if (isInit)
        {
            str = sp.ReadLine();
            if (str == "MMF")
            {
                sp.Write("end");
                Debug.Log(sp.ReadLine());
                isEnd = true;
                isInit = false;
            }
        }
        if (isRevise)
        {
            try
            {
                sp.DiscardInBuffer();
                ecText = sp.ReadLine().Split(',');
                if (str == "end")
                {
                    sp.Write("end");
                    isRevise = false;
                }
            }
            catch (FormatException) //예외처리
            {
                
            }
            
        }
    }

    //public void RepeatCheck()
    //{
    //    values.Add(angle);
    //    if(values.Count == 3)
    //    {
    //        for(int i = 0; i < values.Count-2; i++)
    //        {
    //            if (values[i] + 1 == values[i + 1])
    //            {
    //                print("왔다갔다");
    //                angle = values[0];
    //            }
    //            else
    //            {
    //                values.Clear();
    //                angle = a;
    //            }
    //        }
    //    }
    //}
    
    public void ResetBuffer() // 버퍼 초기화 + 0점
    {
        //sp.Write("w");
        sp.Write("q");
        sp.Write("r");
        Debug.Log(sp.ReadLine());
    }

    public void Active() // 능동 모드 시작
    {
        sp.Write("ac");
        Debug.Log(sp.ReadLine());
        Debug.Log(sp.ReadLine());
        debug = true;
    }

    public void End() // 종료 
    {
        sp.Write("end");
        debug = false;
        sp.Write("r");
        Debug.Log(sp.ReadLine());
        print("end");
    }

    public void Passive() // 수동 모드 시작 
    {
        sp.Write("pa");
        Debug.Log(sp.ReadLine());
    }

    public void PaMotor(string move) // 수동 모드 - 모터 제어
    {
        // ex) move = "0180(각도)0100(속도)"
        sp.Write(move);
        isInit = true;
    }

    public int PauseDevice() // 긴급 정지 - 리턴값 : 멈출 때의 각도
    {
        sp.Write("o");
        return int.Parse(sp.ReadLine());
    }

    public void Resistance() // 저항 모드 시작
    {
        sp.Write("bc");
        debug = true;
    }

    public void Revise() // 각도 보정
    {
        sp.Write("ec");
        isRevise = true;
    }

    public void qw() // 근전도
    {
        sp.Write("q");
        sp.Write("w");
    }

    public string WriteAngle(int angle) // angle을 펌웨어 명령어에 맞게 변경해주는 함수
    {
        if (0 <= angle && angle < 10) // 0 ~ 9
            return "00" + angle.ToString();
        else if (9 < angle && angle < 100) // 10 ~ 99
            return "0" + angle.ToString();
        else if (100 <= angle) // 100 ~
            return angle.ToString();
        else
            return null;
    }

    public void Setting0(string poseindex)
    {
        sp.Write("ac");
        sp.Write("z");
        if(angle != 0)
        {
            sp.Write("z");
            angle = 0;
        }
        End();
        print(angle);
        SettingM(poseindex);
    }

    public void SettingM(string pose)
    {
        switch (pose)
        {
            case "1": // 내/외전
                start = 180;
                break;
            case "2": // 내/외회전
                start = 360;
                break;
            case "3":
                start = 180;
                break;
        }
        //밍
        sp.Write("pa");
        if (pose == "1"||pose == "3") //내/외전 , 굴/신
        {
            if (dir == "1")
            {
                PaMotor(dir + WriteAngle(angle - start) + "1000");
                print(dir + WriteAngle(angle - start) + "1000");
                dir = "0";
            }
            else
            {
                PaMotor(dir + WriteAngle(start - angle) + "1000");
                print(dir + WriteAngle(start - angle) + "1000");
                dir = "1";
            }
        }
        else if(pose =="2") //내/외회전
        {
            if (dir == "0")
            {
                PaMotor(dir + WriteAngle(start-angle) + "1000");
                print(dir + WriteAngle(start - angle) + "1000");
                dir = "1";
            }
            else
            {
                PaMotor(dir + WriteAngle(angle) + "1000");
                print(dir + WriteAngle(angle) + "1000");
                dir = "0";
            }
        }
    }
    
    public void Init(string pose, string dir, int ResetAngle) // 초기 각도 값 설정
    {
        string angle2 = "";
        if (0 <= ResetAngle && ResetAngle < 10)
            angle2 = "00" + ResetAngle.ToString();
        else if (9 < ResetAngle && ResetAngle < 100)
            angle2 = "0" + ResetAngle.ToString();
        else
            angle2 = ResetAngle.ToString();
        Passive();

        //밍
        if (dir == "1")
        {
            PaMotor(dir + angle2 + "1000");
            dir = "0";
        }
        else
        {
            PaMotor(dir + angle2 + "1000");
            dir = "1";
        }

        //PaMotor(dir + angle2 +"1000");
        print(dir + angle2 + "1000");
    }

    public void SerialClose()
    {
        sp.Close(); // 프로그램 종료시 포트 닫기
    }

    private void OnApplicationQuit()
    {
        sp.Write("w");
        print("끝");
        sp.Close();
    }
}