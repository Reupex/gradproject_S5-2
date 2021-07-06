using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    public GameObject MainSean;
    public GameObject Stage;
    public GameObject block;

    public AudioClip mainSound;
    private AudioSource audioSource;

    public int MainScor = 0;
    public int Stage_S;

    int init = 0;

    public static MainManager instance;

    void Awake()
    {
        instance = this;

        if (Data.Instance.GameID.Substring(1, 1) == "1") // 내/외전
        {
            init = (180 - Data.Instance.abduction + Data.Instance.adduction + 180) / 2;
            print(init);
        }
        else if (Data.Instance.GameID.Substring(1, 1) == "2")
        {
            init = (((-Data.Instance.internalR) + Data.Instance.externalR) / 2) + 360;
            print(init);
        }
        //Serial.instance.Init(init);
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        Stage.SetActive(false);

        this.audioSource = this.gameObject.GetComponent<AudioSource>();//효과음 컴포넌트
        this.audioSource.clip = this.mainSound;
        this.audioSource.loop = true;
        this.audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (MainScor == Stage_S)//메인 화면의 모든 블록이 파괴되면 메인화면이 다시 로딩되게 되어있습니다
        {
            Instantiate(block, GameObject.Find("block").transform.position, Quaternion.identity, GameObject.Find("block").transform);
            MainScor = 0;
        }
    }

    public void S_Start()//메인 화면의 게임 시작 버튼
    {
        MainSean.SetActive(false);
        Stage.SetActive(true);
    }

    public void E_Start()//쉬움 버튼
    {
        StageManager.instance.Stage = 1;//스테이지 매니저에 스테이지 번호를 넘깁니다.
        SceneManager.LoadScene("BrickGame");
    }

    public void N_Start()//보통 버튼
    {
        StageManager.instance.Stage = 2;
        SceneManager.LoadScene("BrickGame");
    }

    public void H_Start()//어려움 버튼
    {
        StageManager.instance.Stage = 3;
        SceneManager.LoadScene("BrickGame");
    }

    public void start_tuto()//튜토리얼 버튼
    {
        SceneManager.LoadScene("Tuto");
    }

    public void gmaeExit()//게임 종료버튼
    {
        Destroy(GameObject.Find("StageManager"));
        SceneManager.LoadScene("Forest");
    }
}
