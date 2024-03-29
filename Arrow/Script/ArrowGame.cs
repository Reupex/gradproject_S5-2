﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
                     
/*
 * 게임 매니저 스크립트
 */

// GameManager : 게임 진행 규칙들을 관리하는 매니저이다. / 게임 오버 관리: 플레이어 사망 시 사망 상태를 적용하여 게임 오버를 관리한다. 
// 게임 오버UI 활성화: 플레이어가 사망 상태 시 UIManager에 접근하여 GameoverUI를 출력한다.
// 점수 획득: 플레이어가 적을 처치 시 해당 적의 점수를 획득한다. / 초기 점수 설정: 게임 시작 시 초기 점수(0점)을 설정한다.
public class ArrowGame : MonoBehaviour
{
    #region 변수
    public MonsterType monsterType;
    public GameObject[] Life = new GameObject[4];     // 남은 목숨을 저장할 배열
    public int life;                                  // 남은 목숨, 하트 갯수
    public int count = 0;
    public Text RESULT_time;
    public Text RESULT_score;
    public Text RESULT_stage;
    public Text GAME_score;
    public Text STAGE_stage;
    public Text Count;

    // 화살, 몬스터 생성관련 변수들
    public float time; // Time.deltatime값 누적하는 변수
    public float nextTime_Arrow = 0.0f;
    public float Timeplus_Arrow = 0.8f; // 화살 생성 주기 현재 : 1.0초
    public GameObject Arrow_Prefabs;  // 화살 복제시 사용할 프리팹
    public GameObject Arrow; // 화살 // bulletPos값을 저장하기 위해 초기 화살 오브젝트 변수 생성
    public Vector3 ArrowPos; // 화살 초기 위치(화살 생성 좌표값)
    public GameObject info;   // 화살 발사위치

    public float nextTime_Monster = 0.0f;
    public float Timeplus_Monster = 2.0f; // 몬스터 생성 주기 현재 : 1.5초

    // 몬스터 오브젝트
    public GameObject Monster_Prefabs;  // 몬스터 복제시 사용할 프리팹
    public List<GameObject> monster = new List<GameObject>();
    public int monsterMaxSpwanCount; // 최대 몬스터 수
    public Transform[] spawnPoints; // 몬스터 생성시 위치 배열
    public int monster_count = 0;

    // 오브젝트 이동속도
    public float MonsterSpeed; // 몬스터 이동속도
    public int ArrowSpeed = 5; // 화살 이동속도

    // 스테이지별로 변하는 값들 배열로 선언
    public int[] stage_up = new int[5] { 20, 50, 90, 140, 200 };  // 실제
    //public int[] stage_up = new int[5] { 4, 9, 15, 21, 28 };    // 테스트용

    public float[] Monster_Speed_Real;
    public float[] Monster_Speed_Test;
    public float[] Monster_Spawn;

    public int[] Arrow_Speed_Real; // 실제
    public int[] Arrow_Speed_Test; // 테스트용
    public float[] Arrow_Spawn;    // 실제

    public bool isTuto;
    public float SliderMinValue;
    public float SliderMaxValue;
    public float Rotation_angle; // 회전각

    List<List<int>> Row = new List<List<int>>() {  new List<int> { 0, 0 }, new List<int> { 0, 0 },
                                 new List<int> { 0, 0 }, new List<int> { 2, 5 },   // 3라인
                                 new List<int> { 0, 0 }, new List<int> { 1, 6 },   // 5라인
                                 new List<int> { 0, 0 }, new List<int> { 0, 7 } }; // 7라인

    // 슬라이더 이벤트를 처리하는 2가지 방법
    // 1. 슬라이더의 값이 바뀔때, 함수를 호출하는 방법
    // 2. 스트립트에서 슬라이더에 접근해서 처리하는 방식 -> 사용

    // 슬라이더 관련
    public Slider slider; // 20 ~ 160 정수값 slider.value
    public string type;
    public int UPcircle = 0;
    public int TotalRotate = 0;
    public float checkpoint = 0;
    public float times;
    public float slider_value_backup;
    public List<float> Change_Point = new List<float>(2); // 0, 359
    public float Change_Time;
    #endregion

