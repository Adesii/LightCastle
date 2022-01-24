using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    public GameObject MiniMapCanvas;
    public GameObject RoomPrefab;
    public GameObject MiniMapAnchor;

    private Dictionary<Vector2Int, RoomCombo> MiniMapObjects = new Dictionary<Vector2Int, RoomCombo>();

    private struct RoomCombo
    {
        public GameObject Room;
        public GameObject RoomMiniMap;
    }
    // Start is called before the first frame update
    void Start()
    {
        ZeroPosition = MiniMapAnchor.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.RoomGrid.Count == 0) return;
        var comp = GetComponentInParent<Canvas>();
        float CanvasScale = comp.transform.localScale.x * comp.scaleFactor * 0.5f;
        foreach (var room in GameManager.RoomGrid)
        {
            if (!MiniMapObjects.ContainsKey(room.Key))
            {
                var prefab = room.Value.room?.GetComponent<Room>()?.MiniMapRoomPrefab;
                if (prefab == null) continue;
                var obj = Instantiate(prefab, MiniMapAnchor.transform.position + (new Vector3(-room.Key.y, room.Key.x, 0) * 40 * comp.scaleFactor), Quaternion.identity, MiniMapCanvas.transform);
                MiniMapObjects.Add(room.Key, new RoomCombo() { Room = room.Value.room, RoomMiniMap = obj });
                //obj.transform.localScale = new Vector3(CanvasScale*0.5f, CanvasScale*0.5f, CanvasScale*0.5f);
            }
        }

        foreach (var room in MiniMapObjects)
        {
            //Move the room to the correct position relativ to the anchor
            room.Value.RoomMiniMap.transform.position = MiniMapAnchor.transform.position + (new Vector3(-room.Key.y, room.Key.x, 0) * 40 * comp.scaleFactor);
            room.Value.RoomMiniMap.transform.rotation = Quaternion.Euler(0, 180, room.Value.Room.transform.rotation.eulerAngles.y);
            //room.Value.RoomMiniMap.transform.localScale = new Vector3(CanvasScale, CanvasScale, CanvasScale);
            //room.Value.RoomMiniMap.transform.localScale = new Vector3(1, -1, 1);

        }
    }

    public Vector3 ZeroPosition;

    public RectTransform MiniMapRect;

    public Canvas ownCamvas;

    internal void SetRoom(Room r)
    {
        if (r == null || r.RoomDesignation == null) return;
        if (MiniMapRect == null)
        {
            MiniMapRect = MiniMapCanvas.GetComponent<RectTransform>();
            ownCamvas = GetComponentInParent<Canvas>();
        }


        MiniMapAnchor.transform.position = MiniMapRect.position + (new Vector3(r.RoomDesignation.position.y, -r.RoomDesignation.position.x, 0) * 40 * ownCamvas.scaleFactor);

    }

    internal void clearDict()
    {
        foreach (var item in MiniMapObjects)
        {
            Destroy(item.Value.RoomMiniMap);
        }
        MiniMapObjects.Clear();
    }
}
