

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using PhotonRealtime = Photon.Realtime;

public class AnchorMenu : MonoBehaviour
{
    private PhotonView pv;
    private bool anchorShared;
    public static AnchorMenu instance;
    public GameObject placement, shareButton, projectPrefab;
    public OVRSpatialAnchor anchorPrefab;
    public SharedAnchor sharedAnchorScript;
    private void Start()
    {
        pv = gameObject.GetComponent<PhotonView>();
        anchorShared = false;
        instance = this;
    }

    
    public void shareAnchor()
    {
        // don't let someone try to share a new anchor
        if (anchorShared)
        {
            SampleController.Instance.Log("Anchor already shared");
            return;
        }        

        sharedAnchorScript = Instantiate(anchorPrefab, placement.transform.position, placement.transform.rotation).GetComponent<SharedAnchor>();
        PhotonNetwork.Instantiate(projectPrefab.name, placement.transform.position, placement.transform.rotation);

        someoneSharedAnchor();
        pv.RPC(nameof(someoneSharedAnchor), RpcTarget.OthersBuffered);

        //commence sharing mechanism
        StartCoroutine(localizeAnchorThenShare());
    }

    private System.Collections.IEnumerator localizeAnchorThenShare()
    {
        SampleController.Instance.Log("localizeAnchorThenShare");
        while (!sharedAnchorScript.GetComponent<OVRSpatialAnchor>().Localized)
        {
            SampleController.Instance.Log(nameof(localizeAnchorThenShare) + "...");
            yield return null;
        }

        SampleController.Instance.Log($"{nameof(localizeAnchorThenShare)}: Anchor Localized");
        sharedAnchorScript.OnShareButtonPressed();
    }

    [PunRPC]
    private void someoneSharedAnchor()
    {
        anchorShared = true;
        Destroy(placement);
        shareButton.SetActive(false);
    }
    
    public void snapProjectToAnchor()
    {
        SampleController.Instance.Log("Snap Called");
        bool reject = false;

        if (GameObject.FindGameObjectWithTag("ProjectRoot") == null)
        {
            SampleController.Instance.Log("Project Null");
            reject = true;
        }
        if (GameObject.FindGameObjectWithTag("Anchor") == null)
        {
            SampleController.Instance.Log("Anchor Null");
            reject = true;
        }
        if (reject)
        {
            return;
        }
        GameObject.FindGameObjectWithTag("ProjectRoot").transform.position = GameObject.FindGameObjectWithTag("Anchor").transform.position;
        GameObject.FindGameObjectWithTag("ProjectRoot").transform.rotation = GameObject.FindGameObjectWithTag("Anchor").transform.rotation;
    }
}
