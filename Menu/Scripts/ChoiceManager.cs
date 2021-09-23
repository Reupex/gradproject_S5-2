using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ChoiceManager : MonoBehaviour
{
    public GameObject background; // 배경 panel
    public GameObject nextButton; // [>]
    public GameObject prevButton; // [<]
    public GameObject Contents;
    public GameObject[] step = new GameObject[4]; // GameObject 'StepN'
    private Vector3 nextPosition; // 배경(panel) 움직이는 위치

    Color basicColor; // 버튼 기본 색상
    Color selectImage; // 
    Color selectText;
    Color selectText2;
    int pageIndex = 0;
    public int maxPage = 0;
    string[] choice = new string[3]; // 1단계~3단계 선택한 것 저장하는 배열
    string ContentID; // 최종 선택된 콘텐츠 아이디( = choice[0] + choice[1] + choice[2])
    string NowScene;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        // 색 설정
        ColorUtility.TryParseHtmlString("#946CABFF", out selectText);
        ColorUtility.TryParseHtmlString("#2C5087FF", out selectText2);
        ColorUtility.TryParseHtmlString("#FFFFFFFF", out basicColor);
        ColorUtility.TryParseHtmlString("#FFFFFF50", out selectImage);

        NowScene = SceneManager.GetActiveScene().name;
        // 배경 초기화
        if ( NowScene == "Choice")
        {
            background.GetComponent<RectTransform>().anchoredPosition = new Vector3(2880, 0, 0);
            nextPosition = background.GetComponent<RectTransform>().anchoredPosition;
            pageIndex = 0;
        }
        else
        {
            background.GetComponent<RectTransform>().anchoredPosition = new Vector3(1920, 0, 0);
            nextPosition = background.GetComponent<RectTransform>().anchoredPosition;
            pageIndex = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //배경 움직임
        background.GetComponent<RectTransform>().anchoredPosition
            = Vector3.Lerp(background.GetComponent<RectTransform>().anchoredPosition, nextPosition, Time.deltaTime * 5f);
    }


    public void ChangeColor() // 선택한 버튼 색상 변경 함수(각 단계 버튼 onclick())
    {
        GameObject button = EventSystem.current.currentSelectedGameObject; // 선택한 버튼(GameObject)
        GameObject Parent = button.transform.parent.gameObject; // 선택한 버튼의 부모 게임오브젝트(StepN)
        for (int i = 0; i < Parent.transform.childCount; i++)
        {
            Parent.transform.GetChild(i).GetComponent<Image>().color = basicColor; // 버튼 색상 기본값으로 초기화
            for (int j = 0; j < Parent.transform.GetChild(i).childCount; j++)
            {
                if (j != 2 && pageIndex < 2)
                {
                    Parent.transform.GetChild(i).GetChild(j).GetComponent<Text>().color = selectText;
                }
                else if (j != 2 && pageIndex == 2)
                {
                    print(Parent.transform.GetChild(i).GetChild(j));
                    Parent.transform.GetChild(i).GetChild(j).GetComponent<Text>().color = selectText2;
                }
            }
        }
        for (int i = 0; i < button.transform.childCount; i++)
        {
            if (i == 2)
            {
                continue;
            }
            button.transform.GetChild(i).GetComponent<Text>().color = basicColor;
        }

        button.GetComponent<Image>().color = selectImage;
        if (pageIndex > maxPage)
            maxPage = pageIndex;
        nextButton.SetActive(true);

        choice[pageIndex] = button.name; // step 배열에 선택된 번호 저장(선택 콘텐츠ID 얻기 위해)
        print(choice[pageIndex]);
    }

    public void Next() // [>] 버튼 onclick()
    {
        nextPosition = nextPosition - new Vector3(1920, 0, 0);
        prevButton.SetActive(true);
        step[pageIndex].SetActive(false);
        pageIndex++;
        print(pageIndex);
        if (pageIndex > maxPage)
        {
            nextButton.SetActive(false);
        }
        if (pageIndex == 2)
        {
            ContentsList();
        }
        if (pageIndex == 3)
        {
            SettingImage();
        }
        print(pageIndex);
        step[pageIndex-1].SetActive(true);
        GameObject.Find("StepO").transform.Find(pageIndex.ToString()).transform.GetChild(0).gameObject.SetActive(false); // 전 단계 index 꺼주기
        GameObject.Find("StepO").transform.Find((pageIndex + 1).ToString()).transform.GetChild(0).gameObject.SetActive(true); // 다음 단계 index 켜주기
    }

    public void Prev() // [<] 버튼 onclick()
    {
        nextPosition = nextPosition + new Vector3(1920, 0, 0);
        nextButton.SetActive(true);
        //step[pageIndex].SetActive(false);
        pageIndex--;
        step[pageIndex].SetActive(true);
        if (pageIndex == 0)
        {
            prevButton.SetActive(false);
        }
        GameObject.Find("StepO").transform.Find((pageIndex + 1).ToString()).transform.GetChild(0).gameObject.SetActive(true); // 전 단계 index 켜주기
        GameObject.Find("StepO").transform.Find((pageIndex + 2).ToString()).transform.GetChild(0).gameObject.SetActive(false); // 다음 단계 index 꺼주기
    }

    void ContentsList() // 선택에 맞는 콘텐츠가 정렬되도록
    {
        int StartIndex = 0;
        int EndIndex = 0;

        for (int i = 0; i < Contents.transform.childCount; i++) // 리스트 초기화
        {
            Contents.transform.GetChild(i).gameObject.SetActive(false);
        }
        print(choice[0] + choice[1]);
        switch (choice[0] + choice[1])
        {
            case "11": // 능동 + 내/외전
            case "12": // 능동 + 내/외회전 
            case "41": // 저항 + 내/외전
            case "42": // 저항 + 내/외회전 
                StartIndex = 0;
                EndIndex = Contents.transform.childCount;
                break;
            case "13": // 능동 + 굴곡/신전
            case "43": // 저항 + 굴곡/신전
                StartIndex = 2;
                EndIndex = Contents.transform.childCount;
                break;
            case "21": // 수동 + 내/외전
            case "22": // 수동 + 내/외회전
                StartIndex = 1;
                EndIndex = 5;
                break;
            case "23": // 수동 + 굴곡/신전
                StartIndex = 2;
                EndIndex = 5;
                break;
            case "31": // 수직
                StartIndex = 0;
                EndIndex = 2;
                break;
            case "32": // 수평
                StartIndex = 2;
                EndIndex = 4;
                break;
        }
        for (int i = StartIndex; i < EndIndex; i++)
        {
            Contents.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    void SettingImage()
    {
        
        string pose = "";
        GameObject img;
        if (NowScene == "Choice")
        {
            for (int i = 1; i < 5; i++)
            {
                step[3].transform.Find(i.ToString()).gameObject.SetActive(false);
            }
            pose = choice[1];
            img = step[3];
        }
        else
        {
            for (int i = 1; i < 3; i++)
            {
                step[2].transform.Find(i.ToString()).gameObject.SetActive(false);
            }
            print(choice[1]);
            pose = choice[1].Substring(1, 1);
            img = step[2];
        }
        print(img);

        switch (pose)
        {
            case "1":
                img.transform.Find("1").gameObject.SetActive(true);
                break;
            case "2":
                img.transform.Find("2").gameObject.SetActive(true);
                break;
            case "3":
                if (choice[2] != "bird")
                    img.transform.Find("3").gameObject.SetActive(true);
                else
                    img.transform.Find("4").gameObject.SetActive(true);
                break;
        }
    }

    public void StartContents() // 콘텐츠 [시작하기] 버튼 onclick()
    {
        ContentID = choice[0] + choice[1] + choice[2];
        Data.Instance.GameID = ContentID;
        string order = "SELECT * FROM Measurement WHERE userID = '" + Data.Instance.UserID + "'";
        DBManager.DataBaseRead(order);
        bool isFirst = true;
        while (DBManager.dataReader.Read())
        {
            isFirst = false;
        }
        DBManager.DBClose();

        if (isFirst)
        {
            print("처음임");
            GameObject.Find("Panel").transform.Find("First").gameObject.SetActive(true); // 측정먼저해라 panel 띄워띄워
        }
        else
        {
            GameObject.Find("Panel").transform.Find("Start??").gameObject.SetActive(true);
        }
    }
    public void GoMeasurement()
    {
        SceneManager.LoadScene("Measurement"); // 선택한 콘텐츠에 맞게 씬 로드
    }

    public void GoMain()
    {
        Data.Instance.MaxData();
        SceneManager.LoadScene(choice[2] + "Main"); // 선택한 콘텐츠에 맞게 씬 로드
    }

    public void GoMain2()
    {
        ContentID = choice[1] + choice[2];
        Data.Instance.GameID = ContentID;
        Data.Instance.MaxData();
        SceneManager.LoadScene(choice[2] + "Main"); // 선택한 콘텐츠에 맞게 씬 로드
    }
    public void ForestButton() // [숲으로 가기] 버튼 onclick()
    {
        SceneManager.LoadScene("Forest");
    }
}