using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    private static TransitionManager _instance;
    private void Start()
    {
        if (_instance)
            return;

        _instance = this;
    }

    public void LoadLocalScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public static void LoadScene(int index)
    {
        _instance.LoadLocalScene(index);
    }
}
