using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 굴곡, 내전, 외전 3파트에 현재 관절가동범위 부분에 들어가있음
// 주내용 : 360도 원 영역 채우기 및, 텍스트 위치잡기

public class UIManager : MonoBehaviour
{
    //  (굴곡/신전, 내전 / 외전, 내회전/외회전) - 그래프 제외

    #region Singleton  
    // (굴곡/신전, 내전 / 외전, 내회전/외회전)에서 현재 관절가동범위 데이터 값에 따라 원 그래프 이동시킬때 필요한 것들
    public GameObject Range_of_motion, Ordinary_person1, Ordinary_person2, User1, User2; // 순서대로 일반인_왼쪽, 일반인_오른쪽, 환자_왼쪽, 환자_오른쪽
    public Text Range_of_motion_text, Ordinary_person1_text, Ordinary_person2_text, User1_text, User2_text; // 순서대로 일반인_왼쪽, 일반인_오른쪽, 환자_왼쪽, 환자_오른쪽
    public List<GameObject> obj = new List<GameObject>(4);  // Range_of_motion, Ordinary_person1, Ordinary_person2, User1, User2를 List에 넣음 -> 반복문으로 처리하기 위해서
    public List<Text> obj_text = new List<Text>(4); // Range_of_motion_text, Ordinary_person1_text, Ordinary_person2_text, User1_text, User2_text를 List에 넣음 -> 반복문으로 처리하기 위해서
    public List<float> rotation_value; // 순서대로 일반인_Max, 일반인_Min, 환자_Max, 환자_Min
    public int type; // 굴곡=1, 내전=2, 내회전=3 구분을 위함
    public GameObject Circle_Ordinary_person, Circle_User; // 원 영역 설정을 위한 변수

    string userID, gameID;
    // (굴곡/신전, 내전 / 외전, 내회전/외회전) - 콘텐츠 수행 횟수
    public List<Text> contents = new List<Text>(); // 순서대로 콘텐츠 몇개, 몇 번, 몇 점
    public List<string> t = new List<string>();

    // (굴곡/신전, 내전 / 외전, 내회전/외회전) - 현재 관절가동범위에서 환자의 값
    public List<float> value = new List<float>(2);

    // 통합 운동 수행 시간
    public Text integrated_time;

    // 현재 ROM
    public int x = 0, y = 0;

    private static UIManager _Instance;          // 싱글톤 패턴을 사용하기 위한 인스턴스 변수, static 선언으로 어디서든 참조가 가능함
    public static UIManager Instance                    // 객체에 접근하기 위한 속성으로 내부에 get set을 사용한다.
    {
        get { return _Instance; }                         
    }

    void Awake() // Start()보다 먼저 실행
    {
        _Instance = this;
    }
    #endregion

    void Start()
    {
        userID = SqlFormat(Data.Instance.UserID); 
        gameID = SqlFormat(Data.Instance.GameID); 

        #region (굴곡/신전, 내전 / 외전, 내회전/외회전)에서 현재 관절가동범위 데이터 값에 따라 원 그래프 이동시킬때 필요한 것들
        switch (type)
        {
            case 1: // 내전/외전

                abc("1");
                rotation_value = new List<float>() { 136f, 178f, value[1], value[0] }; // 디비에서 값 읽어온 후 이 변수에 넣어주기
                break;

            case 2: // 내회전/외회전

                abc("2");
                rotation_value = new List<float>() { 117f, 104f, value[1], value[0] }; // 디비에서 값 읽어온 후 이 변수에 넣어주기
                break;

            case 3: // 굴곡/신전

                abc("3");
                rotation_value = new List<float>() { 180f, 76f, value[1], value[0] }; // 디비에서 값 읽어온 후 이 변수에 넣어주기
                break;

        }
        obj.Add(Ordinary_person1); obj.Add(Ordinary_person2); obj.Add(User1); obj.Add(User2);

        // 유니티 상에서 자동으로 Text 오브젝트 채우기
        Range_of_motion_text = Range_of_motion.transform.Find("Text").GetComponent<Text>();
        Ordinary_person1_text = Ordinary_person1.transform.Find("Text").GetComponent<Text>();
        Ordinary_person2_text = Ordinary_person2.transform.Find("Text").GetComponent<Text>();
        User1_text = User1.transform.Find("Text").GetComponent<Text>();
        User2_text = User2.transform.Find("Text").GetComponent<Text>();

        obj_text.Add(Ordinary_person1_text); obj_text.Add(Ordinary_person2_text); obj_text.Add(User1_text); obj_text.Add(User2_text);

        // 원 영역 설정
        Circle_Ordinary_person.GetComponent<Image>().fillAmount = ((rotation_value[0] + rotation_value[1]) / 360);
        Circle_User.GetComponent<Image>().fillAmount = (float)((double)(rotation_value[2] + rotation_value[3]) / (double)360);

        // 원 가운데 값 설정 및 회전
        Range_of_motion.transform.eulerAngles = new Vector3(0, 0, (rotation_value[2] - rotation_value[3]) / 2 * -1); 
        Range_of_motion_text.transform.localEulerAngles = new Vector3(0, 0, (rotation_value[2] - rotation_value[3]) / 2);
        Range_of_motion_text.text = (rotation_value[2] + rotation_value[3]).ToString() + "°";

        // { 180, 60, 75, 25 } -> { -180, 60, -75, 25 } 회전 방향이 달라서 변경해야함
        rotation_value[0] *= -1; rotation_value[2] *= -1;
        Circle_Ordinary_person.transform.eulerAngles = new Vector3(0, 0, rotation_value[0]);
        Circle_User.transform.eulerAngles = new Vector3(0, 0, rotation_value[2]);
        #endregion

        // 현재 관절가동범위에서 텍스트 4개 회전하게하는 코드
        for (int i = 0; i < obj.Count; i++)
        {
            // 텍스트 상위 오브젝트(빈오브젝트 회전)
            obj[i].transform.eulerAngles = new Vector3(0, 0, rotation_value[i]);
            // 텍스트는 상위 오브젝트와 같은 값으로 회전하되 반대로 회전 ex) 상위 오브젝트 +90회전시 텍스트는 -90회전을 해야 텍스트가 안 기울고 제대로 보임
            obj_text[i].transform.localEulerAngles = new Vector3(0, 0, rotation_value[i] * -1);
            if (rotation_value[i].ToString()[0] == '-')
                obj_text[i].text = rotation_value[i].ToString().Substring(1); // -빼고 출력하기 ex) -180도 -> 180도
            else
                obj_text[i].text = rotation_value[i].ToString();
            obj_text[i].text += "°"; // 뒤에 도 붙이기
        }
    }

