﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneChange_p : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 메인씬 -> 게임씬으로 전환
    public void GameStart()
    {
        ClimbGameManager_p.isTutorial = false;      
        SceneManager.LoadScene("ClimbGame_p");
    }

    public void TutorialStart()
    {
        ClimbGameManager_p.isTutorial = true;
        SceneManager.LoadScene("ClimbGame_p");
        
    }

    // 메인으로(게임중단)
    public void GameStop()
    {
        //Serial.instance.End();      
        SceneManager.LoadScene("ClimbMain_p"); //메인으로 가는 코드
        Destroy(ClimbGameManager_p.instance.gameObject);
        Destroy(SoundManager.instance.gameObject);
        Debug.Log("썩은 나무 생성");
        //Data.Instance.DeadTree++;
        //print(Data.Instance.DeadTree);
    }
        
    // 메인으로(홈으로, 다시하기)
    public void BackToMain()
    {
        //Serial.instance.End();       
        SceneManager.LoadScene("ClimbMain_p"); //메인으로 가는 코드 or 다시하기 씬 로드
        Destroy(ClimbGameManager_p.instance.gameObject);
        Destroy(SoundManager.instance.gameObject);
        if (ClimbGameManager_p.instance._success == 1)
        {
            Debug.Log("싱싱한 나무 생성");
            //Data.Instance.FreshTree++;
            //print(Data.Instance.FreshTree);
        }
    }

    // Pause -> 다시하기 선택 시 or 게임 클리어 후 -> 다시하기 선택 시
    public void Retry()
    {
        ClimbGameManager_p.isTutorial = false;
        SceneManager.LoadScene("ClimbGame_p");
        Destroy(ClimbGameManager_p.instance.gameObject);
        Destroy(SoundManager.instance.gameObject);
        if (ClimbGameManager_p.instance._success == 1)
        {
            Debug.Log("싱싱한 나무 생성");
            //Data.Instance.FreshTree++;
            //print(Data.Instance.FreshTree);
        }
    }

    // 메인으로(튜토리얼)
    public void TutorialEnd()
    {
        //Serial.instance.End();       
        Destroy(ClimbGameManager_p.instance.gameObject);
        Destroy(SoundManager.instance.gameObject);
        SceneManager.LoadScene("ClimbMain_p"); //메인으로 가는 코드 or 다시하기 씬 로드      
    }

    public void QuitGame()
    {
        //SceneManager.LoadScene("Forest");
    }
}
