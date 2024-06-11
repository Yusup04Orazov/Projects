using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private float coordIncrement = 0.2f;
    private float rotIncrement = 1f;
    
    public void DecreaseX() { ObjectManager.MagnetScene.transform.position = new Vector3(ObjectManager.MagnetScene.transform.position.x - coordIncrement, ObjectManager.MagnetScene.transform.position.y, ObjectManager.MagnetScene.transform.position.z); }
    public void DecreaseY() { ObjectManager.MagnetScene.transform.position = new Vector3(ObjectManager.MagnetScene.transform.position.x, ObjectManager.MagnetScene.transform.position.y - coordIncrement, ObjectManager.MagnetScene.transform.position.z); }
    public void DecreaseZ() { ObjectManager.MagnetScene.transform.position = new Vector3(ObjectManager.MagnetScene.transform.position.x, ObjectManager.MagnetScene.transform.position.y, ObjectManager.MagnetScene.transform.position.z - coordIncrement); }
    public void DecreaseXRot() { ObjectManager.MagnetScene.transform.rotation = ObjectManager.MagnetScene.transform.rotation * Quaternion.AngleAxis(-rotIncrement, ObjectManager.MagnetScene.transform.right); }
    public void DecreaseYRot() { ObjectManager.MagnetScene.transform.rotation = ObjectManager.MagnetScene.transform.rotation * Quaternion.AngleAxis(-rotIncrement, ObjectManager.MagnetScene.transform.up); }
    public void DecreaseZRot() { ObjectManager.MagnetScene.transform.rotation = ObjectManager.MagnetScene.transform.rotation * Quaternion.AngleAxis(-rotIncrement, ObjectManager.MagnetScene.transform.forward); }

    public void IncreaseX() { ObjectManager.MagnetScene.transform.position = new Vector3(ObjectManager.MagnetScene.transform.position.x + coordIncrement, ObjectManager.MagnetScene.transform.position.y, ObjectManager.MagnetScene.transform.position.z); }
    public void IncreaseY() { ObjectManager.MagnetScene.transform.position = new Vector3(ObjectManager.MagnetScene.transform.position.x, ObjectManager.MagnetScene.transform.position.y + coordIncrement, ObjectManager.MagnetScene.transform.position.z); }
    public void IncreaseZ() { ObjectManager.MagnetScene.transform.position = new Vector3(ObjectManager.MagnetScene.transform.position.x, ObjectManager.MagnetScene.transform.position.y, ObjectManager.MagnetScene.transform.position.z + coordIncrement); }
    public void IncreaseXRot() { ObjectManager.MagnetScene.transform.rotation = ObjectManager.MagnetScene.transform.rotation * Quaternion.AngleAxis(rotIncrement, ObjectManager.MagnetScene.transform.right); }
    public void IncreaseYRot() { ObjectManager.MagnetScene.transform.rotation = ObjectManager.MagnetScene.transform.rotation * Quaternion.AngleAxis(rotIncrement, ObjectManager.MagnetScene.transform.up); }
    public void IncreaseZRot() { ObjectManager.MagnetScene.transform.rotation = ObjectManager.MagnetScene.transform.rotation * Quaternion.AngleAxis(rotIncrement, ObjectManager.MagnetScene.transform.forward); }

    public void SpawnNewMagnet()
    {
        GameObject newMagnet = Instantiate(ObjectManager.LargeMagnet, gameObject.transform.position, Quaternion.identity);
        Magnet magnet = newMagnet.GetComponent<Magnet>();
        Debug.Log("Spawned Magnet");
    }

    public void HideModel()
    {
        ObjectManager.ironFilling.SetActive(false);
    }
    public void ShowModel()
    {
        ObjectManager.ironFilling.SetActive(true);
    }

    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpawnNewMagnet();
        }
    }

    void Update()
    {
        CheckInput();
    }

}
