using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScale : MonoBehaviour
{
    public Slider sliderTScale;
    public Text txtTScale;
    public float tScale = 1f;
    // Start is called before the first frame update
    void Start()
    {
        txtTScale.text = "Time Scale: X" + tScale;
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        tScale = sliderTScale.value;
        Time.timeScale = tScale;
        txtTScale.text = "Time Scale: X" + tScale;
    }

}