    #region Singleton                                 // 싱글톤 패턴은 하나의 인스턴스에 전역적인 접근을 시키며 보통 호출될 때 인스턴스화 되므로 사용하지 않는다면 생성되지도 않습니다.

    private static ArrowGame _Instance;             // 싱글톤 객체 선언, 어디에서든지 접근할 수 있도록 하기위해 

    public static ArrowGame Instance                // 객체에 접근하기 위한 속성으로 내부에 get set을 사용한다.
    {
        get { return _Instance; }                     // GameManager 객체 리턴
    }

    void Awake()                                      // 제일 처음 호출되는 함수
    {
        _Instance = GetComponent<ArrowGame>();      // _gManager라는 변수에 자신의 GameManager 컴포넌트를 참조하는 값을 저장, Game속성에 set코드를 짜면 다르게 대입가능
        if (Data.Instance.GameID.Substring(0, 2) == "32") // 수평일 때
            type = "수평";
        else
            type = "";

        isTuto = false;
        Monster_Speed_Real = new float[5] { 0.020f, 0.024f, 0.028f, 0.032f, 0.036f }; // 실제
        Monster_Speed_Test = new float[5] { 0.2f, 0.5f, 0.8f, 1.1f, 2f };  // 테스트용
        Monster_Spawn = new float[5] { 2.0f, 1.9f, 1.8f, 1.65f, 1.5f }; // 실제

        MonsterSpeed = 0.016f; // 몬스터 이동속도 초기화 - 실제
        // MonsterSpeed = 0.08f; // 몬스터 이동속도 초기화 - 테스트용

        // stage_up = new int[5] { 20, 50, 90, 140, 200 };  // 실제
        stage_up = new int[5] { 10, 20, 30, 40, 50 };    // 약간 테스트
        // stage_up = new int[5] { 4, 8, 12, 16, 20 };       // 테스트

        Arrow_Speed_Real = new int[5] { 7, 7, 8, 8, 9 }; // 실제
        Arrow_Speed_Test = new int[5] { 7, 7, 8, 8, 9 }; // 테스트용
        Arrow_Spawn = new float[5] { 1.0f, 1.0f, 0.9f, 0.9f, 0.8f }; // 실제
        ArrowSpeed = 6;

        slider.value = (slider.maxValue + slider.minValue) / 2;
        if (type == "수평")
            slider.value = 0f;

        monsterMaxSpwanCount = 25;  // 최대 몬스터 수
        for (int i = 0; i < monsterMaxSpwanCount; i++) {  //1234
            monster.Add(null);
        }
        RESULT_stage.text = "1";
        STAGE_stage.text = "1";
        //stage&life 변수 초기화
        life = 3;

        SliderMinValue = slider.minValue;
        SliderMaxValue = slider.maxValue;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Serial.instance.Active(); // 연동 코드 - 디바이스 능동모드로 바꿔줌
        nextTime_Arrow += Timeplus_Arrow; // 제일 처음 화살과 겹쳐서 1초 이후에 부터 1초마다 생성시키기 위함
        // slider = GetComponent<Slider>();
        // Debug.Log("Arrow_Spawn : " + Arrow_Spawn);
        ArrowPos = Arrow.gameObject.transform.position; // 화살의 현재 위치를 받아온다.
        if (isTuto) { ArrowPos.z = 95f; }
        times = 0;
        Change_Point.Add(0f);
        Change_Point.Add(359f);
    }
    // 화살은 충돌시 말고 1초마다 생성 - 1초는 테스트후 조정
    void Update()
    {
        if (!ArrowUI.Instance.pause) // 정지중일땐 time 안 올라가게~
            time += Time.deltaTime;
        Create_Object(); //1234
        ChangeColorAndRotation();
        Slider_MinMax_Value();

        // 수평 -> 추가
        if (type == "수평") { 
            // 회전수 카운트
            RotationCount();

            // 슬라이드 양쪽 끝에서 일정시간 머무르면 반대편으로 옮겨줌
            // Left_Right_Change();
            times += Time.deltaTime;
        }
    }
    // 수평 -> 추가
    public void Left_Right_Change()
    {
        // Debug.Log("times % Change_Time : " + times % Change_Time);
        // 1초마다 slider_value 값 가져옴
        if (times % Change_Time <= 0.1) { 
            slider_value_backup = slider.value;
        }
        bool change = true;
        if (times <= Change_Time)
        {
            if (slider_value_backup != slider.value)
            {
                change = false;
                times = 0;
            }
        }

        if(change) { 
            if (times >= Change_Time) { 
                if(slider_value_backup == Change_Point[1]) // 359라면
                    if (checkpoint != Change_Point[0])
                        slider.value = Change_Point[0]; // 0으로 변경해줌

                if (slider_value_backup == Change_Point[0])
                    if (checkpoint != Change_Point[1])
                        slider.value = Change_Point[1];

                times = 0;
            }
        }
    }
    // 수평 -> 추가
    public void RotationCount()
    {
        if (slider.value < 40 && slider.value >= slider.minValue) //  0 < 슬라이더 값 < 40
        {
            if (checkpoint == slider.maxValue) // 미들 값이 360이라면
            {
                UPcircle++;          // 정방향 회전
                TotalRotate++;
                print(UPcircle);
            }
            checkpoint = 0; // 미들값 0으로 초기화
        }
        else if (slider.value < 300 && slider.value > 100)               // 100 < 슬라이더 값 < 300
        {
            checkpoint = 180; // 미들 값은 180으로 함
        }
        else if (slider.value > 320 && slider.value <= slider.maxValue) // 320 < 슬라이더 값 < 360
        {
            if (checkpoint == slider.minValue) // 미들 값이 0이라면
            {
                UPcircle--;           // 반대로 회전
                TotalRotate++;
                print(UPcircle);
            }
            checkpoint = 359; // 미들 값은 359로함
        }
        if (UPcircle > 1)
            UPcircle = 1;
        else if (UPcircle < -2)
            UPcircle = -2;
    }

