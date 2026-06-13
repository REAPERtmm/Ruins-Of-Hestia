using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlacementModeUi : MonoBehaviour
{
    [SerializeField] private VillageManager _villageManager;

    [SerializeField] private GameObject _placementUiPrefab;

    [SerializeField] private float _StartY = 50.0f;
    [SerializeField] private float _OffsetY = 125.0f;
    private void Start()
    {
        Vector3 placementPosition = new Vector3(0.0f, _StartY, 0.0f);
        for (var i = 0; i < _villageManager.BuildingObjects.Count; ++i)
        {
            var buildInfos = _villageManager.BuildingObjects[i];
            GameObject obj = Instantiate(_placementUiPrefab, transform);
            obj.transform.position += placementPosition;
            placementPosition.y += _OffsetY;
            obj.GetComponentInChildren<Image>().sprite = buildInfos.Visual.Preview;
            obj.GetComponentInChildren<TMP_Text>().text = buildInfos.Name;

            var copyI = i;
            obj.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                _villageManager.StartPlacing(copyI);
            });
        }

        _villageManager.OnVillageLevelUp += UpdatePlacement;
    }

    public void UpdatePlacement(VillageManager villageManager)
    {
    }
}
