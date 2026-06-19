using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlacementModeUi : MonoBehaviour
{
    [SerializeField] private VillageManager _villageManager;
    [SerializeField] private AudioSource _clickSouce;

    [FormerlySerializedAs("_container")]
    [Header("Shop")]
    [SerializeField] private GameObject _shopMenu;
    [SerializeField] private GameObject _buildingsList;
    [SerializeField] private GameObject _placementUiPrefab;

    [SerializeField] private float _StartY = 50.0f;
    [SerializeField] private float _OffsetY = 125.0f;

    [Header("Confirmation menu")]
    [SerializeField]
    private GameObject _confirmationMenu;

    private void Start()
    {
        Vector3 placementPosition = new Vector3(0.0f, _StartY, 0.0f);
        for (var i = 0; i < _villageManager.BuildingObjects.Count; ++i)
        {
            var buildInfos = _villageManager.BuildingObjects[i];
            GameObject obj = Instantiate(_placementUiPrefab, _buildingsList.transform);
            obj.transform.position += placementPosition;
            placementPosition.y += _OffsetY;
            obj.GetComponentInChildren<Image>().sprite = buildInfos.Visual.Preview;
            obj.GetComponentInChildren<TMP_Text>().text = buildInfos.Name;

            var copyI = i;
            obj.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                _clickSouce.Play();
                _villageManager.StartPlacing(copyI);
            });
        }

        _villageManager.OnVillageLevelUp += UpdatePlacement;
    }

    public void UpdatePlacement(VillageManager villageManager)
    {
    }

    public void ShowShop()
    {
        _shopMenu.SetActive(true);
        _confirmationMenu.SetActive(false);
    }

    public void ShowConfirmation()
    {
        _confirmationMenu.SetActive(true);
        _shopMenu.SetActive(false);
    }

    public void HideAll()
    {
        _confirmationMenu.SetActive(false);
        _shopMenu.SetActive(false);
    }
}
