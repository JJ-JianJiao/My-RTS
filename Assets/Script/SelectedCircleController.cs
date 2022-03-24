using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCircleController : MonoBehaviour
{

    public GameObject selectedCircle;
    public void EnableSelectedCircle()
    {
        selectedCircle.SetActive(true);
    }

    public void DisableSelectedCircle()
    {
        selectedCircle.SetActive(false);
    }
}
