using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Ennemi
{
    [SerializeField] public CombatController Combat;
    [SerializeField] public EnnemiController Controller;
}

public class EnnemiManager : MonoBehaviour
{
    static EnnemiManager INSTANCE;

    [Header("Debug")]
    [SerializeField] List<Ennemi> EnnemisRegistered;
    [SerializeField] Ennemi[] AllEnnemis;

    int EnnemisTotalCount;

    List<Ennemi>[,] EnnemisByRoom;

    Vector2 RoomScale;
    Vector2Int RoomCount;
    bool IsInit = false;

    public int ENNEMIES_REGISTERED => EnnemisRegistered.Count;

    private void Awake()
    {
        if(INSTANCE == null)
        {
            INSTANCE = this;
        }
    }

    public void InitWithRoom(Vector2Int room_count, Vector2 room_scale)
    {
        RoomCount = room_count;
        RoomScale = room_scale;
        ForgetEveryRegistered();
        IsInit = true;
    }

    public void RegisterEnnemi( CombatController combat, EnnemiController controller )
    {
        if(EnnemisRegistered == null) EnnemisRegistered = new List<Ennemi>();

        Ennemi ennemi = new Ennemi();
        ennemi.Controller = controller;
        ennemi.Combat = combat;
        EnnemisRegistered.Add(ennemi);
    }

    public void InitWithRegistered()
    {
        AllEnnemis = EnnemisRegistered.ToArray();
        EnnemisTotalCount = AllEnnemis.Length;

        EnnemisByRoom = new List<Ennemi>[RoomCount.x, RoomCount.y];
        foreach (Ennemi ennemi in AllEnnemis) {
            int x_room = ennemi.Controller.InitRoom.x;
            int y_room = ennemi.Controller.InitRoom.y;

            if (EnnemisByRoom[x_room, y_room] == null) EnnemisByRoom[x_room, y_room] = new List<Ennemi>();
            EnnemisByRoom[x_room, y_room].Add(ennemi);
        }
    }

    public void ForgetEveryRegistered()
    {
        EnnemisRegistered.Clear();
        AllEnnemis = null;
        EnnemisTotalCount = 0;
        EnnemisByRoom = null;
    }

    public void KillEnnemi(Ennemi ennemi)
    {
        for (int i = 0; i < EnnemisTotalCount; i++)
        {
            if (AllEnnemis[i] == ennemi)
            {
                AllEnnemis[i] = AllEnnemis[EnnemisTotalCount - 1];
                EnnemisTotalCount--;
                break;
            }
        }

        EnnemisByRoom[ennemi.Controller.InitRoom.x, ennemi.Controller.InitRoom.y].Remove(ennemi);
    }

    public IEnumerable<Ennemi> EnumAllEnnemies()
    {
        for (int i = 0; i < EnnemisTotalCount; i++)
        {
            yield return AllEnnemis[i];
        }
    }

    public IEnumerable<Ennemi> EnumAllActiveEnnemies(int x, int y)
    {
        for (int dx = -1; dx <= 1; dx++) {
            for (int dy = -1; dy <= 1; dy++)
            {
                int X = x + dx;
                int Y = y + dy;
                if (X < 0 || Y < 0 || X >= RoomCount.x || Y >= RoomCount.y) continue;

                foreach(Ennemi ennmi in EnnemisByRoom[X, Y])
                {
                    yield return ennmi;
                }
            }
        }
    }

    public Ennemi GetClosestEnnemi(Vector3 position)
    {
        if(IsInit == false) return null;

        Vector2Int room = new Vector2Int((int)(position.x / RoomScale.y), (int)(position.z / RoomScale.y));

        float distance_sq_min = float.MaxValue;
        Ennemi closest = null;
        foreach (Ennemi ennemi in EnumAllActiveEnnemies(room.x, room.y))
        {
            Vector3 diff = position - ennemi.Controller.transform.position;
            float distance_sq = Vector3.Dot(diff, diff);
            if (distance_sq < distance_sq_min)
            {
                distance_sq_min = distance_sq;
                closest = ennemi;
            }

        }

        return closest;
    }



}
