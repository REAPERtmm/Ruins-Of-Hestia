using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourceDescriptor
{
    [SerializeField] public Vector2Int Room;
    [SerializeField] public Transform ResourceTransform;
}

public class ResourceManager : MonoBehaviour
{

    List<ResourceDescriptor>[,] ResourceByRoom;
    Vector2 RoomScale;
    Vector2Int RoomCount;

    public void InitRooms(Vector2Int room_count, Vector2 room_scale)
    {
        RoomScale = room_scale;
        RoomCount = room_count;
        ResourceByRoom = new List<ResourceDescriptor>[room_count.x, room_count.y];
    }

    public void AppendResourceInRoom(Transform transform)
    {
        Vector2Int room = new Vector2Int((int)(transform.position.x / RoomScale.x), (int)(transform.position.z / RoomScale.y));

        ResourceDescriptor resourceDescriptor = new ResourceDescriptor();
        resourceDescriptor.Room = room;
        resourceDescriptor.ResourceTransform = transform;

        ResourceByRoom[room.x, room.y].Add(resourceDescriptor);   
    }

    public IEnumerable<ResourceDescriptor> GetAllResourceInRoom(Vector2Int room)
    {
        foreach(ResourceDescriptor resourceDescriptor in ResourceByRoom[room.x, room.y])
        {
            yield return resourceDescriptor;
        }
    } 

}
