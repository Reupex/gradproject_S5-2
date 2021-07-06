using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    public int Stage;//스테이지 매니저가 존재하는 단 한가지 이유. 메인 화면에서 스테이지 번호를 넘기기 위함!

    public static StageManager instance;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
