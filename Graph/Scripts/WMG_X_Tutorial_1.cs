using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class WMG_X_Tutorial_1 : MonoBehaviour
{
	//  (굴곡/신전, 내전 / 외전, 내회전/외회전) - 그래프

	// 싱글톤 패턴
	#region Singleton
	private static WMG_X_Tutorial_1 _Instance;    // 싱글톤 패턴을 사용하기 위한 인스턴스 변수, static으로 선언하여 어디서든 접근 가능

	public GameObject emptyGraphPrefab;
	public GameObject Graphs;  // graphGO 그래프 리스트를 담는 빈 오브젝트, 복제시 부모로 지정하기 위해 넣음
	public GameObject graphGO; // 그래프 본 오브젝트
	
	public GameObject Point_Graph;
	public Vector3 Position_Value_Graph;

	public WMG_Axis_Graph graph;

	public WMG_Series series1, series2; // 데이터 시리즈들

	// 데이터 들어갈 변수들 string 타입으로 3/15, 40.0 형식으로 넣으면 아래서 Split으로 둘을 분리하여
	// groups과 data라는 List에 각각 값을 저장하고 이값을 토대로 그래프를 만들어줌
	public List<string> series1Data1, series1Data2;
	public List<string> groups; // x축 날짜 들어갈 곳
	public List<Vector2> data;  // y축 data 들어갈 곳

	// 만약 useData2가 flase일 경우 즉, 데이터를 읽어올 수 없는 경우(오류상황) 디폴트 값으로 그래프를 구성하기 위해 필요한 Vector2형 List
	public List<Vector2> series1DATA1, series1DATA2;

	// 그래프 x축 데이터 갯수 지정
	public int x_data_count;

	public bool useData2;

	public int type; // 1 : 굴곡/신전, 2 : 내전/외전, 3 : 내회전/외회전, 4 : 수평회전/수직회전 -> 유니티 인스펙터 창에서 지정
	public static WMG_X_Tutorial_1 Instance
	{
		get { return _Instance; }          // UIManager 인스턴스 변수를 리턴
	}

	void Awake()
	{
		_Instance = GetComponent<WMG_X_Tutorial_1>();  // 컴포넌트(자기 자신)에 대한 참조를 얻음
	    // useData2가 false인 예외상황 발생시 오류 안나게 하기 위해 값 넣어주기
		series1DATA1.Add(new Vector2(1, 90));
		series1DATA2.Add(new Vector2(1, 90));
		x_data_count = 6;
	}
	#endregion

	public string SqlFormat(string sql)
	{
		return string.Format("\"{0}\"", sql);
	}

	// Use this for initialization
	void Start()
	{
		Debug.Log("series1Data1의 갯수 : " + series1Data1.Count);
		Debug.Log("series1Data2의 갯수 : " + series1Data2.Count);

		#region (굴곡/신전, 내전 / 외전, 내회전/외회전) 데이터 가져오기~~
		string userID = SqlFormat(Data.Instance.UserID);
		string gameID = SqlFormat(Data.Instance.GameID);

		DBManager.DbConnectionChecek();
		DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0} ORDER BY date DESC", userID));
		series1Data1.Clear();
		series1Data2.Clear();
		while (DBManager.dataReader.Read())                            // 쿼리로 돌아온 레코드 읽기
		{
			if (DBManager.dataReader.GetString(2).Substring(1, 1) == type.ToString()) { // 게임이름중 2번째 값 가져오는 코드
				// 5가 min  6이 max값임
				series1Data1.Add(DBManager.dataReader.GetString(0).Substring(6, 7) + ", " + DBManager.dataReader.GetFloat(5).ToString("N1"));
				series1Data2.Add(DBManager.dataReader.GetString(0).Substring(6, 7) + ", " + DBManager.dataReader.GetFloat(6).ToString("N1"));
				if (series1Data1.Count == x_data_count) break;
			}
		}
		
		// 회전수 변화 추세
		if(type == 4)
        {
			DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0} ORDER BY date DESC", userID));
			while (DBManager.dataReader.Read())                            // 쿼리로 돌아온 레코드 읽기
			{
				if (DBManager.dataReader.GetString(2).Substring(0, 1) == "3") { 
					series1Data1.Add(DBManager.dataReader.GetString(0).Substring(6, 7) + ", " + DBManager.dataReader.GetFloat(4).ToString("N1"));
					series1Data2.Add(DBManager.dataReader.GetString(0).Substring(6, 7) + ", " + DBManager.dataReader.GetFloat(4).ToString("N1"));
				}
				if (series1Data1.Count == x_data_count) break;
			}
		}
		DBManager.DBClose();
		// 날짜 기준 내림차순으로 가져와서 데이터가 뒤집어져있어서 다시 뒤집는 작업
		series1Data1.Reverse(); 
		series1Data2.Reverse();
		#endregion

		// 프리팹 복제한 오브젝트를 graphGO에 삽입
		graphGO = Instantiate(emptyGraphPrefab);
		// graphGO 오브젝트의 부모지정
		graphGO.transform.SetParent(Graphs.transform, false);
		graph = graphGO.GetComponent<WMG_Axis_Graph>();

		series1 = graph.addSeries();
		series2 = graph.addSeries();

		// Point_Graph로 그래프 위치 지정
		Position_Value_Graph = Point_Graph.transform.position;

		// type2값으로 1, 2를 준 이유는 그래프별로 색상과 legend값을 다르게 하기 위함
		set(graphGO, Position_Value_Graph, 1, series1Data2, series1DATA2, series1);
		set(graphGO, Position_Value_Graph, 2, series1Data1, series1DATA1, series2);
		graphGO.transform.localScale *= 1.5f;
	}

	public void set(GameObject graphGO, Vector3 position, int type2, List<string> series1Data, List<Vector2> series1DATA, WMG_Series series1)
    {
		groups = new List<string>();
		data = new List<Vector2>();
		graph.xAxis.AxisMaxValue = 5;

		if (useData2) // 그냥 bool값
		{
			// 3/15, 40.0 형식으로 들어온걸 ","을 기준으로 Split 해서 x축 gropus과 y축 data 리스트에 add 시켜줌
			for (int i = 0; i < series1Data.Count; i++)
			{
				string[] row = series1Data[i].Split(',');
				groups.Add(row[0]);
				if (!string.IsNullOrEmpty(row[1]))
				{
					// Debug.Log(row[1]);
					float y = float.Parse(row[1]);
					data.Add(new Vector2(i + 1, y));
				}
			}
			graph.groups.SetList(groups); // x축값 넣기
			graph.useGroups = true;

			graph.xAxis.LabelType = WMG_Axis.labelTypes.groups;
			graph.xAxis.AxisNumTicks = groups.Count;

			series1.UseXDistBetweenToSpace = true;
			
			if (type2 == 1) { // 그래프1

				series1.seriesName = "최대값";
				// 그래프2 색상 지정
				series1.pointColor = HexToColor("795898");
				series1.lineColor = HexToColor("795898");
				if (type == 4)
                {
					series1.seriesName = "";
					series1.pointColor = HexToColor("FFFFFF");
					series1.lineColor = HexToColor("FFFFFF");
					// 회전수 변화 추세는 그래프 하나만 그리므로 레전드 하나는 안보이게 해줌
					//this.transform.Find("Entries").gameObject.transform.GetChild(0).gameObject.SetActive(false);
				}
            }
			else if(type2 == 2) { // 그래프2 
								  // 그래프1 색상 지정
				series1.pointColor = HexToColor("21D1A7");
				series1.lineColor = HexToColor("21D1A7");
				series1.seriesName = "최소값";
				if (type == 4)
				{
					series1.seriesName = "회전수";
				}
			}
			series1.pointValues.SetList(data); // y축값 넣기
		}
		else
		{
			Debug.Log("오류발생 : useData2값이 flase입니다.");
			series1.pointValues.SetList(series1DATA); // 오류상황일 경우 디폴트값을 y축값으로 넣기
		}
		// graphGO.SetActive(false);
		graphGO.transform.position = position;
	}

	// hex값을 Color변수로 바꿔주는 메서드 ex) 795898 -> Color32(00, 00, 00, 00);
	public Color HexToColor(string hex)
	{
		hex = hex.Replace("0x", "");
		hex = hex.Replace("#", "");
		byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
		byte a = 255;        // 아래 if문이 안 돌수도 있어서 디폴트값 지정
		if (hex.Length == 8) // 만약 투명도 값까지 줬다면 실행
			a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
		return new Color32(r, g, b, a);
	}
}