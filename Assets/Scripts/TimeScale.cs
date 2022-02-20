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
    }

    // Update is called once per frame
    void Update()
    {
        tScale = sliderTScale.value;
        txtTScale.text = "Time Scale: X" + tScale;
    }

}
