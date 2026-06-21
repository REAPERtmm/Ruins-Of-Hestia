using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    private static TransitionManager _instance;
    private static Inventory LastInventory;
    private static VillageSaver LastVillage;

    private void Start()
    {
        _instance = this;
    }

    public void LoadVillage()
    {
        SceneManager.LoadScene(1);
    }

    public void ReturnVillage()
    {
        SceneManager.LoadScene(3);
        LastVillage.gameObject.SetActive(true);
    }

    public void LoadExpedition( int scene = 2 )
    {
        LastVillage = FindFirstObjectByType<VillageSaver>();
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        LastVillage.gameObject.SetActive(false);
    }

    public void LoadExpedition( Inventory inventory, int scene = 2 )
    {
        if ( inventory != null)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            LastInventory = inventory;
            SceneManager.LoadScene(scene, LoadSceneMode.Single);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var root in scene.GetRootGameObjects())
        {
            Inventory controller = root.GetComponentInChildren<Inventory>();
            if (controller== null)
                continue;
            controller.Add(LastInventory);
            SceneManager.sceneLoaded -= OnSceneLoaded;
            break;
        }

    }

    public void Quit()
    {
        Application.Quit();
    }
}
