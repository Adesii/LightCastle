using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine.SceneManagement;

[System.Serializable]
public partial class GameManager : yaSingleton.Singleton<GameManager>
{

    public GameObject EnemyPrefab;
    /// <summary>
    ///  Room Prefabs
    /// </summary>
    public WeightedRandom[] Rooms;
    /// <summary>
    ///  Door Prefabs
    /// </summary>
    public GameObject[] Doors;

    [System.Serializable]
    public struct WeightedRandom
    {
        public GameObject Object;
        public int Weight;
    }

    private List<GameObject> everythingCache = new List<GameObject>();

    public static Dictionary<Vector2Int, RoomDesignation> RoomGrid = new Dictionary<Vector2Int, RoomDesignation>();

    public bool CreateDungeon;
    public bool Ascents;

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (CreateDungeon)
        {
            GenerateDungeon();
            CreateDungeon = false;
            GeneratingDungeon = true;
        }
        if (Ascents)
        {
            Ascents = false;
            Ascent();
        }
    }

    private bool GeneratingDungeon = false;
    public GameObject EndSpawn;

    public int EnemySpawnAmount => Mathf.RoundToInt(EnemySpawnRate.Evaluate(GameManager.Instance.PlayerStats.CurrentAscention));
    public StatHolder EnemyStats
    {
        get
        {
            var stats = new StatHolder();
            stats.Level = Mathf.RoundToInt(EnemyLevel.Evaluate(PlayerStats.CurrentAscention));
            stats.Health = EnemyHealthCurve.Evaluate(stats.Level);
            stats.Strength = Mathf.RoundToInt(EnemyDamageCurve.Evaluate(stats.Level));
            return stats;
        }
    }

    public async void GenerateDungeon()
    {
        Debug.Log("Generating Dungeon");
        //Debug.Log(GeneratingDungeon);
        //if (GeneratingDungeon) return;
        foreach (var item in everythingCache)
        {
            Destroy(item.gameObject);
        }
        RoomGrid.Clear();
        Debug.Log("Cleared RoomGrid");
        var StartRoom = UnityEngine.Object.FindObjectsOfType<Room>().FirstOrDefault(x => x.StartRoom);
        RoomGrid.Add(new Vector2Int(0, 0), new RoomDesignation(new Vector2Int(0, 0), StartRoom.gameObject));
        RoomGrid.Add(new Vector2Int(-1, 0), new RoomDesignation(new Vector2Int(0, 0), null));
        var FirstDoor = StartRoom.GetComponentInChildren<Door>();
        StartRoom.ConnectedDoors.Add(FirstDoor);
        StartRoom.RoomDesignation = RoomGrid[new Vector2Int(0, 0)];
        await RecursiveResolveRoom(FirstDoor, StartRoom, 0);
        Debug.Log("Finished Generating Dungeon");
        GeneratingDungeon = false;

        if (RoomGrid.Count < 5)
        {
            await Task.Delay(100);
            GenerateDungeon();
            GeneratingDungeon = true;
            return;
        }

        foreach (var doors in GameObject.FindObjectsOfType<Door>())
        {
            doors.ConnectedFrom = doors.GetComponentInParent<Room>();
            if (doors != null && doors.ConnectedFrom.RoomDesignation != null)
            {
                var gridpos = doors.ConnectedFrom.RoomDesignation.position + new Vector2Int(Mathf.RoundToInt(doors.OutDirection.x), Mathf.RoundToInt(doors.OutDirection.z));
                if (RoomGrid.ContainsKey(gridpos))
                {
                    if (RoomGrid[gridpos] != null && RoomGrid[gridpos].room != null)
                        foreach (var door in RoomGrid[gridpos].room.GetComponent<Room>().Doors)
                        {
                            if (door != null && doors != null && Vector3.Distance(door.transform.position, doors.transform.position) < 5f && door.ConnectedTo == null)
                            {
                                doors.ConnectedTo = door.ConnectedFrom;

                                door.ConnectedTo = doors.ConnectedFrom;
                            }
                        }
                }
            }
        }
        foreach (var doors in FindObjectsOfType<Door>())
        {
            if (doors.ConnectedTo == null)
            {
                doors.GetComponent<MeshRenderer>().enabled = true;
                doors.GetComponent<Collider>().enabled = true;


            }
        }


        var EndroomPossibilities = FindObjectsOfType<Room>().Where(x => x.HasEndSpawn).OrderBy(e => System.Guid.NewGuid()).ToList();
        for (int i = 0; i < EndroomPossibilities.Count; i++)
        {
            var item = EndroomPossibilities[i];
            if (i == 0)
            {
                item.GetComponentsInChildren<ObjectSpawn>().Where(e => e.IsPossibleEndSpawn).OrderBy(e => System.Guid.NewGuid()).FirstOrDefault()?.spawn();
            }
            else
            {
                item.DecorateRoom();
            }
        }
        if (FindObjectOfType<EndManager>() == null)
        {
            GenerateDungeon();
            GeneratingDungeon = true;
            return;
        }

        GeneratingDungeon = false;

        foreach (var item in FindObjectsOfType<Room>())
        {
            item.InitNavMesh();
            item.SpawnEnemies();
        }


    }
    public async Task RecursiveResolveRoom(Door lastdoor, Room LastRoom, int depth)
    {
        if (lastdoor == null || LastRoom == null || RoomGrid.Count > 20)
        {
            Debug.Log("Failed to resolve room");
            return;
        }

        Debug.Log("Resolving Room");
        var GridPosition = LastRoom.RoomDesignation.position + new Vector2Int(Mathf.RoundToInt(lastdoor.OutDirection.x), Mathf.RoundToInt(lastdoor.OutDirection.z));
        if (RoomGrid.ContainsKey(GridPosition))
        {
            Debug.Log("Already Resolved");
            return;
        }

        if (depth >= 3)
        {
            Debug.Log("Depth Reached");

            var lastsdoor = Instantiate(Doors[Random.Range(0, Doors.Length)], lastdoor.transform.position + (Vector3.down * 1.6f), Quaternion.LookRotation(lastdoor.OutDirection, Vector3.up));
            var dj = lastsdoor.GetComponent<Room>();
            lastdoor.ConnectedTo = dj;
            dj.RoomDesignation = new RoomDesignation(GridPosition, dj.gameObject);
            RoomGrid.Add(dj.RoomDesignation.position, dj.RoomDesignation);
            everythingCache.Add(lastsdoor);
            var lastdoorConnectorEnd = lastsdoor.GetComponentInChildren<Door>();
            lastdoorConnectorEnd.ConnectedTo = LastRoom;
            var deadend = Instantiate(Rooms[0].Object, Vector3.zero, Quaternion.identity);
            var deadendRoom = new RoomDesignation(dj.RoomDesignation.position + new Vector2Int(Mathf.RoundToInt(lastdoorConnectorEnd.OutDirection.x), Mathf.RoundToInt(lastdoorConnectorEnd.OutDirection.z)), deadend.gameObject);
            if (RoomGrid.ContainsKey(deadendRoom.position))
            {
                //lastdoorConnectorEnd.GetComponent<MeshRenderer>().enabled = true;
                //lastdoorConnectorEnd.GetComponent<Collider>().enabled = true;
                Destroy(deadend.gameObject);
                return;
            }
            RoomGrid.Add(deadendRoom.position, deadendRoom);


            deadend.GetComponent<Room>().RoomDesignation = deadendRoom;



            everythingCache.Add(deadend);
            var deadendinNewRoom = deadend.GetComponentInChildren<Door>();
            deadendinNewRoom.ConnectedTo = dj;
            //transform the DoorinNewRoom to be at the end of NewDoorConnectorEnd by transforming the NewRoom
            //NewRoom.transform.rotation = Quaternion.LookRotation(NewDoorConnectorEnd.transform.forward, Vector3.up);
            deadend.transform.rotation = Quaternion.LookRotation(-lastdoorConnectorEnd.OutDirection);
            deadend.transform.position = lastdoorConnectorEnd.transform.position - deadendinNewRoom.transform.position;
            deadend.transform.RotateAround(deadendinNewRoom.transform.position, Vector3.up, Vector3.SignedAngle(lastdoorConnectorEnd.OutDirection, deadendinNewRoom.OutDirection, Vector3.up));
            return;
        }
        Debug.Log("Generating Room");


        var NewDoor = Instantiate(Doors[Random.Range(0, Doors.Length)], lastdoor.transform.position + (Vector3.down * 1.6f), Quaternion.LookRotation(lastdoor.OutDirection, Vector3.up));
        NewDoor.GetComponent<Room>().ConnectedDoors.Add(lastdoor);
        lastdoor.ConnectedTo = NewDoor.GetComponent<Room>();
        Debug.Log("Generated Door");


        RoomGrid.Add(GridPosition, new RoomDesignation(GridPosition, NewDoor));
        NewDoor.GetComponent<Room>().RoomDesignation = new RoomDesignation(GridPosition, NewDoor.gameObject);
        everythingCache.Add(NewDoor);
        var NewDoorConnectorEnd = NewDoor.GetComponentsInChildren<Door>();

        foreach (var item in NewDoorConnectorEnd)
        {
            LastRoom.ConnectedDoors.Add(item);
            item.ConnectedFrom = NewDoor.GetComponent<Room>();

            if (RoomGrid.ContainsKey(GridPosition + new Vector2Int(Mathf.RoundToInt(item.OutDirection.x), Mathf.RoundToInt(item.OutDirection.z))))
            {
                Debug.Log($"{GridPosition} From");
                Debug.Log($"{GridPosition + new Vector2Int(Mathf.RoundToInt(item.OutDirection.x), Mathf.RoundToInt(item.OutDirection.z))} is already in the room grid");

                return;
            }

            var NewRoom = Instantiate(Rooms[GetRandomWeightedIndex(Rooms)].Object, Vector3.zero, Quaternion.identity);
            everythingCache.Add(NewRoom);
            var romd = new RoomDesignation(GridPosition + new Vector2Int(Mathf.RoundToInt(item.OutDirection.x), Mathf.RoundToInt(item.OutDirection.z)), NewRoom.gameObject);
            RoomGrid.Add(romd.position, romd);
            NewRoom.GetComponent<Room>().RoomDesignation = romd;

            item.ConnectedTo = NewRoom.GetComponent<Room>();

            NewRoom.GetComponent<Room>().Doors[0].ConnectedTo = NewDoor.GetComponent<Room>();

            var doorsinNewRoom = NewRoom.GetComponentsInChildren<Door>();
            var doorinNewRoom = doorsinNewRoom[0];
            //transform the DoorinNewRoom to be at the end of NewDoorConnectorEnd by transforming the NewRoom
            NewRoom.transform.rotation = Quaternion.LookRotation(-item.transform.forward, Vector3.up);
            NewRoom.transform.position = item.transform.position - doorinNewRoom.transform.position;
            //Debug.Log(Vector3.SignedAngle(item.transform.forward, -doorinNewRoom.OutDirection, Vector3.up));
            //NewRoom.transform.RotateAround(doorinNewRoom.transform.position, Vector3.up, Mathf.Abs(Vector3.SignedAngle(-item.OutDirection, doorinNewRoom.OutDirection, Vector3.up)));
            await Task.Delay(1);
            foreach (var doors in NewRoom.GetComponentsInChildren<Door>().Where(x => x != doorinNewRoom))
            {
                doors.ConnectedFrom = NewRoom.GetComponent<Room>();
                await Task.Delay(1);
                await RecursiveResolveRoom(doors, NewRoom.GetComponent<Room>(), depth + 1);
            }
        }


    }

    public void RecursiveDestroy(Door from)
    {
        if (from.ConnectedTo != null)
            foreach (var dors in from.ConnectedTo.GetComponent<Room>().ConnectedDoors)
            {
                if (dors.ConnectedTo != null)
                {
                    //RecursiveDestroy(dors);
                    Destroy(dors.ConnectedTo.gameObject);
                }
            }
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        for (int x = -50; x < 50; x++)
        {
            for (int y = -50; y < 50; y++)
            {
                if (RoomGrid.ContainsKey(new Vector2Int(x, y)))
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(new Vector3(x * 40, 0, y * 40), Vector3.one * 10);
                }
                else
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(new Vector3(x * 40, 0, y * 40), Vector3.one * 10);
                }
            }
        }

    }

    public int GetRandomWeightedIndex(WeightedRandom[] weights)
    {
        // Get the total sum of all the weights.
        int weightSum = 0;
        for (int i = 0; i < weights.Length; ++i)
        {
            weightSum += weights[i].Weight;
        }

        // Step through all the possibilities, one by one, checking to see if each one is selected.
        int index = 0;
        int lastIndex = weights.Length - 1;
        while (index < lastIndex)
        {
            // Do a probability check with a likelihood of weights[index] / weightSum.
            if (Random.Range(0, weightSum) < weights[index].Weight)
            {
                return index;
            }

            // Remove the last item from the sum of total untested weights and try again.
            weightSum -= weights[index++].Weight;
        }

        // No other item was selected, so return very last index.
        return index;
    }

    public async void Ascent()
    {
        PlayerStats.CurrentAscention++;
        if (PlayerStats.HighestAscention <= PlayerStats.CurrentAscention)
        {
            PlayerStats.HighestAscention = PlayerStats.CurrentAscention;
        }
        PlayerStats.SavePlayerStats();
        Player.LockPlayer();
        await MainGameUI.TransitionPanel.DOFade(1, 0.5f).AsyncWaitForCompletion();
        if (PlayerStats.CurrentAscention == 6)
        {
            PlayerStats.PermaXP += PlayerStats.XPInRun;
            SceneManager.LoadScene(2);
            return;
        }
        Player.transform.SetPositionAndRotation(Player.PlayerSpawnPos, Player.PlayerSpawnRot);
        GenerateDungeon();

        Instance.MainGameUI.ClearDict();

        MainGameUI.LevelText.text = $"Castle Ascention :\n {PlayerStats.CurrentAscention} / 5";

        var idk = DOTween.Sequence();
        idk.Append(MainGameUI.TransitionPanel.DOFade(1, 2f));
        idk.Append(MainGameUI.LevelText.DOFade(0, 0f));
        idk.Append(MainGameUI.LevelText.DOFade(1, 2f));
        idk.Append(MainGameUI.LevelText.DOFade(0, 1f));
        idk.Append(MainGameUI.TransitionPanel.DOFade(0, 2f));
        await idk.Play().AsyncWaitForCompletion();
        Player.UnlockPlayer();
    }


    public void CloseGame()
    {
        Application.Quit();
    }
}

