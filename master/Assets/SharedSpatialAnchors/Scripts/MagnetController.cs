using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MagnetController : MonoBehaviour
{
    void Start()
    {
        // Find FieldLineRenderer in scene
        GameObject fieldLineRendererObj = GameObject.Find("FieldLineRenderer");
        if (fieldLineRendererObj != null)
        {
            FieldLineRenderer fieldLineRenderer = fieldLineRendererObj.GetComponent<FieldLineRenderer>();
            if (fieldLineRenderer != null)
            {
                // Disable the FieldLineRenderer script before making changes
                fieldLineRenderer.enabled = false;

                // Add this magnet to the magnets list in the FieldLineRenderer script
                fieldLineRenderer.magnets.Add(gameObject);
                SampleController.Instance.Log("Magnet Added");

                // Re-enable the FieldLineRenderer script
                fieldLineRenderer.enabled = true;
            }
            else
            {
                SampleController.Instance.Log("FieldLineRenderer component not found on the GameObject.");
            }
        }
        else
        {
            SampleController.Instance.Log("GameObject with FieldLineRenderer not found in the scene.");
        }
    }
}