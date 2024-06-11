using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerTest : MonoBehaviour
{
    public Transform fingertip;
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = fingertip.position; 
        gameObject.transform.rotation = fingertip.rotation; 
    }
}
