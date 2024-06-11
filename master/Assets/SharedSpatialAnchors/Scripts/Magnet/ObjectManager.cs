using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectManager
{
    public static RefManager refManager;

    public static GameObject ironFilling;
    public static GameObject MagnetScene;
    public static GameObject LargeMagnet;

    static ObjectManager()
    {
        refManager = Object.FindObjectOfType<RefManager>();

        MagnetScene = refManager.MagnetScene;
        ironFilling = refManager.ironFilling;
        LargeMagnet = refManager.LargeMagnet;
    }
}
