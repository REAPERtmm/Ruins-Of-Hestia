using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Campfire : MonoBehaviour
{
    [SerializeField] PlayerController Player;
    [SerializeField] RectTransform PannelEndOfMission;

    [SerializeField] TransitionManager _TransitionManager;

    [SerializeField] Image CampFireMiniMapUI;
    [SerializeField] MapGeneration MapGenerationManager;

    public Vector2 NormalizedCampFirePositionInMap
    {
        get
        {
            if (MapGenerationManager == null) return new Vector2(transform.position.x, transform.position.z);
            return new Vector2(transform.position.x / MapGenerationManager.GenerationSizeX, transform.position.z / MapGenerationManager.GenerationSizeY);
        }
    }

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

        if (MapGenerationManager == null)
        {
            return;
        }

        Vector2 normalized_player_position = NormalizedCampFirePositionInMap;
        Vector2 centered = normalized_player_position - Vector2.one * 0.5f;

        const float RECT_SIZE = 360;
        const float RECT_SCALE = 1.0f;
        const float RECT_RESCALED = RECT_SIZE * RECT_SCALE;

        CampFireMiniMapUI.rectTransform.localPosition = centered * RECT_RESCALED;
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
