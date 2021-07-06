using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DriveMain : MonoBehaviour
{
    float soundB, soundE;
    public Slider BackGroundMusic, EffectMusic;
    public GameObject BgmButton, EmButton;
    public Sprite bgmon, bgmoff, emon, emoff;

    //--------- DBtest -----------
    string gameID = DBManager.SqlFormat(Data.Instance.GameID);
    //--------------------------

    // Start is called before the first frame update
    void Start()
    {
        BackGroundMusic.value = 0.5f;
        EffectMusic.value = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToGame()
    {
        switch (Data.Instance.GameID.Substring(0, 2))
        {
            case "11":
            case "12":
                SceneManager.LoadScene("Drive12");
                break;
            case "13":
                SceneManager.LoadScene("Drive3");
                break;
            case "23":
                SceneManager.LoadScene("Drive3Passive");
                break;
            case "31":
                SceneManager.LoadScene("Drive3Training");
                break;
        }
    }

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void EndTuto()
    {
        string check;
        check = SceneManager.GetActiveScene().name.Substring(5, 1);
        if (check == "3")
        {
            SceneManager.LoadScene("Drive3");
        }
        else SceneManager.LoadScene("Drive12");
    }

    public void Totutorial()
    {
        switch (Data.Instance.GameID.Substring(1, 1))
        {
            case "1":
            case "2":
                SceneManager.LoadScene("Drive12Tuto");
                break;
            case "3":
                SceneManager.LoadScene("Drive3Tuto");
                break;
        }
    }

    public void ToMain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("DriveMain");
    }

    public void Warning()
    {
        Time.timeScale = 1f;
        Serial.instance.End();
        SceneManager.LoadScene("DriveMain");
        Data.Instance.DeadTree++;
        print(Data.Instance.DeadTree);
    }

    public void Quit()
    {
        SceneManager.LoadScene("Forest");
    }

    public void SoundChange()
    {
        if (EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite == bgmon)
        {
            soundB = BackGroundMusic.value;
            BackGroundMusic.value = BackGroundMusic.minValue;
            BgmButton.GetComponent<Image>().sprite = bgmoff;
        }
        else if (EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite == emon)
        {
            soundE = EffectMusic.value;
            EffectMusic.value = BackGroundMusic.minValue;
            EmButton.GetComponent<Image>().sprite = emoff;
        }
        else if (EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite == bgmoff)
        {
            BackGroundMusic.value = soundB;
            BgmButton.GetComponent<Image>().sprite = bgmon;
        }
        else if (EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite == emoff)
        {
            EffectMusic.value = soundE;
            EmButton.GetComponent<Image>().sprite = emon;
        }

    }

    public void SoundValue()
    {
        if (SceneManager.GetActiveScene().name == "Drive3")
        {
            Drive3Game.instance.effectAudio.volume = EffectMusic.value;
            Drive3Game.instance.backgroundAudio.volume = BackGroundMusic.value;
        }
        else if (SceneManager.GetActiveScene().name == "Drive3Tuto")
        {
            Drive3Tuto.instance.effectAudio.volume = EffectMusic.value;
            Drive3Tuto.instance.backgroundAudio.volume = BackGroundMusic.value;
        }
        else if (SceneManager.GetActiveScene().name == "Drive12")
        {
            Drive12Game.instance.effectAudio.volume = EffectMusic.value;
            Drive12Game.instance.backgroundAudio.volume = BackGroundMusic.value;
        }
        else if (SceneManager.GetActiveScene().name == "Drive12Tuto")
        {
            Drive12Tuto.instance.effectAudio.volume = EffectMusic.value;
            Drive12Tuto.instance.backgroundAudio.volume = BackGroundMusic.value;
        }else if(SceneManager.GetActiveScene().name == "Drive3Training")
        {
            Drive3Training.instance.effectAudio.volume = EffectMusic.value;
            Drive3Training.instance.backgroundAudio.volume = BackGroundMusic.value;
        }

        if (EffectMusic.value == 0f)
        {
            EmButton.GetComponent<Image>().sprite = emoff;
        }
        else EmButton.GetComponent<Image>().sprite = emon;
        if (BackGroundMusic.value == 0f)
        {
            BgmButton.GetComponent<Image>().sprite = bgmoff;
        }
        else BgmButton.GetComponent<Image>().sprite = bgmon;
    }
}
