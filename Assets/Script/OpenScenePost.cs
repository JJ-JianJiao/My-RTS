using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenScenePost : MonoBehaviour
{
    private float timeStamp;
    public float timeRate = 10;
    private Animator anim;
    private bool isGuard;

    private void Start()
    {
        isGuard = true;
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Time.time > timeStamp + timeRate) {
            isGuard = !isGuard;
            if (isGuard)
            {
                SetAnimShootPos();
                //LeanTween.rotateY(gameObject, -54f, 0.2f).setOnComplete(SetAnimShootPos);

            }
            else {
                SetAnimIdlePos();
                //LeanTween.rotateY(gameObject, -18f, 0.2f).setDelay(1f);

            }
            timeStamp = Time.time;
        }
    }

    private void SetAnimShootPos() {
        anim.SetBool("Idle_Shoot", true);
    }

    private void SetAnimIdlePos() {
        anim.SetBool("Idle_Shoot", false);

    }
}
