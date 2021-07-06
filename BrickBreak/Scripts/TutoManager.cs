using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * 튜토리얼 게임 말고 튜토리얼 씬을 관리해주는 매니저 스크립트 입니다.
 * 대부분 버튼을 누르면 다음 컷을 보여주는 내용입니다.
 */

public class TutoManager : MonoBehaviour
{
    public GameObject OnMain;
    public GameObject OnGame;

    public GameObject endP;
    public GameObject sucP;
    public GameObject menuP;

    public GameObject N1;
    public GameObject N2;
    public GameObject N3;
    public GameObject N4;
    public GameObject N5;
    public GameObject N6;
    public GameObject N7;
    public GameObject N8;
    public GameObject N9;

    public AudioClip tutoSound;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        OnGame.SetActive(false);//일딴 처음꺼 빼고 다 안보이게 합니다.
        endP.SetActive(false);
        sucP.SetActive(false);
        menuP.SetActive(false);
        N2.SetActive(false);
        N3.SetActive(false);
        N4.SetActive(false);
        N5.SetActive(false);
        N6.SetActive(false);
        N7.SetActive(false);
        N8.SetActive(false);
        N9.SetActive(false);

        this.audioSource = this.gameObject.GetComponent<AudioSource>();
        this.audioSource.clip = this.tutoSound;
        this.audioSource.loop = true;
        this.audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void N1Next()//첫장 버튼
    {
        N1.SetActive(false);
        N2.SetActive(true);
    }

    public void N2Next()//2장 버튼
    {
        N2.SetActive(false);
        OnMain.SetActive(false);
        OnGame.SetActive(true);
        N3.SetActive(true);
    }

    public void N3Next()//3장..
    {
        N3.SetActive(false);
        N4.SetActive(true);
    }

    public void N4Next()//4..
    {
        N4.SetActive(false);
        N5.SetActive(true);
    }

    public void N5Next()//5..
    {
        N5.SetActive(false);
        menuP.SetActive(true);
        N6.SetActive(true);
    }

    public void N6Next()//6..
    {
        N6.SetActive(false);
        N7.SetActive(true);
    }

    public void N7Next()//7..
    {
        N7.SetActive(false);
        N8.SetActive(true);
    }

    public void N8Next()//8..
    {
        N8.SetActive(false);
        N9.SetActive(true);
    }

    public void N9Next()//9장 버튼을 누르면 튜토리얼 게임 씬으로 옮겨갑니다. 게임 스테이지는 1로 시작합니다.
    {
        StageManager.instance.Stage = 1;
        SceneManager.LoadScene("TutoGame");
    }
}
