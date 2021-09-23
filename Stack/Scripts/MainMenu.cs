using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
	float soundB, soundE;
	public Slider BackGroundMusic, EffectMusic;
	public GameObject BgmButton, EmButton;
	public Sprite bgmon, bgmoff, emon, emoff;

	public AudioSource backgroundAudio, effectAudio;

	void Start()
    {
        Time.timeScale = 1f;
		BackGroundMusic.value = 0.5f;
		EffectMusic.value = 0.5f;
	}
    private void Update()
    {
        
    }
    public void ToGame(){
        switch (Data.Instance.GameID.Substring(0, 2))
        {
            case "11":
            case "12":
            case "13":
            case "41":
            case "42":
            case "43":
                SceneManager.LoadScene("StackGame");
                break;
            case "21":
            case "22":
            case "23":
                SceneManager.LoadScene("StackPassive");
                break;
        }
	}
    public void ToActiveScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Totutorial()
    {
		SceneManager.LoadScene("StackTutorial");
	}
	public void ToMain()
    {
		SceneManager.LoadScene("StackMain");
    }
    public void Quit()
    {
        SceneManager.LoadScene("Forest");
    }

    public void Warning()
    {
        Serial.instance.End();
        SceneManager.LoadScene("StackMain");
        Data.Instance.DeadTree++; 
        print(Data.Instance.DeadTree);
	}

	public void SoundChange()
    {
        if (EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite == bgmon)
        {
			soundB = BackGroundMusic.value;
			BackGroundMusic.value = BackGroundMusic.minValue;
			BgmButton.GetComponent<Image>().sprite = bgmoff;
        }
        else if(EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite == emon)
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
		if(SceneManager.GetActiveScene().name == "StackGame")
        {
			Stack.instance.effectAudio.volume = BackGroundMusic.value;
			Stack.instance.backgroundAudio.volume = EffectMusic.value;
        }
        else if (SceneManager.GetActiveScene().name == "StackTutorial")
        {
            StackTuto.instance.effectmusic.volume = BackGroundMusic.value;
            StackTuto.instance.backgroundmusic.volume = EffectMusic.value;
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
