using System;
using UnityEngine;

public class VillageSaver : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(this);
    }
}
