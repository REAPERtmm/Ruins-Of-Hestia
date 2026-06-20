using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Campfire : MonoBehaviour
{
    [SerializeField] PlayerController Player;
    [SerializeField] RectTransform PannelEndOfMission;

    [SerializeField] TransitionManager _TransitionManager;

    public void Continue( int scene )
    {
        _TransitionManager.LoadExpedition( Player.inventory, scene );
    }

    public void ReturnVillage()
    {
        Inventory.Instance.Add(Player.inventory);
        _TransitionManager.ReturnVillage();
    }


    void OpenPannel()
    {
        PannelEndOfMission.gameObject.SetActive(true);
    }

    void ClosePannel()
    {
        PannelEndOfMission.gameObject.SetActive(false);
    }

    private void Update()
    {

        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            Player.transform.position = transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            OpenPannel();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            ClosePannel();
        }
    }

}
