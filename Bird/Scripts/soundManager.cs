using UnityEngine;
using System.Collections;

public class soundManager : MonoBehaviour
{
    public AudioClip soundExplosion;   //Audioclip이라는 데이터타입에 변수생성
    AudioSource myAudio; //컴퍼넌트에서 AudioSource가져오기
    public static soundManager instance; //다른 스크립트에서 이스크립트에있는 함수를 호출할때 쓰임

    void Awake()  // Start함수보다 먼저 호출됨
    {
        if (soundManager.instance == null)  //게임시작했을때 이 instance가 없을때
            soundManager.instance = this;  // instance를 생성
    }
    // Use this for initialization
    void Start()
    {
        //myAudio = GetComponent<AudioSource>();  //myAudio에 컴퍼넌트에있는 AudioSource넣기
    }

    public void PlaySound()
    {
        myAudio.PlayOneShot(soundExplosion);
        // 유니티에서 기본으로 제공하는 함수 (이 소리)를 한번재생
    }

    // Update is called once per frame
    void Update()
    {

    }
}