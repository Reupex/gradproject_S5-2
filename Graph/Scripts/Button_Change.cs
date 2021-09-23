using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Button_Change : MonoBehaviour
{
    public List<GameObject> UI;

    // ui창 중에 선택된 거 빼고는 다 false 선택된 것만 true
    private void Change(int c)
    {
        for (int i = 0; i < 5; i++)
        {
            if (i != c)
                UI[i].SetActive(false);
            else
                UI[i].SetActive(true);
        }
    }
    // 디폴트는 통합 UI로 하기
    public void One() { Change(0); }
    public void Two() { Change(1); }
    public void Three() { Change(2); }
    public void Four() { Change(3); }
    public void Five() { Change(4); }

    // 결과창 전으로 LoadScene 하기 - 수정!
    public void Back()
    {
        SceneManager.LoadScene("Forest");
    }
}