    public void abc(string type1)
    {
        #region 현재 관절 가동범위
        
        if(type1 == "1") { 
            x = 2; y = 3;
        }
        else if(type1 == "2") { 
            x = 4; y = 5;
        }
        else if (type1 == "3") { 
            x = 6; y = 7;
        }

        DBManager.DbConnectionChecek();
        DBManager.DataBaseRead(string.Format("SELECT * FROM Measurement WHERE userID = {0}  ORDER BY date DESC", userID));
        value.Clear();
        while (DBManager.dataReader.Read())
        {
            value.Add(DBManager.dataReader.GetFloat(x));
            value.Add(DBManager.dataReader.GetFloat(y));
            if(value[0] != 0 && value[1] != 0)
                break;
        }

        DBManager.DBClose();
        
        // (굴곡/신전, 내전 / 외전, 내회전/외회전)에서 현재 관절가동범위 데이터 값에 따라 원 그래프 이동시킬때 필요한 것들 Start
        // 돌아가는 거 확인하려고 Update에 작성함
        // 완성된 후엔 일반 메소드로 작성하고 start() 마지막에서 호출할 것
        for (int i = 0; i < obj.Count; i++)
        {
            // 텍스트 상위 오브젝트(빈오브젝트 회전)
            obj[i].transform.eulerAngles = new Vector3(0, 0, rotation_value[i]);
            // 텍스트는 상위 오브젝트와 같은 값으로 회전하되 반대로 회전 ex) 상위 오브젝트 +90회전시 텍스트는 -90회전을 해야 텍스트가 안 기울고 제대로 보임
            obj_text[i].transform.localEulerAngles = new Vector3(0, 0, rotation_value[i] * -1);
            if (rotation_value[i].ToString()[0] == '-')
                obj_text[i].text = rotation_value[i].ToString().Substring(1); // -빼고 출력하기 ex) -180도 -> 180도
            else
                obj_text[i].text = rotation_value[i].ToString();
            obj_text[i].text += "°"; // 뒤에 도 붙이기
        }
        #endregion

        #region 운동 수행 시간
        DBManager.DbConnectionChecek();
        DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0}", userID));
        float total_play_time = 0;

        while (DBManager.dataReader.Read())                            // 쿼리로 돌아온 레코드 읽기
            if (DBManager.dataReader.GetString(2).Substring(0, 1) != "3") // 제일 앞자리가 3인 건 원회전으로 따로 분류함
                if (DBManager.dataReader.GetString(2).Substring(1, 1) == type1) // 게임이름중 2번째 값 가져오는 코드
                    total_play_time += DBManager.dataReader.GetFloat(7);

        integrated_time.text = total_play_time.ToString("N2");

        int h = (int)(total_play_time / 3600);
        int m = (int)((total_play_time - h * 3600) / 60);
        int s = (int)((total_play_time % 60));
        integrated_time.text = h.ToString("D2") + " : " + m.ToString("D2") + " : " + s.ToString("D2"); // "D2" -> (2 -> 02) 2자리수 표현이라 앞에 0을 붙여줌
        DBManager.DBClose();
        #endregion

        #region 콘텐츠 수행 횟수
        DBManager.DbConnectionChecek();
        DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0}", userID));
        int game_play_count = 0;
        int game_score = 0;

        while (DBManager.dataReader.Read())                            // 쿼리로 돌아온 레코드 읽기
        {
            if (DBManager.dataReader.GetString(2).Substring(0, 1) != "3") // 제일 앞자리가 3인 건 원회전으로 따로 분류함
            {
                if (DBManager.dataReader.GetString(2).Substring(1, 1) == type1)
                {
                    game_play_count++;
                    game_score += DBManager.dataReader.GetInt32(8);
                    var check = true;
                    var temp = DBManager.dataReader.GetString(2).Substring(1);
                    for (int i = 0; i < t.Count; i++)
                        if (t[i] == temp)
                            check = false;

                    if (check)
                        t.Add(temp);
                }
            }
        }
        DBManager.DBClose();
        contents[0].text = t.Count.ToString() + "콘텐츠";
        contents[1].text = game_play_count.ToString() + "번";
        contents[2].text = game_score.ToString() + "점";
        #endregion
    }

    public string SqlFormat(string sql)
    {
        return string.Format("\"{0}\"", sql);
    }
}
