using System;
using UnityEngine;

[Serializable]
public class ResourceLoot
{
    public ResourceType resourceType;
    public int amount;
    [Range(0, 1)] public float probability = 1.0f;
}


public class ResourceController : MonoBehaviour
{
    public ResourceLoot[] LootByHit;
    public ResourceDescriptor DescriptorReference;
    [SerializeField] int HitBeforeDestroy;
    [SerializeField] ResourceManager Manager;

    private void Awake()
    {
        Manager = MapGeneration.INSTANCE.resource_manager;
    }

    public void DecrementHit()
    {
        HitBeforeDestroy--;
        if(HitBeforeDestroy < 0)
        {
            Manager.RemoveResource(this);
        }
    }
}
