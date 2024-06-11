using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class UI : MonoBehaviour
{
    public GameObject earth;

    [SerializeField]
    private FieldLineRenderer fieldLineRenderer;

    [SerializeField]
    private GameObject magnetPrefab;

    private List<GameObject> spawnedMagnets = new List<GameObject>();


    private void Start()
    {
        earth.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("space key was pressed");
            ShowEarth();
        }
    }

    public void ShowEarth()
    {
        DeleteMagnets();
        earth.SetActive(true);
    }

    private void HideEarth()
    {
        earth.SetActive(false);
    }

    private void DeleteMagnets()
    {
        fieldLineRenderer.GetComponent<FieldLineRenderer>();
        fieldLineRenderer.magnets.Clear();
        if (spawnedMagnets.Count > 0)
        {
            foreach (var prefab in spawnedMagnets)
            {
                Destroy(prefab);
            }
            spawnedMagnets.Clear();
        }
    }

    public void SpawnMagnet()
    {
        HideEarth();
        GameObject newMagnet = Instantiate(magnetPrefab, transform.position, Quaternion.identity);

        // Get the FieldLineRenderer script attached to the FieldLineRenderer game object
        fieldLineRenderer.GetComponent<FieldLineRenderer>();
        // Disable the FieldLineRenderer script
        fieldLineRenderer.GetComponent<FieldLineRenderer>().enabled = false;

        if (fieldLineRenderer != null)
        {
            // Add the networkedMagnet to the magnets list in the FieldLineRenderer script
            fieldLineRenderer.magnets.Add(newMagnet);
            spawnedMagnets.Add(newMagnet);
            // Enable the FieldLineRenderer script
            fieldLineRenderer.GetComponent<FieldLineRenderer>().enabled = true;
        }
        else
        {
            Debug.Log("fieldLineRenderer component not found on the GameObject.");
        }

    }

}