using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum SoliderAnimation
{
    Run,
    Guard,
    Attack,
    Die
}

[RequireComponent(typeof(Health))]
public class SoilderComtroller : MonoBehaviour
{
    [SerializeField]
    private float shootRate = 0.1f;
    private float shootStamp = 0;


    [SerializeField]
    private ParticleSystem shootPS;

    [SerializeField]
    private GameObject attackTarget;

    [SerializeField]
    private List<AudioClip> deathClips;
    //[SerializeField]
    //private List<AudioClip> shootClips;

    [SerializeField]
    private AudioSource deathAS;
    [SerializeField]
    private AudioSource shootAS;

    private AgentController agentController;
    private bool sendDieMsgToAgents;

    [SerializeField]
    private Health healthState;

    [SerializeField]
    private float attackRange;
    [SerializeField]
    private float distanceToTarget;

    private Animator anim;
    private NavMeshAgent agent;
    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;

    public float distance;
    public bool isStop = false;
    private bool shouldMove;
    private bool shouldStop;

    public Vector3 destionation;
    [SerializeField]
    private SoliderAnimation soliderAnim;


    [SerializeField]
    private Transform hotKeyCamTrans;

    void Start()
    {
        agentController = GameObject.Find("AgentController").GetComponent<AgentController>();
        sendDieMsgToAgents = false;
        healthState = GetComponent<Health>();
        soliderAnim = SoliderAnimation.Guard;
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        //agent.updatePosition = false;
        //shootAS.clip = shootClips[UnityEngine.Random.Range(0, 2) % 2];
    }

    // Update is called once per frame
    void Update()
    {
        if (healthState.isDead)
        {
            //Debug.Log(gameObject.name + "I am dead");
            if (!sendDieMsgToAgents) {
                //SoundManager.instance.
                deathAS.clip = deathClips[UnityEngine.Random.Range(0, 7) % 7];
                deathAS.Play();
                sendDieMsgToAgents = true;
                SendDieMessage(agent);
                agent.destination = transform.position;
            }
            agent.enabled = false;
            soliderAnim = SoliderAnimation.Die;
        }
        else {
            if (IsTargetDie())
            {
                attackTarget = null;
            }
            //Debug.Log(gameObject.name +"I am alive");
            //Debug.Log(gameObject.name + " : (Time)" + Time.time);
            if (attackTarget != null)
            {
                agent.destination = attackTarget.transform.position;
                distanceToTarget = Vector3.Distance(transform.position, attackTarget.transform.position);
            }


            distance = Vector3.Distance(agent.destination, transform.position);
            if (agent.remainingDistance > 0.4)
            {
                soliderAnim = SoliderAnimation.Run;
            }
            else if (agent.remainingDistance >= 0 && agent.remainingDistance <= 0.4)
            {
                soliderAnim = SoliderAnimation.Guard;
            }

            if (attackTarget != null) {
                if (distanceToTarget <= attackRange) {
                    if (!shootAS.isPlaying)
                    {
                        shootStamp = -1;
                        shootAS.Play();
                    }
                    soliderAnim = SoliderAnimation.Attack;
                    agent.destination = transform.position;
                }
            }
        }
        if (soliderAnim != SoliderAnimation.Attack && shootAS.isPlaying) {
            //if (shootPS.isPlaying)
            //{
            //    //shootPS.Pause();
            //    shootPS.Stop();
            //}
            shootAS.Stop();
        }
        if (soliderAnim != SoliderAnimation.Attack && shootPS.isPlaying) {
            shootPS.Stop();
            foreach (var ps in shootPS.GetComponentsInChildren<ParticleSystem>())
            {
                if (ps.isPlaying)
                    ps.Stop();
            } 
        }
        if (soliderAnim == SoliderAnimation.Attack && Time.time > shootStamp + shootRate) {
            MakeDamageToTarget();
            shootStamp = Time.time;
        }


        SwitchAnimation();
    }

    private void SendDieMessage(NavMeshAgent unit)
    {
        if(agentController)
            agentController.RecieveDieMessage(unit);

    }

    private void SwitchAnimation() {
        switch (soliderAnim)
        {
            case SoliderAnimation.Run:
                anim.SetBool("isRun", true);
                anim.SetBool("Attack", false);
                break;
            case SoliderAnimation.Guard:
                anim.SetBool("isRun", false);
                anim.SetBool("Attack", false);
                break;
            case SoliderAnimation.Attack:
                anim.SetBool("Attack", true);
                if (!shootPS.isPlaying)
                {
                    shootPS.Play();
                    foreach (var ps in shootPS.GetComponentsInChildren<ParticleSystem>())
                    {
                        if (!ps.isPlaying)
                            ps.Play();
                    }
                }
                transform.LookAt(attackTarget.transform);
                break;
            case SoliderAnimation.Die:
                anim.SetBool("Die", true);
                //anim.SetTrigger("Die");
                break;
            default:
                break;
        }
    }

    public void SetAttackTarget(GameObject attackT) {
        Debug.Log("I get the attack target, the name is :" + attackT.name);
        attackTarget = attackT;
        //agent.destination = attackTarget.transform.position;
    }

    public void ClearAttackTarget()
    {
        Debug.Log("I clear the attack target");
        attackTarget = null;
        distanceToTarget = 0;
    }

    public bool IsUnitDead() {
        if (soliderAnim == SoliderAnimation.Die)
        {
            return true;
        }
        else
            return false;
    }
    private void MakeDamageToTarget()
    {

        Health targetHealth = attackTarget.GetComponent<Health>();
        if (targetHealth)
        {
            if (attackTarget.name.Contains("Red"))
            {
                targetHealth.GetDamage(1, gameObject, out bool killN, attackTarget.GetComponent<RedDragon>().GetInvincible());
            }
            else {
                targetHealth.GetDamage(1, gameObject, out bool killN);
            }
        }
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

    internal Vector3 GetHotKeyCamPositon()
    {
        return hotKeyCamTrans.position;
    }

    internal Quaternion GetHotKeyCamRoatation()
    {
        return hotKeyCamTrans.rotation;
    }
}
