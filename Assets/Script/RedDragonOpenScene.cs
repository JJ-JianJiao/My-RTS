using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedDragonOpenScene : MonoBehaviour
{
    public Transform circleCenter;

    // Start is called before the first frame update
    void Start()
    {
        //LeanTween.rotateAroundLocal(gameObject, circleCenter.position, 360f, 10f).setLoopPingPong();
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(circleCenter.position, Vector3.up, 0.1f);
    }
}
