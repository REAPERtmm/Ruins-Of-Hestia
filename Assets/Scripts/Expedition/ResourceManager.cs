using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourceDescriptor
{
    [SerializeField] public Vector2Int Room;
    [SerializeField] public Transform ResourceTransform;
    [SerializeField] public ResourceController Controller;
}

public class ResourceManager : MonoBehaviour
{
    [Header("ResourcePrefabs")]
    [SerializeField] GameObject WoodResource;
    [SerializeField] GameObject StoneResource;
    [SerializeField] GameObject MetalResource;
    [SerializeField] GameObject FiberResource;

    bool IsInit = false;
    [SerializeField] List<ResourceDescriptor>[,] ResourceByRoom;
    [SerializeField] Vector2 RoomScale;
    [SerializeField] Vector2Int RoomCount;

    public void InitRooms(Vector2Int room_count, Vector2 room_scale)
    {
        RoomScale = room_scale;
        RoomCount = room_count;
        ResourceByRoom = new List<ResourceDescriptor>[room_count.x, room_count.y];
        for (int x = 0; x < room_count.x; x++) {
            for (int y = 0; y < room_count.y; y++)
            {
                ResourceByRoom[x, y] = new List<ResourceDescriptor>();
            }
        }
        IsInit = true;
    }

    public Transform AppendResourceInRoom(Vector3 position, Transform where)
    {
        ResourceType type = (ResourceType)UnityEngine.Random.Range(0, 4);

        GameObject instance;
        if (type == ResourceType.Stone)
        {
            instance = Instantiate(StoneResource, where);
        }
        else if(type == ResourceType.Leaves)
        {
            instance = Instantiate(FiberResource, where);
        }
        else if (type == ResourceType.Wood)
        {
            instance = Instantiate(WoodResource, where);
        }
        else if (type == ResourceType.Metal)
        {
            instance = Instantiate(MetalResource, where);
        }
        else
        {
            Debug.LogWarning("Unsupported ResourceType");
            return null;
        }
        instance.transform.position = position;

        Vector2Int room = new Vector2Int(
            (int)(position.x / RoomScale.x),
            (int)(position.z / RoomScale.y)
        );

        ResourceDescriptor resourceDescriptor = new ResourceDescriptor();
        resourceDescriptor.Room = room;
        resourceDescriptor.ResourceTransform = instance.transform;
        resourceDescriptor.Controller = instance.transform.GetComponent<ResourceController>();
        resourceDescriptor.Controller.DescriptorReference = resourceDescriptor;

        ResourceByRoom[room.x, room.y].Add(resourceDescriptor);


        return instance.transform;
    }

    public void RemoveResource(ResourceController resourceController)
    {
        Destroy(resourceController.gameObject);
        ResourceByRoom[resourceController.DescriptorReference.Room.x, resourceController.DescriptorReference.Room.y].Remove(resourceController.DescriptorReference);
    }

    public ResourceDescriptor GetClosestResource(Vector3 position)
    {
        if(IsInit == false)
        {
            Debug.Log("Resource Manager not Initiated");
            return null;
        }

        Vector2Int room = new Vector2Int((int)(position.x / RoomScale.x), (int)(position.z / RoomScale.y));

        float distance_sq_min = float.MaxValue;
        ResourceDescriptor closest = null;
        foreach (ResourceDescriptor resourceDescriptor in GetAllResourceInRoom(room))
        {
            Vector3 diff = position - resourceDescriptor.ResourceTransform.position;
            float distance_sq = Vector3.Dot(diff, diff);
            if (distance_sq < distance_sq_min) { 
                distance_sq_min = distance_sq;
                closest = resourceDescriptor;
            }

        }

        return closest;
    }

    public IEnumerable<ResourceDescriptor> GetAllResourceInRoom(Vector2Int room)
    {
        foreach(ResourceDescriptor resourceDescriptor in ResourceByRoom[room.x, room.y])
        {
            yield return resourceDescriptor;
        }
    } 

}
