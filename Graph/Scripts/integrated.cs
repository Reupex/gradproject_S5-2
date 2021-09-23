using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class integrated : MonoBehaviour
{
    // 통합 + 원회전

    #region Singleton  
    // 통합 - 현재 상지 관절 가동범위 - 인덱스 순서대로 굴곡/신전, 외전/내전, 외회전/내회전
    public List<GameObject> obj_init = new List<GameObject>(3); // 초기값 - 이미지
    public List<Text> obj_init_text = new List<Text>(3); // 초기값 - 이미지 자식 텍스트
    public List<GameObject> obj_now = new List<GameObject>(3);  // 현재값 - 이미지
    public List<Text> obj_now_text = new List<Text>(3);  // 현재값 - 이미지 자식 텍스트
    public List<int> init_value = new List<int>();
    public List<int> now_value = new List<int>();
    public List<int> Ordinary_value = new List<int>();
    public List<float> init_value_fillAmount = new List<float>();
    public List<float> now_value_fillAmount = new List<float>();
    // 통합 - 개인정보
    public List<Text> user_info = new List<Text>(5); // 인덱스 순서대로  - 아이디(0-text), 이름(1-text), 성별(2-text), 나이(3-int), 시작일(7-text)

    // 통합 - 관절가동 범위 증가량 % 텍스트
    public List<Text> increment = new List<Text>(); // 통합 관절가동 범위 증가량 % 텍스트

    // 통합 - 통합 운동 수행 시간
    public Text integrated_time;

    // 통합 - 콘텐츠 수행 횟수
    public List<Text> contents = new List<Text>(); // 순서대로 콘텐츠 몇개, 몇 번, 몇 점
    public List<string> t = new List<string>();

    // 통합 - 오른쪽 위 업데이트 일
    public Text Last_Updated_Date;

    // 원회전 - 총 회전 횟수
    public Text numOfRotation;

    // 원회전 - 원 운동 수행시간
    public Text integrated_time2;

    // 원회전 - 콘텐츠 수행 횟수
    public List<Text> contents2 = new List<Text>(); // 순서대로 콘텐츠 몇개, 몇 번, 몇 점
    public List<string> t2 = new List<string>();

    private static integrated _Instance;          // 싱글톤 패턴을 사용하기 위한 인스턴스 변수, static 선언으로 어디서든 참조가 가능함
    public static integrated Instance                     // 객체에 접근하기 위한 속성으로 내부에 get set을 사용한다.
    {
        get { return _Instance; }                         
    }
    #endregion
    void Awake() // Start()보다 먼저 실행
    {
        _Instance = this;

        string userID = SqlFormat(Data.Instance.UserID);
        string gameID = SqlFormat(Data.Instance.GameID); 

        // 배열 초기화
        for (int i = 0; i < 3; i++)
        {
            init_value.Add(0);
            now_value.Add(0);
            init_value_fillAmount.Add(0);
            now_value_fillAmount.Add(0);
        }

        Ordinary_value = new List<int>(3) { 178 + 136, 104 + 117, 180 + 76 }; // 출처 : 표 3 위팔 체절의 관절가동범위 - 지연한테 받아옴
    
        #region 통합 - 개인 정보 user_info 인덱스 순서대로  - 아이디(0-text), 이름(1-text), 성별(2-text), 나이(3-int), 시작일(7-text)
        DBManager.DbConnectionChecek();
        DBManager.DataBaseRead(string.Format("SELECT * FROM UserInfo WHERE userID = {0}", userID));

        while (DBManager.dataReader.Read())                            // 쿼리로 돌아온 레코드 읽기
        {
            user_info[0].text = "아이디 : " + DBManager.dataReader.GetString(0);
            user_info[1].text = "이름 : " + DBManager.dataReader.GetString(1);
            user_info[2].text = "성별 : " + DBManager.dataReader.GetString(2);
            user_info[3].text = "생년월일 : " + (DBManager.dataReader.GetInt32(3)).ToString();
            user_info[4].text = "시작일 : " + DBManager.dataReader.GetString(7);
        }
        DBManager.DBClose();
        #endregion

        #region 통합 - 통합 운동 수행 시간
        DBManager.DbConnectionChecek();
        DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0}", userID));
        float total_play_time = 0;
        while (DBManager.dataReader.Read())                            // 쿼리로 돌아온 레코드 읽기
        {
            total_play_time += DBManager.dataReader.GetFloat(7);
        }
        integrated_time.text = total_play_time.ToString("N2");

        int h = (int)(total_play_time / 3600);
        int m = (int)((total_play_time - h * 3600) / 60);
        int s = (int)((total_play_time % 60));
        integrated_time.text = h.ToString("D2") + " : " + m.ToString("D2") + " : " + s.ToString("D2"); // "D2" -> (2 -> 02) 2자리수 표현이라 앞에 0을 붙여줌
        DBManager.DBClose();
        #endregion

        #region 통합 - 콘텐츠 수행 횟수
        DBManager.DbConnectionChecek();
        DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0}", userID));
        int game_play_count = 0;
        int game_score = 0;
        
        while (DBManager.dataReader.Read())                            // 쿼리로 돌아온 레코드 읽기
        {
            var check1 = true;
            var temp = DBManager.dataReader.GetString(2).Substring(0);
            if (DBManager.dataReader.GetString(2).Substring(0, 1) == "3") // 제일 앞자리가 3인 건 원회전으로 따로 분류함
            {
                game_play_count++;
                game_score += DBManager.dataReader.GetInt32(8);
                
                for (int i = 0; i < t.Count; i++)
                    if (t[i] == temp)
                        check1 = false;

                if (check1)
                    t.Add(temp);
            }
            else if(DBManager.dataReader.GetString(2).Substring(0, 1) != "3") // 제일 앞자리가 3인 건 원회전으로 따로 분류함
            {
                game_play_count++;
                game_score += DBManager.dataReader.GetInt32(8);
                check1 = true;
                temp = DBManager.dataReader.GetString(2).Substring(1);
                for (int i = 0; i < t.Count; i++)
                    if (t[i] == temp)
                        check1 = false;

                if (check1)
                    t.Add(temp);
            }
            
        }
        DBManager.DBClose();
        contents[0].text = (t.Count).ToString() + "콘텐츠";
        contents[1].text = game_play_count.ToString() + "번";
        contents[2].text = game_score.ToString() + "점";
        #endregion

        #region 통합 - 현재 상지 관절 가동범위 - 초기값, 현재값 가져오기
        DBManager.DbConnectionChecek();
        DBManager.DataBaseRead(string.Format("SELECT * FROM Measurement WHERE userID = {0}", userID));

        bool check = true;
        while (DBManager.dataReader.Read())                            // 쿼리로 돌아온 레코드 읽기
        {
            int value1 = DBManager.dataReader.GetInt32(2) + DBManager.dataReader.GetInt32(3);
            int value2 = (int)(DBManager.dataReader.GetFloat(4) + DBManager.dataReader.GetInt32(5));
            int value3 = DBManager.dataReader.GetInt32(6) + DBManager.dataReader.GetInt32(7);

            // 초기값 3개 세팅
            if (check)
            {
                if (value1 != 0 && init_value[0] == 0)
                    init_value[0] = value1;

                if (value2 != 0 && init_value[1] == 0)
                    init_value[1] = value2;

                if (value3 != 0 && init_value[2] == 0)
                    init_value[2] = value3;
            }

            if (init_value[0] != 0 && init_value[1] != 0 && init_value[2] != 0)
                check = false;

            // 최근값 3개 세팅
            if (value1 != 0)
                now_value[0] = value1;

            if (value2 != 0)
                now_value[1] = value2;

            if (value3 != 0)
                now_value[2] = value3;
        }
        DBManager.DBClose();

        for (int i = 0; i < 3; i++)
        {
            init_value_fillAmount[i] = ((float)init_value[i] / (float)Ordinary_value[i]);
            now_value_fillAmount[i] = ((float)now_value[i] / (float)Ordinary_value[i]);
        }
        #endregion 

        #region 원회전 - 총회전 횟수
        DBManager.DbConnectionChecek();
        DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0}", userID));
        int c = 0;
        while (DBManager.dataReader.Read())
        {
            gameID = DBManager.dataReader.GetString(2);
            Debug.Log("123123 : " + gameID.Substring(0, 2));
            if (gameID.Substring(0, 1) == "3") // 제일 앞자리가 3인 건 원회전으로 따로 분류함
                c += DBManager.dataReader.GetInt32(4);
        }
        numOfRotation.text = c.ToString() + "번";
        DBManager.DBClose();
        #endregion

        #region 원회전 - 개선된 관절가동범위
        // Improved_ROM.cs 파일에 작성함
        #endregion

        #region 원회전 - 운동 수행 시간
        DBManager.DbConnectionChecek();
        DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0}", userID));
        total_play_time = 0;
        while (DBManager.dataReader.Read())                            // 쿼리로 돌아온 레코드 읽기
        {
            if (DBManager.dataReader.GetString(2).Substring(0, 1) == "3")
                total_play_time += DBManager.dataReader.GetFloat(7);
        }
        h = (int)(total_play_time / 3600);
        m = (int)((total_play_time - h * 3600) / 60);
        s = (int)((total_play_time % 60));
        integrated_time2.text = h.ToString("D2") + " : " + m.ToString("D2") + " : " + s.ToString("D2"); // "D2" -> (2 -> 02) 2자리수 표현이라 앞에 0을 붙여줌
        DBManager.DBClose();
        #endregion

        #region 원회전 - 콘텐츠 수행 횟수
        DBManager.DbConnectionChecek();
        DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0}", userID));
        game_play_count = 0;
        game_score = 0;

        while (DBManager.dataReader.Read())                            // 쿼리로 돌아온 레코드 읽기
        {
            if(DBManager.dataReader.GetString(2).Substring(0,1) == "3")
            {
                game_play_count++;
                game_score += DBManager.dataReader.GetInt32(8);
                var check1 = true;
                var temp = DBManager.dataReader.GetString(2).Substring(1);
                for (int i = 0; i < t2.Count; i++)
                    if (t2[i] == temp)
                        check1 = false;

                if (check1)
                    t2.Add(temp);
            }
        }
        DBManager.DBClose();
        contents2[0].text = (t2.Count).ToString() + "콘텐츠";
        contents2[1].text = game_play_count.ToString() + "번";
        contents2[2].text = game_score.ToString() + "점";
        #endregion
    }

    // Start is called before the first frame update
    void Start()
    {
        #region 통합 - 현재 상지 관절가동범위 + 관절가동범위
        for (int i = 0; i < 3; i++)
        {
            // 초기값, 현재값 이미지 모두 50%로 채우기
            obj_init[i].GetComponent<Image>().fillAmount = init_value_fillAmount[i];
            obj_now[i].GetComponent<Image>().fillAmount = now_value_fillAmount[i];

            // 초기값, 현재값 텍스트 오브젝트 추가해주기
            obj_init_text.Add(obj_init[i].transform.GetChild(0).GetComponent<Text>());
            obj_now_text.Add(obj_now[i].transform.GetChild(0).GetComponent<Text>());

            // 텍스트값 초기화
            obj_init_text[i].text = ((int)(init_value_fillAmount[i] * 100f)).ToString() + "%";
            obj_now_text[i].text = ((int)(now_value_fillAmount[i] * 100f)).ToString() + "%";

            // 텍스트 위치 조정
            Vector3 v_init = new Vector3(0, obj_init[i].GetComponent<Image>().fillAmount * 1.6f, 0);
            obj_init_text[i].transform.position += v_init;
            Vector3 v_now = new Vector3(0, obj_now[i].GetComponent<Image>().fillAmount * 1.6f, 0);
            obj_now_text[i].transform.position += v_now;

            // 통합 - 관절가동범위 증가량
            increment[i].text = (((float)now_value[i] / (float)init_value[i]) * 100).ToString("N0") + "%";
        }
        #endregion
    }

    public string SqlFormat(string sql)
    {
        return string.Format("\"{0}\"", sql);
    }
}
