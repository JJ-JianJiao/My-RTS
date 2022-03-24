using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapScript : MonoBehaviour
{
    public Transform firstCam;

    private void LateUpdate()
    {
        Vector3 newPosition = firstCam.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        //transform.rotation = Quaternion.Euler(90f, firstCam.eulerAngles.y, 0f);
        //transform.rotation = Quaternion.Euler(90f, firstCam.localEulerAngles.y, 0f);
        //transform.rotation = Quaternion.Euler(90f, firstCam.GetComponentInChildren<Transform>().localEulerAngles.y, 0f);
    }
}
