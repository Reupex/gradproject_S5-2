using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeScene : MonoBehaviour
{
    public static EscapeScene instance;

    void Awake()
    {
        if (EscapeScene.instance == null)
            EscapeScene.instance = this;
    }
    public void MainMenu()
    {
        EscapeSound.instance.menu_Click();
        SceneManager.LoadScene("EscapeMain");
    }

    public void GameStart()
    {
        EscapeSound.instance.menu_Click();
        SceneManager.LoadScene("EscapeDial");
        if(Data.Instance.GameID.Substring(0, 2) == "32")
            SceneManager.LoadScene("EscapeTraining");
    }

    public void Tutorial()
    {
        EscapeSound.instance.menu_Click();
        SceneManager.LoadScene("EscapeTuTo");
    }
    public void Training()
    {
        EscapeSound.instance.menu_Click();
        SceneManager.LoadScene("EscapeTraining");
    }
    public void GameQuit()
    {
        SceneManager.LoadScene("Forest");
    }
}
