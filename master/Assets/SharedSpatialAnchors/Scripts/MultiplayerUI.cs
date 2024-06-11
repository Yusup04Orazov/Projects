using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;
using TMPro;

public class MultiplayerUI : MonoBehaviour
{
    //UI buttons
    //1. Spawn Magnets
    //2. Hide/Show Earth
    //3. Hide/Show Solar Wind

    [SerializeField]
    private GameObject earthPrefab;

    [SerializeField]
    private GameObject magnetPrefab;

    [SerializeField]
    private FieldLineRenderer fieldLineRenderer;

    private List<GameObject> spawnedEarths = new List<GameObject>();

    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private Transform referencePoint;

    void Start()
    {
        transform.parent = referencePoint;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    void Update()
    {
        
    }

    public void OnSpawnEarthButtonPressed()
    {
        SampleController.Instance.Log("OnSpawnEarthButtonPressed");
        SpawnEarth();
    }

    public void OnSpawnMagnetButtonPressed()
    {
        SampleController.Instance.Log("OnSpawnMagnetButtonPressed");
        SpawnMagnet();
    }

    public void DeleteMagnet()
    {
        var fieldLineRendererComponent = fieldLineRenderer.GetComponent<FieldLineRenderer>();
        if (fieldLineRendererComponent != null && fieldLineRendererComponent.magnets.Count > 0)
        {
            foreach (var magnet in fieldLineRendererComponent.magnets)
            {
                // Destroy the magnet and remove it from the list
                PhotonPun.PhotonNetwork.Destroy(magnet);
                fieldLineRendererComponent.magnets.Clear();
            }
            SampleController.Instance.Log("Magnets Deleted");
        }
    }

    public void SpawnMagnet()
    {
        DeleteEarth();

        var networkedMagnet = PhotonPun.PhotonNetwork.Instantiate(magnetPrefab.name, spawnPoint.position, spawnPoint.rotation);
        var photonGrabbable = networkedMagnet.GetComponent<PhotonGrabbableObject>();
        photonGrabbable.TransferOwnershipToLocalPlayer();

    }

    public void DeleteEarth()
    {
        if (spawnedEarths.Count > 0)
        {
            foreach (var prefab in spawnedEarths)
            {
                PhotonPun.PhotonNetwork.Destroy(prefab);
            }
            spawnedEarths.Clear();
        }
        SampleController.Instance.Log("Earth Deleted");
    }

    public void SpawnEarth()
    {
        DeleteMagnet();
        DeleteEarth();

        var networkedEarth = PhotonPun.PhotonNetwork.Instantiate(earthPrefab.name, spawnPoint.position, spawnPoint.rotation);
        var photonGrabbable = networkedEarth.GetComponent<PhotonGrabbableObject>();
        photonGrabbable.TransferOwnershipToLocalPlayer();

        spawnedEarths.Add(networkedEarth);
    }
}
