using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class coin : MonoBehaviour
{
    public GameObject wallPrefab;
    public float interval;
    public float range = 3.0f;
    public float DestroyXPos; // 닭이 사라지는 지점
    public float rotateSpeed = 180f;
    public Text score;
    public Text endScore;
    public int scoreT = 0;
    public AudioSource soundManager;
    public AudioSource backSoundManager;
    public AudioClip coinPlus;
    public Slider coinVolume;
    public Slider backVolume;

    private bool coinSoundFlag=true;
    private bool backSoundFlag = true;

    private float coinVol = 0.5f;
    private float backVol = 0.5f;

    public Sprite Back_on;
    public Sprite Back_off;
    public Sprite POP_on;
    public Sprite POP_off;

    public float V_Back;
    public float V_Coin;
    public float back_V;
    public float coin_V;

    public Button Back_B;
    public Button coin_B;

    private static coin _instance;
    public static coin instance
    {
        get { return _instance; }
    }
    private void Awake()
    {

        // Serial 운동 설정
        if (Data.Instance.GameID.Substring(0, 1) == "1") // 능동운동
            Serial.instance.Active();
        else if (Data.Instance.GameID.Substring(0, 1) == "4") // 저항운동
            Serial.instance.Resistance();

        _instance = GetComponent<coin>();
        coinVolume.value = coinVol;
        backVolume.value = backVol;
        soundManager.volume = coinVolume.value;
        backSoundManager.volume = backVolume.value;
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (true)
        {
            transform.position = new Vector3(transform.position.x, Random.Range(-range, range), transform.position.z);
            Instantiate(wallPrefab, transform.position, transform.rotation);
            yield return new WaitForSeconds(interval);
        }
    }

    // Update is called once per frame
    void Update()
    {
        score.text = "점수 : " + scoreT;
        endScore.text = "점수 : " + scoreT;
        coinSoundSlider();
        backSoundSlider();
        if (transform.position.x <= DestroyXPos)
        {
            Destroy(gameObject);
        }
        if (coinVolume.value == 0f)//슬라이더 값이 0이 되면
        {
            soundManager.volume = 0f;//볼륨을 0으로 하고
            coinVolume.value = 0f;//혹시 모르니 슬라이더 값도 0으로 하고
            coin_B.GetComponent<Image>().sprite = POP_off;//볼륨꺼짐으로 그림을 교체 합니다.
            coinSoundFlag = false;//그리고 효과음이 안나는 중이라 것을 확인해줍니다.
        }//'슬라이더'를 이용해서 볼륨이 0이 되는 경우
        else//슬라이더 값이 0이 아니면
        {
            coin_B.GetComponent<Image>().sprite = POP_on;//볼륨 켜짐으로 그림을 교체 합니다.
            coinSoundFlag = true;//소리 켜진거 확인
        }
        if (backVolume.value == 0f)//배경음도 똑같은 내용입니다. 효과음 대신 배경음을 따로 구현한것
        {
            backSoundManager.volume = 0f;
            backVolume.value = 0f;
            Back_B.GetComponent<Image>().sprite = Back_off;
            backSoundFlag = false;
        }
        else
        {
            Back_B.GetComponent<Image>().sprite = Back_on;
            backSoundFlag = true;
        }
    }

    public void coinSoundSlider()
    {
        soundManager.volume = coinVolume.value;
        coinVol = coinVolume.value;
    }
    public void PlaySound()
    {
        soundManager.PlayOneShot(coinPlus);
        // 유니티에서 기본으로 제공하는 함수 소리를 한번재생
    }

    public void backSoundSlider()
    {
        backSoundManager.volume = backVolume.value;
        backVol = backVolume.value;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
            scoreT++;            
        }
    }

    public void coinSoundBtn()
    {
        if (coinSoundFlag == true)
        {
            V_Coin = coinVolume.value;
            soundManager.volume = 0f ;
            coinVol = coinVolume.value;
            coin_B.GetComponent<Image>().sprite = POP_off;
            coinSoundFlag = false;
        }
        else
        {
            soundManager.volume = coinVolume.value;
            coinVol = coinVolume.value;
            coinSoundFlag = true;
        }
    }

    public void backSoundBtn()
    {
        if (backSoundFlag == true)
        {
            V_Back = backVolume.value;
            backSoundManager.volume = 0f;
            backVol = backVolume.value;
            Back_B.GetComponent<Image>().sprite = POP_off;
            backSoundFlag = false;
        }
        else
        {
            backSoundManager.volume = backVolume.value;
            backVol = backVolume.value;
            backSoundFlag = true;
        }
    }
}
