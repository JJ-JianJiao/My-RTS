using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialOrNotManager : MonoBehaviour
{

    private Animator anim;

    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    public void ChooseYesTutorial() {
        anim.SetTrigger("Yes");
        //anim.Play("ChooseYesTutorial");
    }

    public void ChooseNoTutorial()
    {
        anim.SetTrigger("No");
    }

    public void EnableTutorialPanel() {
        TutorialManager.instance.NeedTutorialPanel();
        gameObject.SetActive(false);
    }
    public void DisableTutorialChoosePanel() {
        TutorialManager.instance.NoNeedTutorialPanel();
        gameObject.SetActive(false);
    }
}
