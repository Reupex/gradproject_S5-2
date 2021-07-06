using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Data;
using Mono.Data.Sqlite; //db 사용
using UnityEngine.Networking; //네트워크 사용
using Mono.Data.SqliteClient;

using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
*  Stack에 들어있는 스크립트
*/
public class StackPassive : MonoBehaviour
{
    public AudioSource backgroundAudio, effectAudio;
    public AudioClip pause;
    public AudioClip com;

    public Slider slider;
    public GameObject background; // 배경 패널
    public Text BestScore; // 최고점수
    public Text scoreText;  //점수 text
    public Text resultTime;
    public Text resultscore;
    public Color32[] gameColors = new Color32[10]; //게임 컬러 10개
    public Material stackMaterial;  //stack material -> cubeMeterial
    public GameObject pausePanel, endPanel, circleGraph;     //게임 끝났을 때 패널

    private const float BOUND_SIZE = 3.5f;              //바운드 상한
    private const float STACK_MOVING_SPEED = 3.0f;      //스택 움직이는 속도
    private const float ERROR_MARGIN = 0.1f;            //

    private GameObject[] theStack;
    private Vector2 stackBound = new Vector2(BOUND_SIZE, BOUND_SIZE);

    private int stackIndex = 0;     //stack index
    private int scoreCount = 0;     //stack 점수
    private int combo = 0;          //콤보?

    private float tileTransition = 1f;
    private float tileSpeed = 2f;           //타일 움직이는 속도
    private float timer = 0f;
    private float xPosition = 0f;
    private float yPosition = 0f;
    public int maxScore;

    public float randNum = 0, cmpNum = 0;
    float sliderValue = 0;

    private Vector3 desiredPosition;
    private Vector3 lastTilePosition;
    private Vector3 panelNextPosition;

    private bool isMoveOnX = true;
    private bool isPause = false;
    private bool isDead = false;
    public bool check = false;
    public bool tuto = false;
    public bool even = false;

    public int rotNum = 0;
    string direction;
    string move;
    public string date;
    public string sql;
    public int angle = 0;
    float speedAngle;
    string Querry;
    float standardPoint = 0f;
    string userID;
    string gameID;
    
