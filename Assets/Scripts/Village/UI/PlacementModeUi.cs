using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlacementModeUi : MonoBehaviour
{
    [SerializeField] private VillageManager _villageManager;
    [SerializeField] private AudioSource _clickSouce;

    [SerializeField] private Sprite _Button;

    [FormerlySerializedAs("_container")]
    [Header("Shop")]
    [SerializeField] private GameObject _shopMenu;
    [SerializeField] private GameObject _buildingsList;
    [SerializeField] private GameObject _placementUiPrefab;

    [SerializeField] List<Vector3> _placementUiPositions;

    [Header("Confirmation menu")]
    [SerializeField]
    private GameObject _confirmationMenu;

    private void Start()
    {
        for (var i = 0; i < _villageManager.BuildingObjects.Count; ++i)
        {
            var buildInfos = _villageManager.BuildingObjects[i];
            if ( buildInfos.GlobalBuildingData.BaseBuildingCount <= 0)
                continue;
            GameObject obj = Instantiate(_placementUiPrefab, _buildingsList.transform);
            obj.GetComponent<RectTransform>().localPosition = _placementUiPositions[i];
            var componentInChildren = obj.GetComponentInChildren<ShopCase>();
            foreach (var resourcesCost in buildInfos.GlobalBuildingData.PerLevelUpgradeCost)
            {
                switch (resourcesCost.Type)
                {
                    case ResourceType.Wood:
                        componentInChildren.PriceWood.text = resourcesCost.Qte.ToString();
                        break;
                    case ResourceType.Leaves:
                        componentInChildren.PriceLeaves.text = resourcesCost.Qte.ToString();
                        break;
                    case ResourceType.Stone:
                        componentInChildren.PriceStone.text = resourcesCost.Qte.ToString();
                        break;
                }

            }

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
