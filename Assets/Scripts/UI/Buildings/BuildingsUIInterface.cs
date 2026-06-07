using UnityEngine;
using System.Collections;

public abstract class BuildingsUI : MonoBehaviour
{
    public abstract void Open(BuildingScript building);

    public abstract void Close(); 
}