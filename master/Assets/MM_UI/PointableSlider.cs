using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Photon.Pun;
public class PointableSlider : MonoBehaviour
{
    public List<Transform> fingerTips;
    public Transform sliderKnob, panel;
    private PhotonView knobPV;
    public float minimumZ;
    [HideInInspector]
    public float x, y;
    private float xRange, yRange;
    public bool setX, setY;
    void Awake()
    {
        knobPV = sliderKnob.GetComponent<PhotonView>();
        fingerTips = new List<Transform>();
        StartCoroutine(acquireFingerTip("LeftIndexTip"));
        StartCoroutine(acquireFingerTip("RightIndexTip"));
        xRange = panel.transform.localScale.x / 2;
        yRange = panel.transform.localScale.y / 2;
    }

    void Update()
    {
        checkForFinger();
        setExternalValues();
    }
            
    private void checkForFinger()
    {
        if(fingerTips.Count == 0) { return; }
        foreach (Transform fingerTip in fingerTips)
        {
            Vector3 fingerTipLocalPosition = transform.InverseTransformPoint(fingerTip.position);

            if (fingerWithinDistance(fingerTipLocalPosition))
            {
                float xToSet = 0f, yToSet = 0f;

                if (setX)
                {
                    if(fingerTipLocalPosition.x > xRange)
                    {
                        xToSet = xRange;
                    }
                    else if(fingerTipLocalPosition.x < -xRange)
                    {
                        xToSet = -xRange;
                    }
                    else
                    {
                        xToSet = fingerTipLocalPosition.x;
                    }
                }
                if (setY)
                {
                    if (fingerTipLocalPosition.y > yRange)
                    {
                        yToSet = yRange;
                    }
                    else if (fingerTipLocalPosition.y < -yRange)
                    {
                        yToSet = -yRange;
                    }
                    else
                    {
                        yToSet = fingerTipLocalPosition.y;
                    }
                }

                if (knobPV.Owner != PhotonNetwork.LocalPlayer)
                {
                    knobPV.TransferOwnership(PhotonNetwork.LocalPlayer);
                }

                sliderKnob.transform.localPosition = new Vector3(xToSet, yToSet, 0f);               
            }
            SampleController.Instance.Log("Finger out of distance.");
        }
    }

    private bool fingerWithinDistance(Vector3 localPos)
    {
        float xBuffer = 0.3f;
        float yBuffer = 0.1f;
        if (Mathf.Abs(localPos.z * transform.lossyScale.z) > minimumZ) {return false;}
        if (Mathf.Abs(localPos.x) > xRange +xBuffer) {return false;}
        if (Mathf.Abs(localPos.y) > yRange + yBuffer) {return false;}
        return true;
    }

    private void setExternalValues()
    {        
        x = 0.5f* (sliderKnob.transform.localPosition.x/xRange) + 0.5f;
        y = 0.5f * (sliderKnob.transform.localPosition.y/yRange) + 0.5f;        
    }

    private IEnumerator acquireFingerTip(string fingerTag)
    {        
        while(GameObject.FindGameObjectWithTag(fingerTag) == null)
        {
            yield return null;
        }
        fingerTips.Add(GameObject.FindGameObjectWithTag(fingerTag).transform);
    }
}
