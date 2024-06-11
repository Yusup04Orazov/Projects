using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Oculus.Interaction;
using System.Collections.Generic;
using Photon.Pun;

/// <summary>
/// Main manager for sample interaction
/// </summary>
public class PanelController : MonoBehaviour
{
    public bool automaticCoLocation = false;
    public bool cachedAnchorSample = false;

    [HideInInspector]
    public CachedSharedAnchor colocationCachedAnchor;

    [SerializeField]
    public TextMeshProUGUI logText;

    public static PanelController Instance;
    private bool _isPlacementMode;
    public int lineLimit;
    private int lineCount;
    private List<string> logLines;

    private List<SharedAnchor> sharedanchorList = new List<SharedAnchor>();

    private RayInteractor _rayInteractor;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        lineCount = 0;
        logLines = new List<string>();
        //to test maximum lines available, run this while also changing the linelimit in the editor
        /*for (int i=0; i<100; i++)
        {
            Log(i.ToString());
        }*/

        //colocationAnchor = AnchorMenu.instance.anchor.GetComponent<SharedAnchor>();

    }


    public void Log(string message, bool error = false)
    {
        if (logText == null)
        {
            return;
        }
        lineCount++;
        logLines.Add(message + "\n");
        if (logLines.Count > lineLimit)
        {
            logLines.RemoveAt(0);
        }
        string textToDisplay = "";
        foreach (string line in logLines)
        {
            textToDisplay += line;
        }
        logText.text = textToDisplay;

        //logText.pageToDisplay = SampleController.Instance.logText.textInfo.pageCount;

        // Console logging (goes to logcat on device)

        const string anchorTag = "SpatialAnchorsUnity: ";
        if (error)
            Debug.LogError(anchorTag + message);
        else
            Debug.Log(anchorTag + message);

        //pageText.text = SampleController.Instance.logText.pageToDisplay + "/" + logText.textInfo.pageCount;
    }

    public void LogError(string message)
    {
        Log(message, true);
    }

    public void AddSharedAnchorToLocalPlayer(SharedAnchor anchor)
    {
        sharedanchorList.Add(anchor);
    }

    public List<SharedAnchor> GetLocalPlayerSharedAnchors()
    {
        return sharedanchorList;
    }
}
