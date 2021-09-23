using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ForestManager : MonoBehaviour
{
    public Text fText;
    public Text dText;
    public List<Transform> Ground = new List<Transform>();
    public List<GameObject> TreePrefabs = new List<GameObject>();

    Transform GroundPieces;
    Transform Parent;

    int FreshTree = 0;
    int DeadTree = 0;
    int count = 0;
    int[] PoseTree = new int[8];
    GameObject tree;
    public GameObject tree2;
    public GameObject tree3;
    public GameObject ecText;

    bool isRevise = false;

    List<int> index = new List<int>();
    List<string> child = new List<string>();
    public List<int> countTree = new List<int>();
    public List<GameObject> treeList = new List<GameObject>();
    void Start()
    {
        treeList.Clear();
        CountTree();
        GroundPieces = GameObject.Find("GroundPieces").transform;
        // CSV -> 나무
        string FolderName = Application.dataPath + @"\CSV";
        DirectoryInfo di = new DirectoryInfo(FolderName);
        if (!di.Exists)
            di.Create();
        ReadLineFile(FolderName + @"\"+ Data.Instance.UserID + "_Tree" + ".csv");
        PlantTree("Init"); // 기존 나무 생성

        for (int i = 0; i < GroundPieces.childCount; i++)
        {
            if (GroundPieces.GetChild(i).childCount == 0)
                Ground.Add(GroundPieces.GetChild(i));
        }
        PlantTree("Random"); // 새로운 나무 생성
        SaveTree();
    }

    // Update is called once per frame
    void Update()
    {
        fText.text = FreshTree.ToString();
        dText.text = DeadTree.ToString();

        // Device 보정
        //if (Serial.instance.isRevise)
        //{
        //    for (int i = 0; i < 4; i++)
        //    {
        //        ecText.transform.GetChild(i + 2).gameObject.GetComponent<Text>().text = Serial.instance.ecText[i];
        //        print(Serial.instance.ecText[i]);
        //    }
        //}
    }

    public void ContentsTree(string TreeNum, int pindex)
    {
        GameObject prefab = null;
        switch (TreeNum)
        {
            case "11": // 능동 - 내/외전
                prefab = TreePrefabs[0];
                break;
            case "12": // 능동 - 내/외회전
                prefab = TreePrefabs[1];
                break;
            case "13": // 능동 - 굴곡/신전
                prefab = TreePrefabs[2];
                break;
            case "14": // 중단나무
                prefab = TreePrefabs[3];
                break;
            case "21": // 수동 - 내/외전
                prefab = TreePrefabs[4];
                break;
            case "22": // 수동 - 내/외회전
                prefab = TreePrefabs[5];
                break;
            case "23": // 수동 - 굴곡/신전
                prefab = TreePrefabs[6];
                break;
            case "24": // 중단나무
                prefab = TreePrefabs[7];
                break;
            case "31": // 원회전 - 수직
                prefab = TreePrefabs[8];
                break;
            case "32": // 원회전 - 수평
                prefab = TreePrefabs[9];
                break;
            case "34": // 중단나무
                prefab = TreePrefabs[10];
                break;
            case "41": // 저항 - 내/외전
                prefab = TreePrefabs[11];
                break;
            case "42": // 저항 - 내/외회전
                prefab = TreePrefabs[12];
                break;
            case "43": // 저항 - 굴곡/신전
                prefab = TreePrefabs[13];
                break;
            case "44": // 중단나무
                prefab = TreePrefabs[14];
                break;
        }

        // 나무 clone 생성 코드
        Parent = GroundPieces.GetChild(pindex).gameObject.transform; //부모 Ground 지정
        tree = Instantiate(prefab, transform.position, transform.rotation) as GameObject; // 나무 생성
        tree.transform.SetParent(Parent);
        tree.transform.localPosition = new Vector3(0, 1, 0); // 위치 지정(Ground 정가운데 생성되도록)
        treeList.Add(tree); 
    }

    public void PlantTree(string mode)
    {
        GameObject tree;
        if(mode == "Init")
        {
            for (int i = 0; i < child.Count; i++)
            {
                if (child[index[i]].Length == 2)
                    ContentsTree(child[index[i]], index[i]);
            }
        }
        else if(mode == "Random")
        {
            for (int i = 0; i < Data.Instance.DeadTree; i++)
            {
                int random = Random.Range(0, Ground.Count);
                ContentsTree(Data.Instance.GameID.Substring(0, 1) + "4", random);
                DeadTree++;
                Ground.RemoveAt(random); // 한번 지정된 Ground 삭제
            }
            for (int i = 0; i < Data.Instance.FreshTree; i++)
            {
                int random = Random.Range(0, Ground.Count);
                ContentsTree(Data.Instance.GameID.Substring(0, 2), random);
                FreshTree++;
                Ground.RemoveAt(random); // 한번 지정된 Ground 삭제
            }
        }
    }

    public void CountTree() // DB에서 forest 데이터 가져오기
    {
        FreshTree = 0;
        DeadTree = 0;
        DBManager.DataBaseRead(string.Format("SELECT * FROM Forest WHERE userID = '{0}'", Data.Instance.UserID));
        while (DBManager.dataReader.Read())
        {
            for (int i = 1; i < 16; i++)
            {
                if(i < 9)
                    tree3.transform.GetChild(i - 1).gameObject.transform.GetComponent<Text>().text = DBManager.dataReader.GetInt32(i).ToString();
                else
                    tree2.transform.GetChild(i - 9).gameObject.transform.GetComponent<Text>().text = DBManager.dataReader.GetInt32(i).ToString();
                if (i == 4 || i == 8 || i == 11 || i == 15) // 중단 나무 개수를 나타내는 인덱스이기 때문에
                    DeadTree += DBManager.dataReader.GetInt32(i);
                else
                    FreshTree += DBManager.dataReader.GetInt32(i);
                
                if(countTree.Count < 15) { countTree.Add(DBManager.dataReader.GetInt32(i)); print("더해"); }
                else { countTree[i - 1] = DBManager.dataReader.GetInt32(i); }
            }
        }
        DBManager.DBClose();
    }

    public void StartContents() // [콘텐츠 시작]
    {
        SceneManager.LoadScene("Choice");
    }

    public void StartTraining() // [콘텐츠 시작]
    {
        SceneManager.LoadScene("Choice2");
    }

    public void StartM() // 측정 확인 메시지
    {
        GameObject.Find("Canvas").transform.Find("StartMeasurement").transform.Find("ready").gameObject.GetComponent<Text>().text
            = Data.Instance.UserName + " 님,\n" + System.DateTime.Now.ToString("yyyy년 MM월 dd일") + "\n측정을 시작하시겠습니까?";
    }

    public void StartC() // 콘텐츠 시작 확인 메시지
    {
        GameObject button = EventSystem.current.currentSelectedGameObject; // 선택한 버튼(GameObject)
        if (button.name == "Contents")
        {
            GameObject.Find("Canvas").transform.Find("StartContents").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("StartContents").transform.Find("ready").gameObject.GetComponent<Text>().text
                = Data.Instance.UserName + "님,\n" + "콘텐츠를 선택하시겠습니까?";
        }
        else if (button.name == "Training")
        {
            GameObject.Find("Canvas").transform.Find("StartTraining").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("StartTraining").transform.Find("ready").gameObject.GetComponent<Text>().text
                = Data.Instance.UserName + "님,\n" + "훈련용 콘텐츠를 선택하시겠습니까?";
        }
    }
    public void Result() // [그래프]
    {
        SceneManager.LoadScene("Graph");
    }

    public void Measurement() // [측정하기]
    {
        SceneManager.LoadScene("Measurement");
    }

    public void Logout() // [로그아웃]
    {
        Destroy(GameObject.Find("Data").gameObject);
        // Serial.instance.SerialClose();
        SceneManager.LoadScene("Title");
    }

    // CSV 관련 스크립트
    public void SaveTree()
    {
        CSV_Data.TreeData.Clear();

        for (int i = 0; i < countTree.Count; i++)
            countTree[i] = 0;

        string t = "";
        for (int i = 0; i < GroundPieces.childCount; i++)
        {
            if(GroundPieces.GetChild(i).transform.childCount != 0)
                t = GroundPieces.GetChild(i).transform.GetChild(0).gameObject.name.Substring(0,2);
            else
                t = "x";
            Tree temp = new Tree
            {
                TreeIndex = i,
                isplant = t
            };
            CSV_Data.TreeData.Add(temp);
        }

        for (int i = 0; i < treeList.Count; i++)
        {
            switch (treeList[i].name.Substring(0, 2))
            {
                case "11": // 능동 - 내/외전
                    countTree[0]++;
                    break;
                case "12": // 능동 - 내/외회전
                    countTree[1]++;
                    break;
                case "13": // 능동 - 굴곡/신전
                    countTree[2]++;
                    break;
                case "14": // 중단나무
                    countTree[3]++;
                    break;
                case "21": // 수동 - 내/외전
                    countTree[4]++;
                    break;
                case "22": // 수동 - 내/외회전
                    countTree[5]++;
                    break;
                case "23": // 수동 - 굴곡/신전
                    countTree[6]++;
                    break;
                case "24": // 중단나무
                    countTree[7]++;
                    break;
                case "31": // 원회전 - 수직
                    countTree[8]++;
                    break;
                case "32": // 원회전 - 수평
                    countTree[9]++;
                    break;
                case "34": // 중단나무
                    countTree[10]++;
                    break;
                case "41": // 저항 - 내/외전
                    countTree[11]++;
                    break;
                case "42": // 저항 - 내/외회전
                    countTree[12]++;
                    break;
                case "43": // 저항 - 굴곡/신전
                    countTree[13]++;
                    break;
                case "44": // 중단나무
                    countTree[14]++;
                    break;
            }
        }


        // DB에 나무 정보 저장
        DBManager.DatabaseSQLAdd(string.Format("DELETE FROM Forest WHERE userID = '{0}'", Data.Instance.UserID)); 
        string query = "INSERT INTO Forest VALUES ('" + Data.Instance.UserID + "',";
        for (int i = 0; i < countTree.Count-1; i++)
        {
            query += countTree[i].ToString() + ",";
        }
        query += countTree[14].ToString() + ")";
        print(query);
        DBManager.DatabaseSQLAdd(query);
    }

    public void UpdateLineFile() // CSV 파일 업데이트
    {
        Data.Instance.FreshTree = 0;
        Data.Instance.DeadTree = 0;

        string filePath = CSV_Data.getPath(Data.Instance.UserID);        
        StreamWriter outStream = File.CreateText(filePath);
        string str;
        outStream.WriteLine("TreeIndex,isplant");
        for (int i = 0; i < CSV_Data.TreeData.Count; i++)
        {
            str = CSV_Data.TreeData[i].TreeIndex + "," + CSV_Data.TreeData[i].isplant;
            outStream.WriteLine(str);
        }
        outStream.Close();
    }

    public void ReadLineFile(string file) // CSV 파일 READ 함수
    {
        if (File.Exists(file))
        {
            List<Dictionary<string, object>> data = CSVReader.Read(file);
            for (var i = 0; i < data.Count; i++)
            {
                index.Add(int.Parse(data[i]["TreeIndex"].ToString()));
                child.Add(data[i]["isplant"].ToString());
            }
        }
    }
    public void SettingOffset() // 보정 값 받아오는 함수
    {
        Serial.instance.Active();
        ecText.transform.GetChild(0).gameObject.GetComponent<Text>().text = Serial.instance.angle.ToString();
        ecText.transform.GetChild(1).gameObject.GetComponent<Text>().text = Serial.instance.angle.ToString();
        Serial.instance.End();
    }
    
    public void Revise(Text text) // 보정
    {
        count++;
        if(count % 2 ==  1)
        {
            Serial.instance.Revise();
            text.text = "보정 끝내기";
        }
        else 
        {
            Serial.instance.End();
            text.text = "보정";
        }
    }
}

public class CSV_Data
{
    public static List<Tree> TreeData = new List<Tree>();

    public static string getPath(string UserID)
    {
        return Application.dataPath + "/CSV/" + UserID + "_Tree" + ".csv";
    }
}

public class Tree
{
    public int TreeIndex { get; set; }
    public string isplant { get; set; }
}

public class CSVReader
{
    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();

        FileInfo fi = new FileInfo(file);
        StreamReader sr = new StreamReader(fi.FullName);

        string strData = "";
        var strKey = sr.ReadLine().Split(',');

        while ((strData = sr.ReadLine()) != null)
        {
            var strValue = strData.Split(',');

            Dictionary<string, object> obj = new Dictionary<string, object>();
            for (int i = 0; i < strValue.Length; i++)
                obj.Add(strKey[i], strValue[i]);
            list.Add(obj);
        }
        sr.Close();

        return list;
    }
}