    public void Slider_MinMax_Value()
    {
        if (slider.value < SliderMinValue)
        {
            SliderMinValue = slider.value;
        }
        if (slider.value > SliderMaxValue)
        {
            SliderMaxValue = slider.value;
        }
    }

    public void ChangeColorAndRotation()
    {
        var device = bullet.Instance.slider.GetComponent<Slider>();

        //Quaternion Direction = Quaternion.Euler(0, 0, (180 - device.value));
        //info.transform.rotation = Direction;

        // 수평일때 안내 바(origin) 회전은 -> bullet에서 관리
        if (type != "수평") {
            
            Rotation_angle = (float)(138.8 - (168 / (device.maxValue - device.minValue) * (device.value - device.minValue) + 20));
            info.transform.rotation = Quaternion.Euler(0, 0, Rotation_angle);
        }

        // 가운데 일때 색상변경 - 영점 맞추기
        Rotation_angle = 180 - (140 / (device.maxValue - device.minValue) * (device.value - device.minValue) + 20);
        if (Rotation_angle == 89 || Rotation_angle == 90 || Rotation_angle == 91)
            GameObject.Find("orignOne").GetComponent<Image>().color = HexToColor("FF5E56AA");
        else if (Rotation_angle == 20 || Rotation_angle == 160)
            GameObject.Find("orignOne").GetComponent<Image>().color = HexToColor("FF5E56AA");
        else
            GameObject.Find("orignOne").GetComponent<Image>().color = HexToColor("27F6F6AA");
    }

