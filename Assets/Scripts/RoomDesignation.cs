using UnityEngine;

public class RoomDesignation
{
    public Vector2Int position;
    public GameObject room;

    public bool isRoom => (room != null && room.GetComponent<Room>() != null);

    public RoomDesignation(Vector2Int position, GameObject room)
    {
        this.position = position;
        this.room = room;
    }
}