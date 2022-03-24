using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class RedDragon : MonoBehaviour
{
    private enum RedDragonState
    {
        Sleep,
        Scream,
        Walk,
        TakeOff,
        FlyForward,
        Idle,
        Attack,
        Die,
        MoveToBase,
        Land
    }

    //[SerializeField]
    //private float attackRate = 1.05f;
    //private float attackStamp = 0;

    [SerializeField]
    private int killNum;
    public float damagePerAttack;

    [SerializeField]
    private List<AudioClip> dragonClips; //0.sleep, 1. roar,  2. die 3. wing flap 4 fly attack 5. bite 6.idle
    [SerializeField]
    private AudioSource dragonAS;
    private bool isplayDieSound = false;
    private bool isFlying = false;

    [SerializeField]
    private Health healthState;
    [SerializeField]
    private Transform originalPositon;
    public float DistanceOfOriginal;
    public float maxChaseDistance = 100;
    [SerializeField]
    private GameObject attackTarget;
    [SerializeField]
    private RedDragonState redDragonState = RedDragonState.Sleep;

    private Animator anim;
    private NavMeshAgent agent;
    [SerializeField]
    private float sightRadius;
    public float DistanceTarget;
    private bool needGoBack;

    [SerializeField]
    private bool attackWakeUp;

    [SerializeField]
    private bool canDoAction;

    [SerializeField]
    private GameObject firstCam;
    private ShakeTheCam shakeCam;

    private Quaternion OriginalRotation;

    // Start is called before the first frame update
    void Start()
    {
        OriginalRotation = transform.rotation;
        shakeCam = firstCam.GetComponent<ShakeTheCam>();
        needGoBack = false;
        canDoAction = false;
        healthState = GetComponent<Health>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (healthState.isDead) {
            redDragonState = RedDragonState.Die;
            agent.destination = transform.position;
            if (gameObject.GetComponent<BoxCollider>().enabled)
            {
                gameObject.GetComponent<BoxCollider>().enabled = false;
                //agent.avoidancePriority = 50;
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

        //if (redDragonState == RedDragonState.Attack && Time.time > attackStamp + attackRate) {
        //    MakeDamageToTarget();
        //    attackStamp = Time.time;
        //}

        SwitchAnimation();
        AudioSoundSwitch();
    }

    private bool IsTargetDie()
    {
        if (attackTarget) {
            if (attackTarget.GetComponent<Health>().IsUnitDie())
            {
                return true;
            }
            else
                return false;
        }
        return false;
    }

    private void SwitchAnimation() {
        switch (redDragonState)
        {
            case RedDragonState.Idle:
                anim.SetBool("Sleep", false);
                anim.SetBool("Chase", false);
                anim.SetBool("Attack", false);
                anim.SetBool("OutBase", false);
                //if (anim.GetCurrentAnimatorStateInfo(0).IsName("Land") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                //{
                //    transform.rotation = OriginalRotation;
                //    isFlying = false;
                //}
                if (isFlying && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle01") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    transform.rotation = OriginalRotation;
                    isFlying = false;
                    if (!gameObject.GetComponent<BoxCollider>().enabled)
                    {
                        gameObject.GetComponent<BoxCollider>().enabled = true;
                        //agent.avoidancePriority = 50;
                    }
                }
                //dragonAS.clip = dragonClips[6];
                //if (!dragonAS.isPlaying)
                //    dragonAS.Play();
                break;
            case RedDragonState.Sleep:
                anim.SetBool("Sleep", true);
                //dragonAS.clip = dragonClips[0];
                //if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Sleep")
                //{
                //    if (!dragonAS.isPlaying)
                //        dragonAS.Play();
                //}
                //else
                //{
                //    dragonAS.Stop();
                //}
                break;
            case RedDragonState.Scream:
                anim.SetBool("Sleep", false);
                //dragonAS.clip = dragonClips[1];
                //if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Scream")
                //{
                //    if(!dragonAS.isPlaying)
                //        dragonAS.Play();
                //}
                //else {
                //    dragonAS.Stop();
                //}
                //if (!dragonAS.isPlaying)
                //    dragonAS.Play();
                break;
            case RedDragonState.Walk:
                anim.SetBool("Chase", true);
                anim.SetBool("Attack", false);
                //shakeCam.enabled = true;
                //if (dragonAS.isPlaying)
                //    dragonAS.Stop();
                break;
            case RedDragonState.TakeOff:
                break;
            case RedDragonState.FlyForward:
                break;
            case RedDragonState.Attack:
                anim.SetBool("Attack", true);
                transform.LookAt(attackTarget.transform);
                //dragonAS.clip = dragonClips[5];
                //if (!dragonAS.isPlaying)
                //    dragonAS.Play();
                break;
            case RedDragonState.Die:
                if(!anim.GetBool("Die"))
                    anim.SetBool("Die", true);
                //dragonAS.clip = dragonClips[2];
                //dragonAS.loop = false;
                //if (!dragonAS.isPlaying)
                //    dragonAS.Play();
                break;
            case RedDragonState.MoveToBase:
                anim.SetBool("OutBase", true);
                anim.SetBool("Chase", false);
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Take Off") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) {
                    EndTakeOff();
                }
                
                break;
            default:
                break;
        }
    }

    private void AudioSoundSwitch() {

        if (anim.GetCurrentAnimatorClipInfoCount(0) > 0)
        {

            if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Scream")
            {
                dragonAS.clip = dragonClips[1];
                if (!dragonAS.isPlaying)
                    dragonAS.Play();
                shakeCam.enabled = true;
            }
            else if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Sleep")
            {
                dragonAS.clip = dragonClips[0];
                if (!dragonAS.isPlaying)
                    dragonAS.Play();
            }
            else if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Walk")
            {

                dragonAS.clip = dragonClips[7];
                if (!dragonAS.isPlaying)
                    dragonAS.Play();
            }
            else if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle")
            {
                dragonAS.clip = dragonClips[6];
                if (!dragonAS.isPlaying)
                    dragonAS.Play();
            }
            else if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Basic Attack")
            {
                dragonAS.clip = dragonClips[5];
                if (!dragonAS.isPlaying)
                    dragonAS.Play();
            }
            else if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle")
            {
                dragonAS.clip = dragonClips[6];
                if (!dragonAS.isPlaying)
                    dragonAS.Play();
            }
            else if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Die" && !isplayDieSound)
            {
                isplayDieSound = true;
                dragonAS.clip = dragonClips[2];
                dragonAS.Play();
                dragonAS.loop = false;
                //if (!dragonAS.isPlaying)
                shakeCam.setShakeTime = 2.0f;
                shakeCam.enabled = true;
            }
            else
            {
                if (!isplayDieSound)
                    dragonAS.Stop();
            }
        }
    }


    private void SwitchState() {
        DistanceOfOriginal = Vector3.Distance(transform.position, originalPositon.position);
        if (DistanceOfOriginal < maxChaseDistance && !needGoBack && attackTarget == null && !isFlying)
        {
            FindEnemy();
        }
        else if (DistanceOfOriginal >= maxChaseDistance)
        {
            agent.destination = originalPositon.position;
            attackTarget = null;
            needGoBack = true;
            isFlying = true;
        }
        else if (attackTarget == null && !needGoBack)
        {
            redDragonState = RedDragonState.Idle;
        }
        else if (attackTarget != null && !isFlying) {
            ChaseAndAttackTarget();
        }

        if (!attackWakeUp && healthState.damagerMaker != null)
        {
            attackWakeUp = true;
            attackTarget = healthState.damagerMaker;
            sightRadius = 65f;
            //if (redDragonState == RedDragonState.Sleep)
            //{
            //    redDragonState = RedDragonState.Scream;
            //    sightRadius = 65f;
            //}
        }


        if (needGoBack) {
            redDragonState = RedDragonState.MoveToBase;
            if (gameObject.GetComponent<BoxCollider>().enabled)
            {
                gameObject.GetComponent<BoxCollider>().enabled = false;
                //agent.avoidancePriority = 50;
            }
        }

        if (DistanceOfOriginal <= 1 && needGoBack) {
            needGoBack = false;
            EndFlying();
        }
    }

    private void ChaseAndAttackTarget()
    {
        DistanceTarget = Vector3.Distance(gameObject.transform.position, attackTarget.transform.position);
        if (redDragonState == RedDragonState.Sleep)
        {
            redDragonState = RedDragonState.Scream;
            sightRadius = 65f;
        }
        else if (redDragonState != RedDragonState.Sleep)
        {
            redDragonState = RedDragonState.Walk;
            if(canDoAction)
                agent.destination = attackTarget.transform.position;
            else
                agent.destination = transform.position;

            if (Vector3.Distance(gameObject.transform.position, attackTarget.transform.position) < 20)
            {
                agent.destination = this.transform.position;
                //attackStamp = -1;
                redDragonState = RedDragonState.Attack;
            }
        }
    }

    private bool FindEnemy() {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {
            if (target.transform.name.Contains("Soldier")) {

                attackTarget = target.gameObject;
                if (!attackTarget.GetComponent<Health>().IsUnitDie())
                {                    
                    DistanceTarget = Vector3.Distance(gameObject.transform.position, target.transform.position);
                    if (redDragonState == RedDragonState.Sleep)
                    {
                        redDragonState = RedDragonState.Scream;
                        sightRadius = 65f;
                    }
                    else if (redDragonState != RedDragonState.Sleep)
                    {
                        redDragonState = RedDragonState.Walk;
                        agent.destination = attackTarget.transform.position;

                        if (Vector3.Distance(gameObject.transform.position, target.transform.position) < 20)
                        {
                            agent.destination = this.transform.position;
                            //attackStamp = -1;
                            redDragonState = RedDragonState.Attack;
                        }
                    }
                    return true;
                }
            }
        }
        agent.destination = this.transform.position;
        if (redDragonState != RedDragonState.Sleep) {
            redDragonState = RedDragonState.Idle;
            attackTarget = null;
        }
        return false;
    }

    internal bool GetInvincible()
    {
        if (canDoAction)
            return false;
        else
            return true;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        //Gizmos.color = Color.yellow;
        Gizmos.color = new Color(1,0.92f,0.016f,0.4f);
        //Gizmos.color.a = 128 / 256f;
        Gizmos.DrawSphere(transform.position, sightRadius);
    }

    private void MakeDamageToTarget() {

        Health targetHealth = attackTarget.GetComponent<Health>();
        if (targetHealth) {
            bool killTarget = false;
            targetHealth.GetDamage(damagePerAttack, gameObject, out killTarget);
            if(killTarget)
                killNum++;
        }
    }

    private void EndWakeUpProcess() {
        Invoke("EnableCanDoAction", 1f);
    }

    private void EnableCanDoAction() {
        canDoAction = true;
    }


    internal int GetKilledNum()
    {
        return killNum;
    }

    private void EndTakeOff() {
        anim.SetBool("FlyForward", true);
    }

    private void EndFlying()
    {
        anim.SetBool("FlyForward", false);
    }

}
