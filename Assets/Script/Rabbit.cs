using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum AnimalState
{
    Move,
    Stay
}
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class Rabbit : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent agent;

    [SerializeField]
    protected Transform LeftObj;
    [SerializeField]
    protected Transform RightObj;
    [SerializeField]
    protected Transform FrontObj;
    [SerializeField]
    protected Transform BackObj;
    [SerializeField]
    protected float xMin;
    [SerializeField]
    protected float xMax;
    [SerializeField]
    protected float zMin;
    [SerializeField]
    protected float zMax;

    [SerializeField]
    protected float timeStamp = 0;
    [SerializeField]
    protected float actionFrequency = 10;
    [SerializeField]
    protected AnimalState animalState = AnimalState.Stay;

    public float Destination;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        xMin = LeftObj.position.x;
        xMax = RightObj.position.x;
        zMin = FrontObj.position.z;
        zMax = BackObj.position.z;

        LeftObj.gameObject.SetActive(false);
        RightObj.gameObject.SetActive(false);
        FrontObj.gameObject.SetActive(false);
        BackObj.gameObject.SetActive(false);

    }
    // Update is called once per frame
    void Update()
    {
        Destination = agent.remainingDistance;
        //the frequency of Movement
        if (Time.time > timeStamp + actionFrequency)
        {
            agent.SetDestination(GenerateRandomDestination());
            timeStamp = Time.time;
        }
        if (agent.remainingDistance >1)
        {
            animalState = AnimalState.Move;
        }
        else {
            //agent.destination = transform.position;
            animalState = AnimalState.Stay;
            //timeStamp = Time.time;
        }

        RunAnimation();
    }

    private void RunAnimation()
    {
        switch (animalState)
        {
            case AnimalState.Move:
                anim.SetBool("Move", true);
                break;
            case AnimalState.Stay:
                anim.SetBool("Move", false);
                break;
            default:
                break;
        }
    }

    private Vector3 GenerateRandomDestination() {

        float x = Random.Range(xMin, xMax);
        float z = Random.Range(zMin, zMax);
        return new Vector3(x, transform.position.y, z);
    }
}
