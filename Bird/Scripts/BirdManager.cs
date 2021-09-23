using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BirdManager : MonoBehaviour
{

    public GameObject endNel, pickNel, startNel, cNel, conNel, dNel, gameNel, menuNel;
    public GameObject momo, coco, gari; //주인공
    public GameObject momoE, cocoE, gariE;//악역
    public GameObject coin;//동전
    public GameObject warning;//경고창

    float playTime;
    public Text pt;

    public void start_btn()
    {
        SceneManager.LoadScene("BirdGame");
    }
    public void pickChiken_btn()
    {
        cNel.SetActive(true);
        conNel.SetActive(false);
        dNel.SetActive(false);
    }
    public void pickCondor_btn()
    {
        cNel.SetActive(false);
        conNel.SetActive(true);
        dNel.SetActive(false);
    }
    public void pickDragon_btn()
    {
        cNel.SetActive(false);
        conNel.SetActive(false);
        dNel.SetActive(true);
    }
    public void Replay_btn() //다시하기 버튼
    {
        Serial.instance.End();
        SceneManager.LoadScene("BirdGame");
        Data.Instance.FreshTree++;
        print(Data.Instance.FreshTree);
    }
    public void home_btn() //홈 버튼
    {
        Serial.instance.End();
        SceneManager.LoadScene("BirdMain");
        Data.Instance.FreshTree++;
        print(Data.Instance.FreshTree);
    }

    public void Replay_btn_12() //다시하기 버튼
    {
        Serial.instance.End();
        SceneManager.LoadScene("BirdGame1");
        Data.Instance.FreshTree++;
        print(Data.Instance.FreshTree);
    }
    public void momo_play_btn() //수리 게임
    {
        Time.timeScale = 1; //게임 정지 해제
        pickNel.SetActive(false);
        momo.SetActive(true);
        gariE.SetActive(true);
        coin.SetActive(true);
    }
    public void momo_play_btn3() //수리 게임
    {
        Time.timeScale = 1; //게임 정지 해제
        pickNel.SetActive(false);
        momo.SetActive(true);
        gariE.SetActive(true);
    }
    public void coco_play_btn() //코코 게임
    {
        pickNel.SetActive(false);
        coco.SetActive(true);
        momoE.SetActive(true);
        Time.timeScale = 1; //게임 정지 해제
        coin.SetActive(true);
    }
    public void coco_play_btn3() //코코 게임
    {
        pickNel.SetActive(false);
        coco.SetActive(true);
        momoE.SetActive(true);
        Time.timeScale = 1; //게임 정지 해제
    }
    public void gari_play_btn() //가리 게임
    {
        Time.timeScale = 1; //게임 정지 해제
        pickNel.SetActive(false);
        gari.SetActive(true);
        cocoE.SetActive(true);
        coin.SetActive(true);
    }
    public void gari_play_btn3() //가리 게임
    {
        Time.timeScale = 1; //게임 정지 해제
        pickNel.SetActive(false);
        gari.SetActive(true);
        cocoE.SetActive(true);
    }

    public void Continue_btn()
    {
        Time.timeScale = 1;
        menuNel.SetActive(false);
    }
    public void Menu_btn()
    {
        Time.timeScale = 0;
        menuNel.SetActive(true);
    }

    public void GameExit_btn()
    {
        SceneManager.LoadScene("Forest");
        //Application.Quit();
    }

    public void stop_btn()
    {
        Serial.instance.End();
        SceneManager.LoadScene("BirdMain"); //메인으로 가는 코드
        Data.Instance.DeadTree++;
        print(Data.Instance.DeadTree);

    }

}
