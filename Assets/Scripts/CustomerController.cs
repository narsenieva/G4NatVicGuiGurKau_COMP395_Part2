using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class CustomerController : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    public Transform targetWindow;
    public Transform targetCustomer=null;
    public Transform targetExit = null;

    public bool InService { get; set; }
    public GameObject atmWindow;
    public Queue queueManager;
    public Text Timer;
    public float elapsedSeconds = 0f;

    public enum CustomerState
    {
        None=-1,
        Entered, // customer going to ATM
        InService,
        Serviced
    }
    public CustomerState customerState = CustomerState.None;
    // Start is called before the first frame update
    void Start()
    {
        atmWindow = GameObject.FindGameObjectWithTag("ATMWindow");
        targetWindow = GameObject.FindGameObjectWithTag("ATMWindow").transform;
        targetExit = GameObject.FindGameObjectWithTag("CustomerExit").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
#if DEBUG_CC
        print("Start: this.GO.ID=" + this.gameObject.GetInstanceID());
#endif

        //
        customerState = CustomerState.Entered;
        FSMCustomer();

    }

    void FSMCustomer()
    {
#if DEBUG_CC
        print("CC.FSMCustomer:state="+carState+",ID="+this.gameObject.GetInstanceID());
#endif
        switch (customerState)
        {
            case CustomerState.None: //do nothing - shouldn't happen
                break;
            case CustomerState.Entered:
                DoEntered();
                break;
            case CustomerState.InService:
                DoInService();
                break;
            case CustomerState.Serviced:
                DoServiced();
                break;
            default:
                print("customerState unknown!:" + customerState);
                break;

        }
    }
    void DoEntered()
    {

        targetCustomer = targetWindow;

        queueManager = GameObject.FindGameObjectWithTag("ATMWindow").GetComponent<Queue>();
        queueManager.Add(this.gameObject);

        navMeshAgent.SetDestination(targetCustomer.position);
        navMeshAgent.isStopped = false;
    }
    void DoInService()
    {
        navMeshAgent.isStopped = true;
    }
    void DoServiced()
    {
        navMeshAgent.SetDestination(targetExit.position);
        navMeshAgent.isStopped = false;
    }
    public void ChangeState(CustomerState newCarState)
    {
        this.customerState = newCarState;
        FSMCustomer();
    }

    public void FixedUpdate()
    {
        elapsedSeconds += Time.deltaTime;
        Timer.text = elapsedSeconds.ToString();

    }
    public void SetInService(bool value)
    {
    }
    public void ExitService(Transform target)
    {
        queueManager.PopFirst();
        ChangeState(CustomerState.Serviced);
    }

    private void OnTriggerEnter(Collider other)
    {
#if DEBUG_CC
        Debug.LogFormat("CarController(this={0}).OnTriggerEnter:other={1}",this.gameObject.GetInstanceID(), other.gameObject.tag);
#endif
        if (other.gameObject.tag == "Customer")
        {
            //this.navMeshAgent.desiredVelocity.
            //if (targetCar == null)
            //{
                //targetCar = other.gameObject.transform;
                //navMeshAgent.SetDestination(targetCar.position);
            //}
        }
        else if (other.gameObject.tag == "ATMWindow")
        {
            ChangeState(CustomerState.InService);
            //SetInService(true);
        }
        else if (other.gameObject.tag == "CustomerExit")
        {
            Destroy(this.gameObject);
        }
    }


    private void OnDrawGizmos()
    {
#if DEBUG_CC
        print("InCC.OnDrawGizmos:targetCar.ID=" + targetCar.gameObject.GetInstanceID());
        print("InCC.OnDrawGizmos:targetCar.ID=" + targetExit.gameObject.GetInstanceID());

#endif
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, targetWindow.transform.position);
        if (targetCustomer)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(this.transform.position, targetCustomer.transform.position);
        }
        if (targetExit)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(this.transform.position, targetExit.transform.position);
        }
    }
}
