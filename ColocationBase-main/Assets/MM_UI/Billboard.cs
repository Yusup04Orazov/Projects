using UnityEngine;

public class Billboard : MonoBehaviour
{
    private float currentAngle;
    public float cutoffAngle;
    [Range(1f, 20f)]
    public float smoothingFactor = 1f;
    public Transform targetTransform;
    public string targetTag;
    private Camera mainCamera;
    private Vector3 z = new Vector3(0, 0, 1);

    void Start()
    {
        if (targetTransform == null)
        {
            targetTransform = GameObject.FindGameObjectWithTag(targetTag).transform;
        }        
    }
    void LateUpdate()
    {
        Vector3 lookDirection = transform.position - targetTransform.position;
        Vector3 objectNormal = gameObject.transform.rotation * z;

        currentAngle = Vector3.Angle(lookDirection, objectNormal);
        
        if (currentAngle > cutoffAngle)
        {
            float difference = currentAngle - cutoffAngle;
            Vector3 newDirection = Vector3.RotateTowards(objectNormal, lookDirection, difference*Time.deltaTime/smoothingFactor, 0.1f);            
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }


}