using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//New as of Feb.25rd

public class Service : MonoBehaviour
{
    public GameObject customerInService;
    public Transform customerExitPlace;

    public float serviceRateAsCustomersPerHour = 20; // customer/hour
    public float interServiceTimeInHours; // = 1.0 / Service time of customer Per Hour;
    private float interServiceTimeInMinutes;
    private float interServiceTimeInSeconds;

    public bool generateServices = false;
    // minimum and maximum interservice time in seconds
    public float minInterServiceTimeInSeconds = 3;
    public float maxInterServiceTimeInSeconds = 60;

    Queue queueManager;

    public Text Timer;
    public float elapsedSeconds = 0f;

    public float timeScale = 1;

    public Slider sliderTScale;

    public enum ServiceIntervalTimeStrategy
    {
        ConstantIntervalTime,
        UniformIntervalTime,
        ExponentialIntervalTime,
        ObservedIntervalTime
    }

    public ServiceIntervalTimeStrategy serviceIntervalTimeStrategy = ServiceIntervalTimeStrategy.UniformIntervalTime;

    // Start is called before the first frame update
    void Start()
    {
        interServiceTimeInHours = 1.0f / serviceRateAsCustomersPerHour;
        interServiceTimeInMinutes = interServiceTimeInHours * 60;
        interServiceTimeInSeconds = interServiceTimeInMinutes * 60;
    }

    private void Update()
    {
        timeScale = sliderTScale.value;
    }

    private void FixedUpdate()
    {
        elapsedSeconds += Time.deltaTime;
        Timer.text = "Total time in seconds: " + elapsedSeconds.ToString();
    }
    private void OnTriggerEnter(Collider other)
    {
#if DEBUG_SP
        print("ServiceProcess.OnTriggerEnter:otherID=" + other.gameObject.GetInstanceID());
#endif

        if (other.gameObject.tag == "Customer")
        {
            customerInService = other.gameObject;
            customerInService.GetComponent<CustomerController>().SetInService(true);
            generateServices = true;
            StartCoroutine(GenerateServices());
        }
    }

    IEnumerator GenerateServices()
    {
        while (generateServices)
        {
            float timeToNextServiceInSec = interServiceTimeInSeconds;
            switch (serviceIntervalTimeStrategy)
            {
                case ServiceIntervalTimeStrategy.ConstantIntervalTime:
                    timeToNextServiceInSec = interServiceTimeInSeconds;
                    break;
                case ServiceIntervalTimeStrategy.UniformIntervalTime:
                    timeToNextServiceInSec = Random.Range(minInterServiceTimeInSeconds, maxInterServiceTimeInSeconds);
                    break;
                case ServiceIntervalTimeStrategy.ExponentialIntervalTime:
                    float U = Random.value;
                    float Lambda = 1 / serviceRateAsCustomersPerHour;
                    timeToNextServiceInSec = GetExp(U, Lambda);
                    break;
                case ServiceIntervalTimeStrategy.ObservedIntervalTime:
                    timeToNextServiceInSec = interServiceTimeInSeconds;
                    break;
                default:
                    print("No acceptable ServiceIntervalTimeStrategy:" + serviceIntervalTimeStrategy);
                    break;

            }

            generateServices = false;
            yield return new WaitForSeconds(timeToNextServiceInSec / timeScale);
        }
        customerInService.GetComponent<CustomerController>().ExitService(customerExitPlace);
        customerInService = null;

    }
    private void OnDrawGizmos()
    {
        if (customerInService)
        {
            Renderer r = customerInService.GetComponent<Renderer>();
            r.material.color = Color.green;
        }
    }

    static float GetExp(float u, float lambda)
    {
        //throw new NotImplementedException();
        return -Mathf.Log(1 - u) / lambda;
    }
}

