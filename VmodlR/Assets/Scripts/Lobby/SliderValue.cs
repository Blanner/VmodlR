using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class SliderValue : MonoBehaviour
{
    private Text sliderValueText;

    // Start is called before the first frame update
    void Start()
    {
        sliderValueText = GetComponent<Text>();
    }

    public void setValue(float value)
    {
        sliderValueText.text = value.ToString();
    }
}
