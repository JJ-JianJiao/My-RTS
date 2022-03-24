using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [SerializeField]
    private GameObject tutorialPanel;

    [SerializeField]
    private List<GameObject> tutorialTexts = new List<GameObject>();
    public int index = 0;

    //movement tutorial
    public bool MoveF, MoveB, MoveL, MoveR;
    public bool isDoneMoveTutorial;

    //Rotate tutorial
    public bool RotateL, RotateR;
    public bool isDoneRotateTutorial;

    //Zoom Tutorial
    public bool ZoomIn, ZoomOut;
    public bool isDoneZoomTutorial;

    //Left Click Tutorial
    public bool leftClickTutorial;
    public bool leftClick;

    //move unit Tutorial
    public bool moveUnitTutorial;
    public bool moveUnit;

    //Find more Soldier
    public bool findMoreSoldierTutorial;
    public bool findMoreSoldier;

    //Select All
    public bool selectAllTutorial;
    public bool selectAll;
    private bool isNotTweening = false;

    private void Awake()
    {
        instance = this;
        MoveF = false;
        MoveB = false;
        MoveL = false;
        MoveR = false;
        isDoneMoveTutorial = false;

        RotateL = false;
        RotateR = false;
        isDoneRotateTutorial = false;

        ZoomIn = false;
        ZoomOut = false;
        isDoneZoomTutorial = false;

        leftClick = false;
        leftClickTutorial = false;

        moveUnit = false;
        moveUnitTutorial = false;

        findMoreSoldier = false;
        findMoreSoldierTutorial = false;

        selectAll = false;
        selectAllTutorial = false;
    }

    private void LateUpdate()
    {
        if (index >= 0)
        {
            if (index >= tutorialTexts.Count)
            {
                index = -1;
                return;
            }

            if (!isDoneMoveTutorial)
            {
                if (MoveR && MoveF && MoveB && MoveL)
                {
                    isDoneMoveTutorial = true;
                    Invoke("ActiveNextTutorial", 2f);
                }
            }
            else if (!isDoneRotateTutorial)
            {
                if (RotateL && RotateR)
                {
                    isDoneRotateTutorial = true;
                    Invoke("ActiveNextTutorial", 2f);
                }
            }
            else if (!isDoneZoomTutorial)
            {
                if (ZoomOut && ZoomIn)
                {
                    isDoneZoomTutorial = true;
                    Invoke("ActiveNextTutorial", 2f);
                }
            }
            else if (!leftClickTutorial)
            {
                if (leftClick)
                {
                    leftClickTutorial = true;
                    Invoke("ActiveNextTutorial", 2f);
                }
            }
            else if (!moveUnitTutorial)
            {
                if (moveUnit)
                {
                    moveUnitTutorial = true;
                    Invoke("ActiveNextTutorial", 2f);
                }
            }
            else if (!findMoreSoldierTutorial)
            {
                if (findMoreSoldier)
                {
                    findMoreSoldierTutorial = true;
                    Invoke("ActiveNextTutorial", 2f);
                }
            }
            else if (!selectAllTutorial)
            {
                if (selectAll)
                {
                    selectAllTutorial = true;
                    Invoke("ActiveNextTutorial", 2f);
                }
            }
        }
        else {
            if (!isNotTweening && tutorialPanel.activeInHierarchy)
            {
                Debug.Log("finish the tutorial");
                isNotTweening = true;
                LeanTween.moveX(tutorialPanel, -600f, 5f).setDelay(3f).setOnComplete(NoNeedTutorialPanel);
            }
        }
    }

    private void ActiveNextTutorial() {
        tutorialTexts[index].GetComponent<TMP_Text>().color = Color.green;
        if ((index+1) < tutorialTexts.Count) {
            tutorialTexts[++index].gameObject.SetActive(true);
            return;
        }
        index++;
    }


    public void NeedTutorialPanel()
    {
        tutorialPanel.SetActive(true);
    }

    public void NoNeedTutorialPanel()
    {
        index = -1;
        tutorialPanel.SetActive(false);
    }
}
