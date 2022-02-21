using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arrival : MonoBehaviour
{

    public GameObject customerPrefab;
    public Transform customerSpawnPlace;


    public float arrivalRateAsCustomersPerHour = 200; // customer/hour


    public float interArrivalTimeInHours; // = 1.0 / arrival rate of customeer per hour;
    private float interArrivalTimeInMinutes;
    private float interArrivalTimeInSeconds;

    public bool generateArrivals = true;
    // simple minimum and maximum InterArrivalTime in seconds
    public float minInterArrivalTimeInSeconds = 3; 
    public float maxInterArrivalTimeInSeconds = 60;
    public float timeScale = 1;

    public Slider sliderTScale;
    

    public enum ArrivalIntervalTimeStrategy
    {
        ConstantIntervalTime,
        UniformIntervalTime,
        ExponentialIntervalTime,
        ObservedIntervalTime
    }

    public ArrivalIntervalTimeStrategy arrivalIntervalTimeStrategy=ArrivalIntervalTimeStrategy.UniformIntervalTime;

    Queue queueManager;

    //UI debugging
#if DEBUG_AP
    public Text txtDebug;
#endif

    // Start is called before the first frame update
    void Start()
    {
        queueManager = GameObject.FindGameObjectWithTag("ATMWindow").GetComponent<Queue>();
        interArrivalTimeInHours = 1.0f / arrivalRateAsCustomersPerHour;
        interArrivalTimeInMinutes = interArrivalTimeInHours * 60;
        interArrivalTimeInSeconds = interArrivalTimeInMinutes * 60;
        StartCoroutine(GenerateArrivals());
#if DEBUG_AP
        print("proc#:" + System.Environment.ProcessorCount);
        txtDebug.text = "\nproc#:" + System.Environment.ProcessorCount;
#endif
    }

    private void Update()
    {
        timeScale = sliderTScale.value;
    }

    IEnumerator GenerateArrivals()
    {
        while (generateArrivals)
        {
            GameObject customerGO=Instantiate(customerPrefab, customerSpawnPlace.position, Quaternion.identity);

            float timeToNextArrivalInSec = interArrivalTimeInSeconds;
            switch (arrivalIntervalTimeStrategy)
            {
                case ArrivalIntervalTimeStrategy.ConstantIntervalTime:
                    timeToNextArrivalInSec= interArrivalTimeInSeconds;
                    break;
                case ArrivalIntervalTimeStrategy.UniformIntervalTime:
                    timeToNextArrivalInSec = Random.Range(minInterArrivalTimeInSeconds, maxInterArrivalTimeInSeconds);
                    break;
                case ArrivalIntervalTimeStrategy.ExponentialIntervalTime:
                    float U = Random.value;
                    float Lambda = 1 / arrivalRateAsCustomersPerHour;
                    timeToNextArrivalInSec = GetExp(U,Lambda);
                    break;
                case ArrivalIntervalTimeStrategy.ObservedIntervalTime:
                    timeToNextArrivalInSec = interArrivalTimeInSeconds;
                    break;
                default:
                    print("No acceptable arrivalIntervalTimeStrategy:" + arrivalIntervalTimeStrategy);
                    break;

            }

            yield return new WaitForSeconds(timeToNextArrivalInSec / timeScale);

        }

    }
    static float GetExp(float u, float lambda)
    {
        return -Mathf.Log(1 - u) / lambda;
    }
}


   
