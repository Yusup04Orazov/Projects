using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableMotionConstrain : MonoBehaviour
{
    private Quaternion startRotation;
    private Vector3 startPosition;
    private float[] eulers;
    public bool fixHeight, YrotationOnly;
    void Start()
    {
        startRotation = gameObject.transform.rotation;
        startPosition = gameObject.transform.position;
        eulers = new float[2];
        eulers[0] = startRotation.eulerAngles.x;
        eulers[1] = startRotation.eulerAngles.z;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (YrotationOnly)
        {
            float rotationY = gameObject.transform.eulerAngles.y;
            gameObject.transform.rotation = Quaternion.Euler(eulers[0], rotationY, eulers[1]);
        }
        
        if (fixHeight)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, startPosition.y, gameObject.transform.position.z);
        }
    }
}
