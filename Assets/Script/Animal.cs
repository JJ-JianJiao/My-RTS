using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum AnimalState
//{
//    Move,
//    Stay
//}

public class Animal
{
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
    protected float actionFrequency = 5;
    [SerializeField]
    protected AnimalState animalState = AnimalState.Stay;

    // Start is called before the first frame update
    void Start()
    {
        xMin = LeftObj.position.x;
        xMax = RightObj.position.x;
        zMin = FrontObj.position.z;
        zMax = BackObj.position.z;
        
    }

}
