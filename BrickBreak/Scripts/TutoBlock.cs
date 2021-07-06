using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 튜토리얼용 블록 스크립트입니다.
 * 튜토리얼 매니저가 따로 생겨서 
 * 튜토리얼 매니저에서 색을 받기 위해 만들었습니다
 */

public class TutoBlock : MonoBehaviour
{
    Vector3 vec;//나머지는 그냥 블록이랑 똑같습니다.
    public float blockST = 14.0f;
    public float color_f = 1;
    float[,] b_color = new float[,] { { 36 / 255f, 36 / 255f, 224 / 255f }, //파랑
                                        { 0f, 128 / 255f, 0f },             //초록
                                        { 254 / 255f, 73 / 255f, 2 / 255f },//주황
                                        { 139 / 255f, 0f, 1f},              //보라
                                        { 1f, 109 / 255f, 101 / 255f}       //빨강
    };    //

    public static TutoBlock instance;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        vec.x = 0f;
        vec.y = -1.5f;
        vec.z = 0f;

        //여기가 그냥 블록이랑 다른 부분. 튜토리얼 매니저에서 색을 받아 씁니다
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).GetComponent<Image>().color = new Color(b_color[TutoGameManager.instance.n, 0], b_color[TutoGameManager.instance.n, 1], b_color[TutoGameManager.instance.n, 2], color_f);
        }
        color_f -= 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        blockST -= Time.deltaTime;
        if (blockST < 0)
        {
            this.transform.Translate(vec);
            blockST = 14.0f;
            for (int i = 0; i < this.transform.childCount; i++) //여기도 달라요
            {
                this.transform.GetChild(i).GetComponent<Image>().color = new Color(b_color[TutoGameManager.instance.n, 0], b_color[TutoGameManager.instance.n, 1], b_color[TutoGameManager.instance.n, 2], color_f);
            }
            color_f -= 0.2f;
        }
        if (this.transform.childCount > 0)
        {
            //Debug.Log("색 없어짐");
            if (color_f < 0)
            {
                TutoGameManager.instance.GameOvQ = true;
            }
            else if (color_f > 0.99)
            {
                TutoGameManager.instance.F_Line();
                blockST = 14.0f;
            }
        }
    }
}