    public Color HexToColor(string hex)
    {
        hex = hex.Replace("0x", "");
        hex = hex.Replace("#", "");
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte a = 255;
        if(hex.Length == 8)
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, a);
    }
    public void Stage_Check(int i)
    {
        RESULT_stage.text = (i + 2).ToString();
        STAGE_stage.text = "스테이지 : " + (i + 2).ToString();
        // Debug.LogError((i + 2) + "스테이지 진입");
        Debug.Log((i + 2) + "스테이지 진입");

        ArrowUI.Instance.StagePanel_on(i);
        // MonsterSpeed = Monster_Speed_Test[i];
        MonsterSpeed = Monster_Speed_Real[i];
        ArrowSpeed = Arrow_Speed_Test[i];
        
        Count.text = "잡은 몬스터 수 : " + count.ToString();
        Debug.Log("MonsterSpeed : " + MonsterSpeed);
        Debug.Log("ArrowSpeed : " + ArrowSpeed);
    }
    public void countPlus()
    { 
        count++;
        GAME_score.text = "점수 : " + (count * 10).ToString();
        for (int i = stage_up.Length - 1; i >= 0; i--)
        {
            if (count == stage_up[i])
                Stage_Check(i);
        }
        // 게임점수가 최고점수를 갱신했을 경우
        if (ArrowUI.Instance.maxScore < (count * 10))
        {
            ArrowUI.Instance.maxScore = (count * 10);
            ArrowUI.Instance.MaxScore_Text.text = "최고점수 : " + ArrowUI.Instance.maxScore;
        }

    } // 몬스터 처치 횟수 증가

    void Create_Object() // 화살, 몬스터 생성코드
    {
        // 1초마다 실행 - 화살 생성
        if (time > nextTime_Arrow)
        {
            // Debug.Log("화살 생성 : " + time);
            Create_Bullet();
            nextTime_Arrow += Timeplus_Arrow; // Timeplus : 몬스터 생성주기

            //SpriteChange();
        }

        // 1.5초마다 실행 - 몬스터 생성
        if (time > nextTime_Monster)
        {
            // Debug.Log("몬스터 생성 : " + time);
            Create_Monster(); //1234
            nextTime_Monster += Timeplus_Monster; // Timeplus : 몬스터 생성주기
        }
    }

    public void Create_Bullet() // 새로운 화살 생성
    {
        // 초기 위치(bulletPos)에 화살을 생성시키는 코드
        // var t = Instantiate(Arrow_Prefabs, ArrowPos, Quaternion.identity, GameObject.Find("Canvas").transform.Find("GameObject").transform); // 새로운 화살 생성 Quaternion.identity : 회전값 지정 - 불필요   
        var t = Instantiate(Arrow_Prefabs, ArrowPos, Quaternion.identity); //, GameObject.Find("Canvas").transform); // 새로운 화살 생성 Quaternion.identity : 회전값 지정 - 불필요   
        t.transform.Rotate(new Vector3(0, 0, 90f));
        ArrowSound.Instance.Arrow();
    }

    public void Create_Monster() //1234
    {
        // stage_up = new int[5] { 40, 90, 150, 210, 280 };
        // Monster_Spawn = new float[5] { 2.0f, 1.7f, 1.3f, 1.0f, 0.7f }; // 실제
        // Arrow_Spawn = new float[5] { 0.9f, 0.8f, 0.7f, 0.6f, 0.5f }; // 실제

        for(int i = 0; i < stage_up.Length; i++)  {
            if (count <= stage_up[i]) {
                Timeplus_Monster = Monster_Spawn[i];
                Timeplus_Arrow = Arrow_Spawn[i];
                break;
            }
        }
        if (count > stage_up[4]) {
            Timeplus_Monster = Monster_Spawn[4] - (count - stage_up[4]) * 0.01f;
            Timeplus_Arrow = 0.7f;
        }

        // monster_count = GameObject.FindGameObjectsWithTag("Monster").Length;
        for (int i = 0; i < monster.Count; i++)
        {
            if (monster[i] == null)
            {
                int x = Random.Range(2, 5); // 디폴트는 3라인
                if (Timeplus_Arrow == Arrow_Spawn[2])
                    x = Random.Range(1, 6); // 스테이지 3단계면 5라인
                else if (Timeplus_Arrow < Arrow_Spawn[4])
                    x = Random.Range(0, 7); // 스테이지 5단계면 7라인

                //int x = 3; // - 가운데로만 몬스터가 나오는 테스트용
                monster[i] = Instantiate(Monster_Prefabs, spawnPoints[x].position, Quaternion.identity); //count는 처리한 몬스터 수
                return;
            }
        }
    }

    public enum MonsterType
    {
        normal
    }
}