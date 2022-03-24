using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class NightMareController : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> dragonClips;    //0.die 1.attack 2.run
    [SerializeField]
    private AudioSource dragonAS;
    private bool isplayDieSound = false;

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

    private enum NightMareState
    {
        Walk,
        Attack,
        Idle,
        Die
    }
    [SerializeField]
    private int killNum;
    public float damagePerAttack;
    public float attackRange;
    [SerializeField]
    private float sightRadius;

    [SerializeField]
    private Health healthState;
    [SerializeField]
    private GameObject attackTarget;
    [SerializeField]
    private NightMareState nightMareState = NightMareState.Idle;

    private Animator anim;
    private NavMeshAgent agent;

    public float distanceToTarget;


    // Start is called before the first frame update
    void Start()
    {

        xMin = LeftObj.position.x;
        xMax = RightObj.position.x;
        zMin = FrontObj.position.z;
        zMax = BackObj.position.z;

        LeftObj.gameObject.SetActive(false);
        RightObj.gameObject.SetActive(false);
        FrontObj.gameObject.SetActive(false);
        BackObj.gameObject.SetActive(false);

        healthState = GetComponent<Health>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        

        if (healthState.isDead)
        {
            nightMareState = NightMareState.Die;
            agent.destination = transform.position;
            if (gameObject.GetComponent<SphereCollider>().enabled) {
                gameObject.GetComponent<SphereCollider>().enabled = false;
                agent.avoidancePriority = 50;
            }
        }
        else
        {
            if (IsTargetDie())
            {
                attackTarget = null;
            }
            SwitchState();

        }
        SwitchAnimation();
        AudioSoundSwitch();

        if (attackTarget != null)
        {
            distanceToTarget = Vector3.Distance(gameObject.transform.position, attackTarget.transform.position);
        }
        else {
            distanceToTarget = -999;
        }
    }

    private void AudioSoundSwitch()
    {
        if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Run")
        {
            dragonAS.clip = dragonClips[2];
            if (!dragonAS.isPlaying)
                dragonAS.Play();
        }
        else if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Basic Attack")
        {
            dragonAS.clip = dragonClips[1];
            if (!dragonAS.isPlaying)
                dragonAS.Play();
        }
        else if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "die" && !isplayDieSound)
        {
            isplayDieSound = true;
            dragonAS.clip = dragonClips[0];
            dragonAS.Play();
            dragonAS.loop = false;
        }
        else
        {
            if (!isplayDieSound)
                dragonAS.Stop();
        }
    }

    private void SwitchAnimation()
    {
        switch (nightMareState)
        {
            case NightMareState.Walk:
                anim.SetBool("Run", true);
                anim.SetBool("Attack", false);
                break;
            case NightMareState.Attack:
                anim.SetBool("Run", false);
                anim.SetBool("Attack", true);
                transform.LookAt(attackTarget.transform);
                break;
            case NightMareState.Idle:
                anim.SetBool("Run", false);
                anim.SetBool("Attack", false);
                break;
            case NightMareState.Die:
                if (!anim.GetBool("Die"))
                    anim.SetBool("Die", true);
                break;
            default:
                break;
        }
    }

    private void SwitchState()
    {
        FindEnemy();
        if (attackTarget == null) {
            //nightMareState = NightMareState.Idle;
            RandomMove();
        }
    }

    private bool FindEnemy()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {
            if (target.transform.name.Contains("Soldier"))
            {
                attackTarget = target.gameObject;
                if (!attackTarget.GetComponent<Health>().IsUnitDie())
                {
                    nightMareState = NightMareState.Walk;
                    agent.destination = attackTarget.transform.position;
                    if (Vector3.Distance(gameObject.transform.position, target.transform.position) < attackRange)
                    {
                        agent.destination = this.transform.position;
                        //attackStamp = -1;
                        nightMareState = NightMareState.Attack;
                    }
                }
                else {
                    attackTarget = null;
                }
            }
        }
        return false;
    }

    private bool IsTargetDie()
    {
        if (attackTarget)
        {
            if (attackTarget.GetComponent<Health>().IsUnitDie())
            {
                return true;
            }
            else
                return false;
        }
        return false;
    }
    private void MakeDamageToTarget()
    {

        Health targetHealth = attackTarget.GetComponent<Health>();
        if (targetHealth)
        {
            bool killTarget = false;
            targetHealth.GetDamage(damagePerAttack, gameObject, out killTarget);
            if (killTarget)
                killNum++;
        }
    }
    private void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        //Gizmos.color = Color.yellow;
        Gizmos.color = new Color(1, 0.92f, 0.016f, 0.4f);
        //Gizmos.color.a = 128 / 256f;
        Gizmos.DrawSphere(transform.position, sightRadius);
    }

    private void RandomMove() {
        if (Time.time > timeStamp + actionFrequency)
        {
            agent.SetDestination(GenerateRandomDestination());
            timeStamp = Time.time;
        }
        if (agent.remainingDistance > 1)
        {
            nightMareState = NightMareState.Walk;
        }
        else
        {
            //agent.destination = transform.position;
            nightMareState = NightMareState.Idle;
            //timeStamp = Time.time;
        }
    }

    private Vector3 GenerateRandomDestination()
    {
        float x = UnityEngine.Random.Range(xMin, xMax);
        float z = UnityEngine.Random.Range(zMin, zMax);
        return new Vector3(x, transform.position.y, z);
    }
}
