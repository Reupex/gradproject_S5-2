using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class tutorialBtn : MonoBehaviour
{
    public GameObject startNel, enemy, exMomo, exCoin, coin;
    public GameObject T1, T2, T3, T4, T5;
    public GameObject exNel1, exNel2, exNel3, exNel4, exNel5, exNel5_1, exNel5_2, exNel6, exNel7, exNel8, exNel9, exNel10;

    void Start()
    {
        print(gameObject.name + "시작");
        Time.timeScale = 0;
        T1.SetActive(false);
        T2.SetActive(false);
        T3.SetActive(false);
        T4.SetActive(false);
        T5.SetActive(false);
        //exNel1.SetActive(false);
        exNel2.SetActive(false);
        exNel3.SetActive(false);
        exNel4.SetActive(false);
        exNel5.SetActive(false);
        exNel5_1.SetActive(false);
        exNel5_2.SetActive(false);
        exNel6.SetActive(false);
        exNel7.SetActive(false);
        exNel8.SetActive(false);
        exNel9.SetActive(false);
        exNel10.SetActive(false);
        startNel.SetActive(true);
    }

    public void TuStart_btn()
    {
        T1.SetActive(true);
        startNel.SetActive(false);
    }

    public void Tu1_btn()
    {
        T1.SetActive(false);
        T2.SetActive(true);
        exNel1.SetActive(true);
    }

    public void Tu2_btn()
    {
        T3.SetActive(true);
        exNel1.SetActive(false);
        exNel2.SetActive(true);
    }

    public void Ex2_btn()
    {
        exNel3.SetActive(true);
        exNel2.SetActive(false);
    }

    public void Tu3_btn()
    {
        exNel3.SetActive(false);
        T2.SetActive(false);
        T3.SetActive(false);
        T4.SetActive(true);
        exNel4.SetActive(true);
    }

    public void Ex4_btn()
    {
        exNel4.SetActive(false);
        exNel5.SetActive(true);
    }

    public void Ex5_btn()
    {
        exNel5.SetActive(false);
        exNel5_1.SetActive(true);
    }

    public void Ex5_1btn()
    {
        exNel5_1.SetActive(false);
        exNel5_2.SetActive(true);
    }

    public void Ex5_2btn()
    {
        exNel5_2.SetActive(false);
        exNel6.SetActive(true);
    }

    public void Tu4_btn() //메뉴 버튼
    {
        T5.SetActive(true);
        exNel7.SetActive(true);
        exNel6.SetActive(false);
    }

    public void Ex7_btn()
    {
        exNel7.SetActive(false);
        exNel8.SetActive(true);
    }

    public void Ex8_btn()
    {
        exNel8.SetActive(false);
        exNel9.SetActive(true);
    }

    public void Ex9_btn()
    {
        exNel9.SetActive(false);
        exNel10.SetActive(true);
    }

    public void Ex10_btn()
    {
        exNel10.SetActive(false);
        T5.SetActive(false);
        Time.timeScale = 1;
        //momo.SetActive(true);
        enemy.SetActive(true);
        //exMomo.SetActive(false);
        exCoin.SetActive(false);
        coin.SetActive(true);
        
    }

    public void excercise_btn() //연습하기 버튼
    {
        switch(Data.Instance.GameID.Substring(0,2))
        {
            case "11":
            case "12":
            case "41":
            case "42":
                SceneManager.LoadScene("BirdGame1");
                break;
            case "13":
            case "43":
                SceneManager.LoadScene("BirdGame");
                break;
            case "23":
                SceneManager.LoadScene("BirdGame3");
                break;
            case "31":
                SceneManager.LoadScene("BirdGame2");
                break;
        }
        
    }
    public void home_btn() //홈 버튼
    {
        SceneManager.LoadScene("BirdMain");
    }

    public void GameExit_btn()
    {
        SceneManager.LoadScene("Forest");
        //Application.Quit();
    }
}
