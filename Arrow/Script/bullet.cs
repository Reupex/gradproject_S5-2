﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bullet : MonoBehaviour
{
    #region Singleton  
    public GameObject Bullet; // 화살 - 초기 위치값 가져오기위해 선언

    // 슬라이더 값에 따라 회전하게 하기
    public float time = 0f;
    public float slider_value;   // 처리전 슬라이더 값
    public float Rotation_angle; // 처리후 회전각

    // 슬라이더 관련
    public GameObject slider; // 20 ~ 160 정수값 slider.value

    private static bullet _Instance;          // 싱글톤 패턴을 사용하기 위한 인스턴스 변수, static 선언으로 어디서든 참조가 가능함
    public static bullet Instance                    // 객체에 접근하기 위한 속성으로 내부에 get set을 사용한다.
    {
        get { return _Instance; }                         // _sceneManager이 변수값을 리턴받을 수 있음.
    }

    void Awake()                                               // Start()보다 먼저 실행
    {
        _Instance = GetComponent<bullet>();    // _sceneManager변수에 자신의 SceneChangeManager 컴포넌트를 넣는다.
        slider = GameObject.Find("Slider");
        
        slider_value = slider.GetComponent<Slider>().value;   // - 능동시 슬라이더를 사용자가 움직이는 대로 값을 받음
        //  Serial.instance.Passive(); // 수동모드 시작
        
        
    }
    #endregion 

    // Update is called once per frame
    void Update()
    {
        BulletRotate();
        time += Time.deltaTime;
    }

    void BulletRotate() {
        if (!ArrowUI.Instance.pause)
        {
            // WriteAngle : angle을 펌웨어 명령어에 맞게 변경해주는 함수
            // Pa_Motor : 수동 모드 - 모터 제어 매개값 예시 "0(시계방향)180(각도)2000(속도)"
            // Serial.instance.Pa_Motor("0" + Serial.instance.WriteAngle((int)Rotation_angle) + "2000");

            var device = slider.GetComponent<Slider>();

            if (ArrowGame.Instance.type == "수평")
            {
                var t = ArrowGame.Instance.UPcircle;
                Rotation_angle = 90 - t * 35f - (35 / (device.maxValue - device.minValue) * (device.value - device.minValue));
                // ArrowGame.Instance.info.transform.rotation = Quaternion.Euler(0, 0, Rotation_angle - 55);
                ArrowGame.Instance.info.transform.rotation = Quaternion.Euler(0, 0, Rotation_angle + ((Rotation_angle - 90) * 0.15f) - 55);
            }
            else
            {
                Rotation_angle = 180 - (140 / (device.maxValue - device.minValue) * (device.value - device.minValue) + 20);
            }

            if (time < 0.2f)
            {
                transform.rotation = Quaternion.Euler(0, 0, Rotation_angle);
                // Debug.Log("Rotation_angle : " + Rotation_angle);            
            }

            transform.Translate(Vector3.left * -1f * Time.deltaTime * ArrowGame.Instance.ArrowSpeed, Space.Self);  //(0,1,0)
        }
    }

    void OnTriggerEnter2D(Collider2D collision) // 화살 제거
    {
        // Arrow_Destory : 위에 생성된 몬스터가 화면에 나타나기전에 미리 명중되는 것을 막기위해
        if (collision.gameObject.tag == "Border" || collision.gameObject.tag == "Arrow_Destory")
        {
            Destroy(gameObject);
        }
            
        if (collision.gameObject.tag == "Monster")
        {
            if(!ArrowGame.Instance.isTuto) // 튜토리얼에서 몬스터 생성후 안내중 화살에 제거되는 거 방지
            {
                ArrowGame.Instance.countPlus();
                ArrowSound.Instance.Monster_die();
                Destroy(collision.gameObject);
                Destroy(gameObject);
            }
        }
    }
}