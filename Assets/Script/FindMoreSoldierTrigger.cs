using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMoreSoldierTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("Soldier"))
        {
            //TutorialManager.instance.findMoreSoldier = true;
            if (TutorialManager.instance.index == 5 && !TutorialManager.instance.findMoreSoldierTutorial)
            {
                TutorialManager.instance.findMoreSoldier = true;
            }
        }
    }
}
