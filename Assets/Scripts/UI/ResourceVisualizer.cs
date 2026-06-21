using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ResourceVisualizer : MonoBehaviour
{
    [Header("References")]
    public TMP_Text ResourceQte;
    [SerializeField] private ResourceType resourceType              = ResourceType.Wood;

    [Header("Debug")]
    [SerializeField] private bool UPDATE = true;

    void Update()
    {
        if(UPDATE && Inventory.Instance)
        {
            UpdateVisualizer(Inventory.Instance.GetResourceAmount(resourceType));
        }
    }

    public void UpdateVisualizer(int qte)
    {
        UpdateQte(qte);
    }

    public void UpdateQte(int qte)
    {
        ResourceQte.text = qte.ToString();
    }

}