    private static StackPassive _instance;
    public static StackPassive instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = GetComponent<StackPassive>();
    }

    private void Start()
    {
        date = DBManager.SqlFormat(DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초"));
        userID = DBManager.SqlFormat(Data.Instance.UserID); 
        gameID = DBManager.SqlFormat(Data.Instance.GameID);

        Serial.instance.End();
        Serial.instance.Passive();

        slider.value = (slider.maxValue + slider.minValue) / 2;
        standardPoint = -7.0f + 14.0f / (slider.maxValue - slider.minValue) * (slider.value - slider.minValue);
        Time.timeScale = 1f;
        //현재 Stack 게임 오브젝트로 배열 초기화
        theStack = new GameObject[transform.childCount];        //Stack 가져오기
        for (int i = 0; i < transform.childCount; i++)
        {
            theStack[i] = transform.GetChild(i).gameObject; //i번째 게임오브젝트를 가져와 theStack[i]에 저장
            ColorMesh(theStack[i].GetComponent<MeshFilter>().mesh); //Cube[i]의 Mesh Filter을 가져와 ColorMesh 함수 적용
            panelNextPosition = background.GetComponent<RectTransform>().anchoredPosition;
        }

        stackIndex = transform.childCount - 1; //0~9번까지 있으므로 stackIndex = 9
        theStack[stackIndex].transform.localPosition = new Vector3(0, scoreCount + 5, 0);
        theStack[stackIndex].transform.tag = "cube";
        theStack[stackIndex].GetComponent<Rigidbody>().isKinematic = false;

        DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0} and gameID = {1} ORDER BY gameScore DESC ", userID, gameID));
        while (DBManager.dataReader.Read())
        {
            maxScore = DBManager.dataReader.GetInt32(8);
            break;
        }
        DBManager.DBClose();

        BestScore.text = "최고점수 : " + maxScore;
    }

    private void Update()
    {
        if (isDead) //게임이 끝나면 return
            return;
        timer += Time.deltaTime;
        if (check)
        {
            if (PlaceIt())
            {
                SpawnTile();
                scoreCount++;
                scoreText.text = scoreCount.ToString();
                panelNextPosition -= new Vector3(0, 200, 0);
                if (background.GetComponent<RectTransform>().anchoredPosition.y < -9800)
                {
                    background.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 9900);
                    panelNextPosition = new Vector2(0, 9800);
                }
                check = false;
            }
            else
            {
                EndGame();
            }
        }

        MoveTile();

        if (theStack[stackIndex].transform.localPosition.y < scoreCount - 2)
        {
            EndGame();
        }

        background.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(background.GetComponent<RectTransform>().anchoredPosition, panelNextPosition, Time.deltaTime * 0.5f);

        transform.position = Vector3.Lerp(transform.position, desiredPosition, STACK_MOVING_SPEED * Time.deltaTime);

        if (scoreCount > maxScore)
        {
            BestScore.text = "최고점수 : " + scoreCount;
        }
    }

    //떨어져나가는 큐브 만들기
    private void CreateRubble(Vector3 pos, Vector3 sca)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localPosition = pos;       //부모 transform의 상대적인 transform의 위치를 나타냄
        go.transform.localScale = sca;          //게임오브젝트의 상대적인 크기 예를들어 부모의 localscale->(2,2,2) , 자식의 localscale->(1,1,1) => 자식의 절대적인 크기 (2,2,2)
        go.AddComponent<Rigidbody>();           //Rigidbody 컴포넌트 추가

        go.GetComponent<MeshRenderer>().material = stackMaterial;       //meshrenderer의 material을 stackMaterial로 설정
        ColorMesh(go.GetComponent<MeshFilter>().mesh);              //color 설정
    }

    //타일 움직임
    private void MoveTile()
    {
        tileTransition += Time.deltaTime * tileSpeed;
        //position이 randnum에서 slider value가 더해지는 것
        xPosition = (-7.0f + 14.0f / (slider.maxValue - slider.minValue) * (slider.value - slider.minValue));
        
        //yPosition = theStack[stackIndex].transform.localPosition.y;
        //theStack[stackIndex].transform.localPosition = new Vector3(-xPosition, yPosition, 0);
        for (int i = 0; i < 10; i++)
        {
            if (i != stackIndex)
            {
                theStack[i].transform.localPosition = new Vector3(-xPosition, theStack[i].transform.localPosition.y, 0);
            }
        }
        lastTilePosition.x = -xPosition;
    }

    //스택 생성
    private void SpawnTile()
    {
        rotNum++;
        if (rotNum == 4 )
        {
            EndGame();
        }
        do{
            randNum = UnityEngine.Random.Range(-7.0f, 7.0f);
        } while (cmpNum - randNum < 1 && cmpNum - randNum > -1);
        cmpNum = randNum;
        print(randNum);
        angle = (int)(Mathf.Abs(slider.maxValue - slider.minValue)*Mathf.Abs(randNum + xPosition) / 14.0);
        print("randNum : "+randNum + " xPosition : " + xPosition+ " angle : "+ angle);
        if (randNum < -xPosition)   //오른쪽에 생성 : 시계방향
        {
            slider.value += angle;
            Serial.instance.PaMotor("0" + Serial.instance.WriteAngle(angle) + "2000");
            print("0" + Serial.instance.WriteAngle(angle) + "2000");
        }
        else if(randNum > -xPosition){  //왼쪽에 생성 : 반시계방향
            slider.value -= angle;
            Serial.instance.PaMotor("1" + Serial.instance.WriteAngle(angle) + "2000");
            print("1" + Serial.instance.WriteAngle(angle) + "2000");
        }

        //slider.value = randNum;
        lastTilePosition = theStack[stackIndex].transform.localPosition;
        stackIndex--;   //새로 생성될 stackindex
        if (stackIndex < 0)
        {   //스택이 1번 -> 10번
            stackIndex = transform.childCount - 1;
        }

        desiredPosition = (Vector3.down) * (scoreCount);    //Vector3.down = Vector3(0,-1,0)
                                                            //새로 생성된 스택의 위치와 스케일 정해주기
        theStack[stackIndex].transform.localScale = new Vector3(stackBound.x, 1, stackBound.y);
        //스택의 메시를 가져와 색을 바꾼다.
        ColorMesh(theStack[stackIndex].GetComponent<MeshFilter>().mesh);
        theStack[stackIndex].transform.localPosition = new Vector3(randNum, scoreCount + 6, 0);
        theStack[stackIndex].transform.tag = "cube";
        theStack[stackIndex].GetComponent<Rigidbody>().isKinematic = false;
    }

    //버튼 사운드 이펙트
    public void ButtonEffect()
    {
        effectAudio.PlayOneShot(pause);
    }

    //스택이 쌓임
    private bool PlaceIt()
    {
        Transform t = theStack[stackIndex].transform; //moveTile
        float deltaX = lastTilePosition.x - t.position.x;   //잘리는 부분의 x
        if (Mathf.Abs(deltaX) > ERROR_MARGIN)   //Error_margin : 0.1f
        {
            // cut the tile
            combo = 0;
            stackBound.x -= Mathf.Abs(deltaX);      //stackBound.x = 잘리고 남은 stack의 scale
            if (stackBound.x <= 0)  //겹치는 게 없으면 게임 종료
                return false;

            float middle = lastTilePosition.x + t.localPosition.x / 2;
            t.localScale = new Vector3(stackBound.x, 1, stackBound.y);  //남겨진 부분의 scale
                                                                        //Debug.Log(t.position.x);
            CreateRubble(
                //rubble position
                new Vector3((t.position.x > 0)              //(+)면 왼쪽에서 생성, (-)면 오른쪽에서 생성 
                    ? t.position.x + (t.localScale.x / 2)
                    : t.position.x - (t.localScale.x / 2)
                    , t.position.y, t.position.z),
                new Vector3(Mathf.Abs(deltaX), 1, t.localScale.z)); //rubble scale
            t.localPosition = new Vector3(middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);
        }
        else
        {
            combo++;
            //콤보면 stack의 크기가 커짐
            if (combo > 2)
            {
                effectAudio.PlayOneShot(com);
                if (stackBound.x <= 3.25f)  //상한값지정
                {
                    stackBound.x += 0.25f;
                }
                float middle = lastTilePosition.x + t.localPosition.x / 2;
                t.localScale = new Vector3(stackBound.x, 1, stackBound.y);
                t.localPosition = new Vector3(middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);
            }
            else
            {
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
            }

        }
        return true;
    }

    //게임 일시정지
    public void pauseGame()
    {
        direction = "0";
        move = direction + Serial.instance.WriteAngle(180) + "1000";
        Serial.instance.Passive();
        Serial.instance.PaMotor(move);

        effectAudio.PlayOneShot(pause);
        if (!isPause)
        {
            Time.timeScale = 0f;
            for (int i = 0; i < theStack.Length; i++)
            {
                theStack[i].GetComponent<MeshRenderer>().enabled = false;
            }
            pausePanel.SetActive(true);
            isPause = true;
        }
        else
        {
            Time.timeScale = 1f;
            for (int i = 0; i < theStack.Length; i++)
            {
                theStack[i].GetComponent<MeshRenderer>().enabled = true;
            }
            pausePanel.SetActive(false);
            isPause = false;
        }
    }

    //게임 종료
    private void EndGame()
    {
        Data.Instance.FreshTree++;
        print(Data.Instance.FreshTree);

        if (PlayerPrefs.GetInt("score") < scoreCount)
        {
            PlayerPrefs.SetInt("score", scoreCount);
        }
        isDead = true;
        endPanel.SetActive(true);
        //theStack[stackIndex].AddComponent<Rigidbody>();
        for (int i = 0; i < theStack.Length; i++)
        {
            theStack[i].GetComponent<MeshRenderer>().enabled = false;
        }
        resultTime.text = timer.ToString("N2");
        resultscore.text = scoreCount.ToString();
        //Device.instance.ResultCircle(circleGraph); //그래프 설정
        Serial.instance.End();
        Querry = string.Format("Insert into Game VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})", date, userID, gameID, "'홀번호'", "'x'", Math.Abs(slider.minValue), slider.maxValue, timer, scoreCount);
        DBManager.DatabaseSQLAdd(Querry);
    }

    //씬 전환
    public void OnButtonClick(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    //색 변환
    private Color32 Lerp11(Color32 a, Color32 b, Color32 c, Color32 d, Color32 e, Color32 f, Color32 g, Color32 h, Color32 i, Color32 j, float t)
    {
        if (t > 1f)
            t = 0f;
        if (t < 0.1f)
            return Color.Lerp(a, b, t / 0.1f);
        else if (t < 0.2f)
            return Color.Lerp(b, c, (t - 0.1f) / 0.1f);
        else if (t < 0.3f)
            return Color.Lerp(c, d, (t - 0.2f) / 0.1f);
        else if (t < 0.4f)
            return Color.Lerp(d, e, (t - 0.3f) / 0.1f);
        else if (t < 0.5f)
            return Color.Lerp(e, f, (t - 0.4f) / 0.1f);
        else if (t < 0.6f)
            return Color.Lerp(f, g, (t - 0.5f) / 0.1f);
        else if (t < 0.7f)
            return Color.Lerp(g, h, (t - 0.6f) / 0.1f);
        else if (t < 0.8f)
            return Color.Lerp(h, i, (t - 0.7f) / 0.1f);
        else if (t < 0.9f)
            return Color.Lerp(i, j, (t - 0.8f) / 0.1f);
        return Color.Lerp(j, a, (t - 0.9f) / 0.1f);
    }

    //생성된 큐브의 메시를 가져와 색을 바꿈
    private void ColorMesh(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Color32[] colors = new Color32[vertices.Length];

        float f = Mathf.Abs(Mathf.Sin(scoreCount * 0.01f)); //scoreCount는 점수로 큐브가 생성될 때마다 1씩 올라감
        for (int i = 0; i < vertices.Length; i++)
        {
            //color는 배경 색에 맞게 다시 조정
            //진파 -> 파 -> 연파 -> 회 -> 연파 -> 파 -> 진파 ~~
            colors[i] = Lerp11(gameColors[0], gameColors[1], gameColors[2], gameColors[3], gameColors[4], gameColors[5], gameColors[6], gameColors[7], gameColors[8], gameColors[9], f);
        }

        mesh.colors32 = colors; //색 입힘
    }

    public void Check()
    {
        check = true;
    }

}
