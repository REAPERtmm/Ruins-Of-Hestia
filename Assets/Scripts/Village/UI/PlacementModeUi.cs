using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlacementModeUi : MonoBehaviour
{
    [SerializeField] private VillageManager _villageManager;

    [SerializeField] private GameObject _placementUiPrefab;
    private void Start()
    {
        Vector3 placementPosition = _placementUiPrefab.transform.position;
        for (var i = 0; i < _villageManager.BuildingObjects.Count; ++i)
        {
            var buildInfos = _villageManager.BuildingObjects[i];
            GameObject obj = Instantiate(_placementUiPrefab, transform);
            obj.GetComponentInChildren<Image>().sprite = buildInfos.Visual.Preview;
            obj.GetComponentInChildren<TMP_Text>().text = buildInfos.Name;

            var copyI = i;
            obj.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                _villageManager.StartPlacing(copyI);
            });
        }
    }
}
