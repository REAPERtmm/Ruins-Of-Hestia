using System;
using System.Collections.Generic;
using UnityEngine;

public class UiCarnet : MonoBehaviour
{
    [SerializeField] VillageManager villageManager;
    [SerializeField] GameObject previsionVisualizer;

    [SerializeField] Sprite noProduction;

    private Dictionary<string, BuildingProduction> mCurrentProduction = new();
    private List<GameObject> mGeneratedVizualiser = new();

    public void OnEnable()
    {
        foreach (var script in villageManager.Grid.BuildingScripts)
        {

            foreach (var production in script.GetCurrentProduction())
            {
                if (!mCurrentProduction.TryGetValue(production.Name, out var value))
                    mCurrentProduction[production.Name] = production;
                else
                    value.Quantity += production.Quantity;
            }
        }

        Vector3 position = new Vector3( 250f, 41f, 0f );
        foreach (var buildingProduction in mCurrentProduction.Values)
        {
            AddVisualiser(position, buildingProduction);
            position.y -= 95.0f;
        }

        if (mGeneratedVizualiser.Count == 0)
        {
            AddVisualiser( position, new  BuildingProduction()
            {
                Quantity = 0,
                Name = "No production",
                Icon =  noProduction,
                DayLeft =  0
            });
        }
    }

    public void AddVisualiser( Vector3 position, BuildingProduction production)
    {
        GameObject prevision = Instantiate(previsionVisualizer, transform);
        ProductionUI ui = prevision.GetComponent<ProductionUI>();
        prevision.transform.localPosition = position;
        ui.SetProductionUI(production);
        mGeneratedVizualiser.Add(prevision);
    }

    public void OnDisable()
    {
        foreach (var o in mGeneratedVizualiser)
        {
            Destroy(o);
        }
        mGeneratedVizualiser.Clear();
    }
}
