using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class GrabCounter : MonoBehaviour
{
    protected Grabbable grabbable;
    private int count;
    void Start()
    {
        grabbable = gameObject.GetComponent<Grabbable>();
        count = 200;
    }

    // Update is called once per frame
    void Update()
    {
        if(count != grabbable.SelectingPointsCount)
        {
            count = grabbable.SelectingPointsCount;
            SampleController.Instance.Log("SelectingPointsCount: "+count.ToString());
        }        
    }
}
