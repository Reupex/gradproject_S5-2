using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Device2 : MonoBehaviour
{
    public Slider slider;
    
    int plus = 0;
    public List<float> values = new List<float>();

    public static Device2 instance;

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
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = Serial.instance.angle;
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
