using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EscapeSound : MonoBehaviour
{
    #region 
    public AudioClip menu_sound; //버튼 소리
    public AudioClip menu_door; //문 버튼 소리
    public AudioClip success_sound; //성공 소리
    public AudioClip door_sound; //문 열리고 탈출하는 소리
    public AudioClip cave_sound; //동굴 무너지는 소리
    public AudioClip tick_sound; //암호 돌아가는 소리
    public AudioSource[] myAudio1 = new AudioSource[4]; //음악 플레이어
    public AudioSource myAudio2; //음악 플레이어 배경음악
    public Slider backVSlider1; //효과음
    public Slider backVSlider2;//배경음악
    private float backvol = 0.5f;
    private float backvol2 = 0.5f;
    private bool check;
    public Sprite OnSprite1;
    public Sprite OffSprite1;
    public Sprite OnSprite2;
    public Sprite OffSprite2;
    public Button btn;
    public Button btn2;
    #endregion

    #region Singleton
    public static EscapeSound instance;

    void Awake()
    {
        if (EscapeSound.instance == null)
        {
            EscapeSound.instance = this;
        }
    }
    #endregion

    void Start()
    {
        //btn2는 효과음 backVSlider1으로 조절 myAudio1
        //btn은 배경음 backVSlider2로 조절 myAudio2

        btn.GetComponent<Image>().sprite = OnSprite1;
        btn2.GetComponent<Image>().sprite = OnSprite2;
        if (myAudio2 == null)
            check = false;
        else
            check = true;

        backvol = PlayerPrefs.GetFloat("backvol", 0.5f);
        backVSlider2.value = backvol;
        myAudio2.volume = backVSlider2.value;

        backvol2 = PlayerPrefs.GetFloat("backvol2", 0.5f);
        backVSlider1.value = backvol2;
        for (int i = 0; i < myAudio1.Length; i++)
        {
            myAudio1[i].volume = backVSlider1.value;
        }
    }
    private void Update()
    {
        if (check == true)
        {
            soundslider();
            soundslider2();
        }
        else
            return;
        ZeroSound();
    }

    public void soundslider()
    {
        myAudio2.volume = backVSlider2.value;
        backvol = backVSlider2.value;
        PlayerPrefs.SetFloat("backvol", backvol);

    }

    public void soundslider2()
    {
        for (int i = 0; i < myAudio1.Length; i++)
        {
            myAudio1[i].volume = backVSlider1.value;
        }

        backvol2 = backVSlider1.value;
        PlayerPrefs.SetFloat("backvol2", backvol2);
    }
    public void menu_Click()    //일반 메뉴
    {
        myAudio1[0].PlayOneShot(menu_sound);
    }
    public void menu_Door() //문 버튼 클릭
    {
        myAudio1[1].PlayOneShot(menu_door);
    }
    public void Success()   //잠금 엶
    {
        myAudio1[1].PlayOneShot(success_sound);
    }
    public void OpenDoor()  //탈출하는 소리
    {
        myAudio1[2].PlayOneShot(door_sound);
    }
    public void Cave()  //동굴 무너지는 소리
    {
        myAudio1[3].PlayOneShot(cave_sound);
    }
    public void Tick()  //암호 돌리는 소리
    {
        myAudio1[4].PlayOneShot(tick_sound);
    }
    public void no_backsound()//배경음
    {
        if (myAudio2.volume == 0)
        {
            btn.GetComponent<Image>().sprite = OnSprite1;
            myAudio2.volume = 0.5f;
            backVSlider2.value = 0.5f;
        }
        else
        {
            btn.GetComponent<Image>().sprite = OffSprite1;
            myAudio2.volume = 0;
            backVSlider2.value = 0;
        }
    }
    public void no_backsound2()//효과음
    {
        for (int i = 0; i < 4; i++)
        {
            if (myAudio1[i].volume == 0)
            {
                btn2.GetComponent<Image>().sprite = OnSprite2;
                myAudio1[i].volume = 0.5f;
                backVSlider1.value = 0.5f;
            }
            else
            {
                btn2.GetComponent<Image>().sprite = OffSprite2;
                myAudio1[i].volume = 0f;
                backVSlider1.value = 0f;
            }
        }
    }
    public void ZeroSound()
    {
        if (backVSlider2.value == 0)
            btn.GetComponent<Image>().sprite = OffSprite1;
        if (backVSlider2.value > 0)
            btn.GetComponent<Image>().sprite = OnSprite1;
        if (backVSlider1.value == 0)
            btn2.GetComponent<Image>().sprite = OffSprite2;
        if (backVSlider1.value > 0)
            btn2.GetComponent<Image>().sprite = OnSprite2;
    }
}
