using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISelect : MonoBehaviour
{
    public GameObject background;
    public GameObject nextButton;
    public GameObject prevButton;
    public GameObject[] select = new GameObject[4];
    private Vector3 nextPosition;

    Color basicColor;
    Color selectImage;
    Color selectText;
    Color selectText2;
    int pageIndex = 0;
    public int maxPage = 0;

    // Start is called before the first frame update
    void Start()
    {
        // 색 설정
        ColorUtility.TryParseHtmlString("#946CABFF", out selectText);
        ColorUtility.TryParseHtmlString("#2C5087FF", out selectText2);
        ColorUtility.TryParseHtmlString("#FFFFFFFF", out basicColor);
        ColorUtility.TryParseHtmlString("#FFFFFF50", out selectImage);
        // 배경 초기화
        background.GetComponent<RectTransform>().anchoredPosition = new Vector3(2862, 0, 0);
        nextPosition = background.GetComponent<RectTransform>().anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
        //배경 움직임
        background.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(background.GetComponent<RectTransform>().anchoredPosition, nextPosition, Time.deltaTime*5f);
    }


    public void changeColor()
    {
        GameObject button = EventSystem.current.currentSelectedGameObject;
        GameObject Parent = button.transform.parent.gameObject;
        for (int i = 0; i < Parent.transform.childCount; i++)
        {
            Parent.transform.GetChild(i).tag = "unselect";
            Parent.transform.GetChild(i).GetComponent<Image>().color = basicColor;
            for (int j = 0; j < Parent.transform.GetChild(i).childCount; j++)
            {
                if(j != 2)
                {
                    if (pageIndex < 2)
                    {
                        Parent.transform.GetChild(i).GetChild(j).GetComponent<Text>().color = selectText;
                    }
                    else
                    {
                        print(Parent.transform.GetChild(i).GetChild(j));
                        Parent.transform.GetChild(i).GetChild(j).GetComponent<Text>().color = selectText2;
                    }
                }
               
                
            }
        }
        for(int i =0; i<button.transform.childCount; i++)
        {
            if (i== 2)
            {
                continue;
            }
            button.transform.GetChild(i).GetComponent<Text>().color = basicColor;
            
        }
        button.tag = "select";
        button.GetComponent<Image>().color = selectImage;
        if (pageIndex > maxPage)
            maxPage = pageIndex;
        nextButton.SetActive(true);
    }

    public void Next()
    {
        nextPosition = nextPosition - new Vector3(1920,0,0);
        prevButton.SetActive(true);
        select[pageIndex].SetActive(false);
        pageIndex++;
        if (pageIndex > maxPage)
        {
            nextButton.SetActive(false);
        }
        select[pageIndex].SetActive(true);
    }
    
    public void Prev()
    {
        nextPosition = nextPosition + new Vector3(1920, 0, 0);
        nextButton.SetActive(true);
        select[pageIndex].SetActive(false);
        pageIndex--;
        select[pageIndex].SetActive(true);
        if (pageIndex == 0)
        {
            prevButton.SetActive(false);
        }
    }
}
