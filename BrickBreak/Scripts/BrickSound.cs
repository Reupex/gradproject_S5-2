using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 단순히 소리 뿐만 아니라 음량조절도 전부 여기에 포함되어 있습니다.
 * 대부분 똑같은걸 효과음과 배경음 두개로 만들었습니다
 */

public class BrickSound : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioSource B_audioSource;

    public Slider Back_V;
    public Slider pop_V;

    public Button Back_B;
    public Button pop_B;

    public float BV;//배경음
    public float popV;//효과음

    public AudioClip popSpund;
    public AudioClip BackSound;

    public Sprite Back_on;
    public Sprite Back_off;
    public Sprite POP_on;
    public Sprite POP_off;

    public bool checkB;
    public bool checkPOP;

    public float V_Back;
    public float V_POP;


    public static BrickSound instance;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    //배경음이랑 효과음이랑 따로 조종할 수 있도록 각각 컴포넌트를 만들었습니다.
    void Start()
    {
        this.audioSource = this.gameObject.AddComponent<AudioSource>();//효과음 컴포넌트
        this.audioSource.clip = this.popSpund;
        this.audioSource.loop = false;//반복 안함
        this.B_audioSource = this.gameObject.AddComponent<AudioSource>();//배경음 컴포넌트
        this.B_audioSource.clip = this.BackSound;
        this.B_audioSource.loop = true;//반복

        B_audioSource.volume = pop_V.value;//기본 볼륨 슬라이더 기본값 0.5
        B_audioSource.volume = Back_V.value;

        this.B_audioSource.Play();//배경음악은 틀어줍니다

        checkB = true;//배경 소리가 나오는 중으로 확인 해줍니다
        checkPOP = true;//효과음도 확인해줍니다
}

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = pop_V.value;//슬라이더 값을 볼륨값으로 해줍니다
        B_audioSource.volume = Back_V.value;//슬라이더 값 범위 0~1

        if(pop_V.value == 0f)//슬라이더 값이 0이 되면
        {
            audioSource.volume = 0f;//볼륨을 0으로 하고
            pop_V.value = 0f;//혹시 모르니 슬라이더 값도 0으로 하고
            pop_B.GetComponent<Image>().sprite = POP_off;//볼륨꺼짐으로 그림을 교체 합니다.
            checkPOP = false;//그리고 효과음이 안나는 중이라 것을 확인해줍니다.
        }//'슬라이더'를 이용해서 볼륨이 0이 되는 경우
        else//슬라이더 값이 0이 아니면
        {
            pop_B.GetComponent<Image>().sprite = POP_on;//볼륨 켜짐으로 그림을 교체 합니다.
            checkPOP = true;//소리 켜진거 확인
        }
        if (Back_V.value == 0f)//배경음도 똑같은 내용입니다. 효과음 대신 배경음것을 따로 구현한것
        {
            B_audioSource.volume = 0f;
            Back_V.value = 0f;
            Back_B.GetComponent<Image>().sprite = Back_off;
            checkB = false;
        }
        else
        {
            Back_B.GetComponent<Image>().sprite = Back_on;
            checkB = true;
        }
    }

    public void blockPoP()//블록 스크립트에서 소리를 내기 위해 가져가는 함수
    {
        this.audioSource.Play();//효과음입니다
    }

    public void StopBack()//다른 스크립트에서 배경음악 멈추기 위해 가져가는 함수
    {
        this.B_audioSource.Pause();
    }

    public void playBack()//다른 스크립트에서 배경음악 다시 재생하기 위해 가져가는 함수
    {
        this.B_audioSource.Play();
    }

    public void cleckBack()//배경음의 버튼을 눌렀을 경우
    {
        muteBack(checkB);//뮤트 배경음(아래줄에 있습니다)에 소리가 켜졌는지 안켜졌는지 확인하는 것을 줍니다.
    }

    public void cleckPOP()//효과음의 버튼을 눌렀을 경우
    {
        mutePop(checkPOP);//뮤트 효과음(아래줄에 있습니다)에 소리가 켜졌는지 안켜졌는지 확인하는 것을 줍니다.
    }

    public void muteBack(bool mute)//뮤트 배경음
    {
        if(mute == true)//소리가 켜져 있으면
        {
            V_Back = Back_V.value;//현재 볼륨의 크기를 기억해 놓습니다
            B_audioSource.volume = 0f;//볼륨 0
            Back_V.value = 0f;//슬라이더도 0
            Back_B.GetComponent<Image>().sprite = Back_off;//그림 교체
            checkB = false;//소리 꺼짐으로 확인
        }
        else//소리가 안켜져 있으면
        {
            B_audioSource.volume = V_Back;//기억 해뒀던 볼륨의 크기로 볼륨을 올립니다
            Back_V.value = V_Back;//슬라이더도 똑같이 올려줍니다
            Back_B.GetComponent<Image>().sprite = Back_on;//그림 교체
            checkB = true;//소리 켜짐으로 확인
        }
    }

    public void mutePop(bool mute)//뮤트 효과음, 뮤트 배경음이랑 똑같으나 효과음인것만 다릅니다.
    {
        if (mute == true)
        {
            V_POP = pop_V.value;
            audioSource.volume = 0f;
            pop_V.value = 0f;
            pop_B.GetComponent<Image>().sprite = POP_off;
            checkPOP = false;
        }
        else
        {
            audioSource.volume = V_POP;
            pop_V.value = V_POP;
            pop_B.GetComponent<Image>().sprite = POP_on;
            checkPOP = true;
        }
    }
}
