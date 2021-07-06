using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    public float BallSpeed = 100f;
    private Rigidbody2D BallRigi;

    private void Awake()
    {
        BallRigi = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        

        float randomX;
        randomX = Random.Range(-1.0f, 1.0f);//처음 공이 나가는 방향입니다

        Vector2 vector2 = new Vector2(randomX, 1f);//y좌표 축은 위로 고정해서 위로만 나가게 만들었습니다
        vector2 = vector2.normalized;

        BallRigi.AddForce(vector2 * BallSpeed);//발사
    }

    private void OnCollisionEnter2D(Collision2D collision)//게임이 진행 될때 대부분 충돌판정으로 게임이 진행 됩니다.
    {
        if (collision.gameObject.tag == "Block")//충돌한게 블록일 경우
        {
            BrickSound.instance.blockPoP();//소리
            Destroy(collision.gameObject);//블록 삭제

            if(SceneManager.GetActiveScene().name == "TutoGame")//튜토리얼 게임용
            {
                TutoGameManager.instance.GameScor++;
            }
            else//일반 게임용
            {
                BrickGame.instance.GameScor++;
            }
        }
        else if(collision.gameObject.tag == "Flowe") //슬라이더 바 아래 바닥입니다. 바닥이랑 충돌할 경우
        {
            if (SceneManager.GetActiveScene().name == "TutoGame") //튜토리얼 게임용
            {
                TutoGameManager.instance.GameOvQ = true;
            }
            else //일반 게임용
            {
                BrickGame.instance.GameOvQ = true;
                BrickGame.instance.ifOver();

            }
        }
        else if(collision.gameObject.tag == "main_block")//메인 화면 블록이랑 충돌했을 경우 입니다.
        {
            Destroy(collision.gameObject);
            MainManager.instance.MainScor++;
        }

        //공이 수직 또는 수평으로만 날아갈때 게임이 진행이 안되는걸 막기 위한 부분인데 솔직히 작동하는 건지 잘 모르겠습니다.
        if (BallRigi.velocity.x == 0f || BallRigi.velocity.y == 0f)
        {
            float randomX;
            randomX = Random.Range(-1.0f, 1.0f);
            float randomY;
            randomY = Random.Range(-1.0f, 1.0f);
            Vector2 vector2 = new Vector2(randomX, randomY);
            vector2 = vector2.normalized;

            BallRigi.AddForce(vector2 * BallSpeed);
        }
    }
}
