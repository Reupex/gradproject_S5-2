﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    #region Singleton                                         // 싱글톤 패턴은 하나의 인스턴스에 전역적인 접근을 시키며 보통 호출될 때 인스턴스화 되므로 사용하지 않는다면 생성되지도 않습니다.

    public int life = 3;
    public int maxMonsterCount = 10;
    public bool is_tutorial = false;
    public Vector3 curPos, nextPos;
    public bool check;

    private static Monster _Instance;          // 싱글톤 패턴을 사용하기 위한 인스턴스 변수, static 선언으로 어디서든 참조가 가능함
    public static Monster Instance                    // 객체에 접근하기 위한 속성으로 내부에 get set을 사용한다.
    {
        get { return _Instance; }                         // _sceneManager이 변수값을 리턴받을 수 있음.
    }

    void Awake()                                               // Start()보다 먼저 실행
    {
        _Instance = GetComponent<Monster>();    // _sceneManager변수에 자신의 SceneChangeManager 컴포넌트를 넣는다.
        check = false;
    }
    #endregion 

    private void Start()
    {
        // StartCoroutine(Damage());
    }
    // 1 : 몬스터 2초마다 스폰, 2 : 몬스터 1.5초마다 스폰, 3 : 몬스터 1초마다 스폰, 4 : 몬스터 0.7초마다 스폰
    void Update()
    {
        if (!is_tutorial && !ArrowUI.Instance.pause)
        {
            curPos = transform.position;
            nextPos = Vector3.down * ArrowGame.Instance.MonsterSpeed * (Time.deltaTime * 50);
            transform.position = curPos + nextPos;
        }
    }

    void OnTriggerEnter2D(Collider2D collision) // 제거
    {
        if (collision.gameObject.tag == "Border")
        {
            Destroy(gameObject);
        }

        if (is_tutorial)
        {
            if (collision.gameObject.tag == "Arrow_Destory")
            {
                Destroy(gameObject);
            }
        }

        if (collision.gameObject.tag == "Life")
        {
            ArrowUI.Instance.ui();
            Destroy(gameObject);
        }
    }
}