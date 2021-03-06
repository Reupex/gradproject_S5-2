﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowSound : MonoBehaviour
{
    public AudioSource myAudio1;
    public AudioSource myAudio2;
    public AudioClip monster_die;     // 몬스터 사망
    public AudioClip GameOver;        // 말그대로 ㅇㅇ
    public AudioClip Stage_up;        // 스테이지 상승
    public AudioClip Button;          // 버튼클릭 효과음
    public AudioClip arrow;           // 화살 발사음
    public AudioClip player_damage;

    // 메뉴패널 슬라이더 조절
    public Button btn1;
    public Button btn2;
    public Sprite btn1_Sprite_on;
    public Sprite btn1_Sprite_off;
    public Sprite btn2_Sprite_on;
    public Sprite btn2_Sprite_off;
    public Slider backVSlider1;
    public Slider backVSlider2;
    public float backvol1 = 0.5f;
    public float backvol2 = 0.5f;
    public float backupvol1;
    public float backupvol2;
    public bool clear = false;
    // public AudioClip player_damage;      // life 감소시


    // 싱글톤 패턴
    #region Singleton
    public static ArrowSound Instance;    // 싱글톤 패턴을 사용하기 위한 인스턴스 변수, static으로 선언하여 어디서든 접근 가능

    // 인스턴스 변수 초기화
    void Awake()
    {
        // 인스턴스가 생성되지 않았을 때 (인스턴스 중복 생성을 막기 위함)
        if (ArrowSound.Instance == null)
        {
            ArrowSound.Instance = this;   // 자기 자신을 참조하는 인스턴스 생성
        }
        backupvol1 = 0f;
        backupvol2 = 0f;
    }
    #endregion

    // Use this for initialization
    void Start()
    {
        myAudio1 = GetComponents<AudioSource>()[0];  // 오디오 소스(SoundManager 오브젝트)를 가져와 myAudio에 저장

        if(ArrowSceneManager.Instance.isGame)
        {
            backvol1 = PlayerPrefs.GetFloat("backvol1", 0.5f);
            backVSlider1.value = backvol1;
            myAudio1.volume = backVSlider1.value;

            backvol2 = PlayerPrefs.GetFloat("backvol2", 0.5f);
            backVSlider2.value = backvol2;
            myAudio2.volume = backVSlider2.value;
        }
        
    }

    private void Update()
    {
        if(!clear && ArrowSceneManager.Instance.isGame)
        {
            soundslider1();
            soundslider2();
        }
    }

    public void soundslider1()
    {
        myAudio1.volume = backVSlider1.value;
        backvol1 = backVSlider1.value;
        PlayerPrefs.SetFloat("backvol", backvol1);
        if (backVSlider1.value != 0)
            btn1.GetComponent<Image>().sprite = btn1_Sprite_on;
        else
            btn1.GetComponent<Image>().sprite = btn1_Sprite_off;
    }
    public void soundslider2()
    {
        myAudio2.volume = backVSlider2.value;
        backvol2 = backVSlider2.value;
        PlayerPrefs.SetFloat("backvol", backvol2);
        if (backVSlider2.value != 0)
            btn2.GetComponent<Image>().sprite = btn2_Sprite_on;
        else
            btn2.GetComponent<Image>().sprite = btn2_Sprite_off;
    }
    public void no_backsound1()
    {
        if (backVSlider1.value == 0)
        {
            myAudio1.volume = backupvol1;
            backVSlider1.value = backupvol1;
            btn1.GetComponent<Image>().sprite = btn1_Sprite_on;
        }
        else
        {
            backupvol1 = myAudio1.volume;
            myAudio1.volume = 0f;
            backVSlider1.value = 0f;
            btn1.GetComponent<Image>().sprite = btn1_Sprite_off;
        }
    }

    public void no_backsound2()
    {
        if (backVSlider2.value == 0)
        {
            myAudio2.volume = backupvol2;
            backVSlider2.value = backupvol2;
            btn2.GetComponent<Image>().sprite = btn2_Sprite_on;
        }
        else
        {
            backupvol2 = myAudio2.volume;
            myAudio2.volume = 0f;
            backVSlider2.value = 0f;
            btn2.GetComponent<Image>().sprite = btn2_Sprite_off;
        }
                
    }
    public void Btn_Click_Main() // 클릭 버튼음
    {
        myAudio1.PlayOneShot(Button); // 오디오 소스로 소리를 한 번 재생시킴
    }

    public void Btn_Click() // 클릭 버튼음
    {
        myAudio2.PlayOneShot(Button); // 오디오 소스로 소리를 한 번 재생시킴
    }
    public void Monster_die() // 몬스터 사망음
    {
        myAudio2.PlayOneShot(monster_die); // 오디오 소스로 소리를 한 번 재생시킴
    }
    public void stageUp() // 몬스터 사망음
    {
        myAudio2.PlayOneShot(Stage_up); // 오디오 소스로 소리를 한 번 재생시킴
    }
    public void Gameover() // 몬스터 사망음
    {
        // 배경음이랑 물려서 수정
        myAudio1.volume = 0f;
        clear = true;
        myAudio2.PlayOneShot(GameOver); // 오디오 소스로 소리를 한 번 재생시킴
    }
    public void Arrow()
    {
        myAudio2.PlayOneShot(arrow); // 오디오 소스로 소리를 한 번 재생시킴
    }
    public void Player_damage() // life 감소음
    {
        myAudio2.PlayOneShot(player_damage); // 오디오 소스로 소리를 한 번 재생시킴
    }
}
