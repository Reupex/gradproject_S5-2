using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ArrowSceneManager : MonoBehaviour
{
    public int time;
    public bool pass;
    public GameObject Main_Monster;
    public bool isGame = false;

    #region Singleton
    private static ArrowSceneManager _Instance;    // 싱글톤 패턴을 사용하기 위한 인스턴스 변수, static으로 선언하여 어디서든 접근 가능

    // 인스턴스에 접근하기 위한 프로퍼티
    public static ArrowSceneManager Instance
    {
        get { return _Instance; }   // SceneChangeManager 인스턴스 변수를 리턴
    }

    // 인스턴스 변수 초기화
    void Awake()
    {
        _Instance = GetComponent<ArrowSceneManager>();    // _sceneManager에 SceneChangeManager의 컴포넌트(자기 자신)에 대한 참조를 얻음
    }
    #endregion

    void Start()
    {
        time = 0;
        if (SceneManager.GetActiveScene().name == "ArrowGame")
            isGame = true;
    }

    public void RePlay_inGame() // 다시시작 버튼을 눌렀을때
    {
        Time.timeScale = 1f;
        ArrowSound.Instance.Btn_Click();

        if (Data.Instance.GameID.Substring(0, 2) == "32") // 수평일 때
            SceneManager.LoadScene("ArrowGame_수평");
        else
            SceneManager.LoadScene("ArrowGame");
    }

    public void RePlay_inTuto() // 다시시작 버튼을 눌렀을때
    {
        Time.timeScale = 1f;
        ArrowSound.Instance.Btn_Click();
        SceneManager.LoadScene("ArrowTuto");
    }

    public void Home_DeadTree() // 중단버튼
    {
        Time.timeScale = 1f;
        ArrowSound.Instance.Btn_Click();

        // 중단나무 생성 코드 - [중단 버튼] 클릭이벤트 함수에 아래 코드 추가
        Serial.instance.End();
        SceneManager.LoadScene("ArrowMain"); // 메인으로 가는 코드
        // 중단나무 생성을 위해 데이터 저장
        Data.Instance.DeadTree++;
        print(Data.Instance.DeadTree);
    }

    public void Home_FreshTree() // 수행완료 후 홈으로가기
    {
        Time.timeScale = 1f;
        ArrowSound.Instance.Btn_Click();

        // 수행완료 나무 생성 코드 - [홈으로], [다시하기] 클릭이벤트 함수에 아래 코드 추가
        Serial.instance.End();
        SceneManager.LoadScene("ArrowMain"); // 메인으로 가는 코드
        // 수행완료 나무 생성을 위해 데이터 저장
        Data.Instance.FreshTree++;
        print(Data.Instance.FreshTree);
    }

    public void Tutorial() // 튜토리얼 버튼을 눌렀을때
    {
        ArrowSound.Instance.Btn_Click();
        SceneManager.LoadScene("ArrowTuto");
    }

    public void GameExit() // 종료하기 버튼을 눌렀을때
    {
        ArrowSound.Instance.Btn_Click();
        SceneManager.LoadScene("Forest");
    }

    public void Main_Monster_Animation()
    {
        Debug.Log("Main_Monster_Animation() 호출 완료");
        Animator my_animator = Main_Monster.gameObject.GetComponent<Animator>();

        switch(Random.Range(0, 2))
        {
            case 0:
                my_animator.SetTrigger("attack");
                break;

            case 1:
                my_animator.SetTrigger("attack02");
                break;
        }
    }
}