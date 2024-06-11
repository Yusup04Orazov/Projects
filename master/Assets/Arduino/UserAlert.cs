using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;


public class UserAlert : MonoBehaviour
{
    private IEnumerator coroutine;

  
    public void displayMessage(string message)
    {
        coroutine = writeMessage(message);
        StartCoroutine(coroutine);
    }

    private IEnumerator writeMessage(string message)
    {        
        GetComponentInParent<TextMeshProUGUI>().text = message;
        yield return new WaitForSeconds(5);
    }
}
