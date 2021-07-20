using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController GC;
    public GameObject me;

    void Awake()
    {
        if (GC == null)
        {
            GC = this;
        }
    }
}
