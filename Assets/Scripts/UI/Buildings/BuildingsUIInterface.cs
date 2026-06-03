using UnityEngine;
using System.Collections;

public abstract class BuildingsUI : MonoBehaviour
{
    public abstract void Open(Building building);

    public abstract void Close();
}