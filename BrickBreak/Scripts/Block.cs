using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour//블록 안에 들어가는 스크립트 입니다
{//블럭이 생성될때 이 스크립트도 계속 새로 생성되는거 같으니 편집할때 주의
    Vector3 vec;
    public float blockST = 14.0f;//블록 생성되는 시간 (14초 지나면 블록이 생성됩니다)
    public float color_f = 1;//블록 투명도
    float[,] b_color = new float[,] { { 36 / 255f, 36 / 255f, 224 / 255f }, //파랑
                                        { 0f, 128 / 255f, 0f },             //초록
                                        { 254 / 255f, 73 / 255f, 2 / 255f },//주황
                                        { 139 / 255f, 0f, 1f},              //보라
                                        { 1f, 109 / 255f, 101 / 255f}       //빨강
    };    //

    public static Block instance;

    void Awake()
    {
         Block.instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        vec.x = 0f;//블록이 이동하는 위치입니다
        vec.y = -1.5f;//y축(아래)로만 이동합니다
        vec.z = 0f;


        for (int i = 0; i < this.transform.childCount; i++)
        {//블록 색 바꿔주는 for문입니다. 블록이 여러개라 for문을 사용해서 하나하나 바꿔줍니다.
            this.transform.GetChild(i).GetComponent<Image>().color = new Color(b_color[BrickGame.instance.n, 0], b_color[BrickGame.instance.n, 1], b_color[BrickGame.instance.n, 2], color_f);
        }
        color_f -= 0.2f;//블록 색 점점 연해집니다. 투명도가 점점 낮아져 투명해지는 겁니다.
    }

    // Update is called once per frame
    void Update()
    {
        blockST -= Time.deltaTime;//14초에서 현재 지난 시간을 계속 빼줍니다.
        if (blockST < 0)//그리고 14초가 지나면
        {
            this.transform.Translate(vec);//아래로 이동
            blockST = 14.0f;//시간 초기화
            for (int i = 0; i < this.transform.childCount; i++)
            {//블록색 바꿔줍니다.
                this.transform.GetChild(i).GetComponent<Image>().color = new Color(b_color[BrickGame.instance.n, 0], b_color[BrickGame.instance.n, 1], b_color[BrickGame.instance.n, 2], color_f);
            }
            color_f -= 0.2f;//스타트에 있는거 지우고 이걸 for문 앞으로 옮겨서 이쁘게 만들고 싶었는데 뭔가 잘 안되더라구요
        }
        if (this.transform.childCount > 0)//모든 블록이 파괴되지 않고
        {
            //Debug.Log("색 없어짐");
            if(color_f < 0 && BrickGame.instance.GameOvQ)//색이 없어지면
            {
                BrickGame.instance.GameOvQ = true;//게임 오버로 했는데 게임하면서 거의 못본거 같아요
                BrickGame.instance.ifOver();
            }
        }
        else//모든 블록이 파괴 되었을때
        {//이것도 만들긴 만들었는데 14초안에 모든 블록 파괴되는걸 본적이 없어요
            if (color_f > 0.99)//투명도가 1인 블록이 있는 위치는 맨 윗칸입니다.
            {
                BrickGame.instance.F_Line();//블록 새로 생성
                blockST = 14.0f;//시간 초기화
            }
        }
    }
}
