using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Device : MonoBehaviour
{
    public Slider slider;

    int MaxROM = 0;
    int plus = 0;
    public List<float> values = new List<float>();

    public static Device instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        SettingROM(); // ROM max 값 받아옴
    }

    // Update is called once per frame
    void Update()
    {
        SerialAngle(); // 디바이스 값 가져옴
    }

    void SettingROM()
    {
        switch(Data.Instance.GameID.Substring(1, 1))
        {
            case "1": // 내/외전
                MaxROM = Data.Instance.adduction + Data.Instance.abduction;
                if (MaxROM == 160) // 정상인일 때
                    plus = 0;
                else
                    plus = 10;
                slider.minValue = 180 - (Data.Instance.abduction + plus);
                slider.maxValue = Data.Instance.adduction + plus + 180;
                break;
            case "2": // 내/외회전
                MaxROM = Data.Instance.internalR + Data.Instance.externalR;
                if (MaxROM == 240) // 정상인일 때
                    plus = 0;
                else
                    plus = 10;
                slider.minValue = -(Data.Instance.internalR + plus); // 내회전 값
                slider.maxValue = Data.Instance.externalR + plus; // 외회전 값
                print(MaxROM);
                break;
            case "3": // 굴곡/신전
                MaxROM = Data.Instance.flextion + Data.Instance.extension;
                if (MaxROM == 240) // 정상인일 때
                    plus = 0;
                else
                    plus = 10;
                slider.minValue = 180 - (Data.Instance.extension + plus);
                slider.maxValue = 180 + (Data.Instance.flextion + plus);
                break;
        }
    }

    void SerialAngle()
    {
        if (slider.minValue < 0) // 내/외회전
        {
            if (Serial.instance.angle > 240)
                slider.value = Serial.instance.angle - 360;
            else
                slider.value = Serial.instance.angle;
        }
        else // 내/외전, 굴곡/신전
        {
            if (Serial.instance.angle == 0) 
                slider.value = Serial.instance.angle + 360;
            else 
                slider.value = Serial.instance.angle;
        }
        values.Add(slider.value);
    }
    
    public void ResultCircle(GameObject CircleGraph)
    {
        values.Sort();
        CircleGraph.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = 1.0f / 360.0f * (slider.minValue - plus);
        CircleGraph.transform.GetChild(1).gameObject.GetComponent<Image>().fillAmount = 1.0f / 360.0f * (slider.maxValue - plus);
        CircleGraph.transform.GetChild(2).gameObject.GetComponent<Image>().fillAmount = 1.0f / 360.0f * values[0]; // 가장 작은 값
        CircleGraph.transform.GetChild(3).gameObject.GetComponent<Image>().fillAmount = 1.0f / 360.0f * values[values.Count - 1]; // 가장 큰 값
        CircleGraph.transform.GetChild(4).gameObject.GetComponent<Image>().fillAmount = 1.0f / 360.0f * (slider.minValue - plus);
        CircleGraph.transform.GetChild(5).gameObject.GetComponent<Image>().fillAmount = 1.0f / 360.0f * (slider.maxValue - plus);
    }
}
