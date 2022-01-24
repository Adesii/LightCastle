using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Linq;
using System;

public class Room : MonoBehaviour
{

    public bool StartRoom;
    public Door[] Doors;
    private NavMeshSurface[] Links;

    public GameObject MiniMapRoomPrefab;


    public int EnemySpawnAmount => GameManager.Instance.EnemySpawnAmount;



    public RoomDesignation RoomDesignation;

    public List<Door> ConnectedDoors = new List<Door>();
    [SerializeField]
    public List<Vector2Int> MinMaxObstacleCount = new List<Vector2Int>();


    private List<ObjectSpawn> EnemySpawnsList = new List<ObjectSpawn>();

    private List<IEnemy> Enemies = new List<IEnemy>();

    public bool HasEndSpawn = false;

    // Start is called before the first frame update
    void Start()
    {
        InitDecorate();

    }

    ObjectSpawn[] Objectspawns;

    private void InitDecorate()
    {
        Objectspawns = GetComponentsInChildren<ObjectSpawn>();
        if (Objectspawns.Any(e => e.IsPossibleEndSpawn))
        {
            HasEndSpawn = true;
            return;
        }
        DecorateRoom();
    }

    public void DecorateRoom()
    {
        var spawns = Objectspawns.Where(e => e.Type != ObjectSpawn.ObstacleType.Enemy && !e.IsPossibleEndSpawn).OrderBy(e => System.Guid.NewGuid()).GroupBy(e => e.Type).ToList();
        EnemySpawnsList = Objectspawns.Where(e => e.Type == ObjectSpawn.ObstacleType.Enemy && !e.IsPossibleEndSpawn).OrderBy(e => System.Guid.NewGuid()).ToList();

        Dictionary<ObjectSpawn.ObstacleType, int> ObstacleCount = new Dictionary<ObjectSpawn.ObstacleType, int>();

        foreach (var i in Enum.GetValues(typeof(ObjectSpawn.ObstacleType)))
        {
            ObstacleCount.Add((ObjectSpawn.ObstacleType)i, 0);
        }
        for (int i = 0; i < MinMaxObstacleCount.Count; i++)
        {
            Vector2Int minmax = MinMaxObstacleCount[i];
            ObstacleCount[(ObjectSpawn.ObstacleType)i] = UnityEngine.Random.Range(minmax.x, minmax.y);
        }
        //calculate average obstacle count for each type

        Dictionary<ObjectSpawn.ObstacleType, int> ObstacleCountSpawned = new Dictionary<ObjectSpawn.ObstacleType, int>();

        foreach (var i in Enum.GetValues(typeof(ObjectSpawn.ObstacleType)))
        {
            ObstacleCountSpawned.Add((ObjectSpawn.ObstacleType)i, 0);
        }


        foreach (var spawn in spawns)
        {
            foreach (var item in spawn)
            {
                if (ObstacleCountSpawned[item.Type] < ObstacleCount[item.Type])
                {
                    ObstacleCountSpawned[item.Type]++;
                    item.spawn();
                }
            }
        }


    }

    public void InitNavMesh()
    {
        Links = GetComponentsInChildren<NavMeshSurface>();
        foreach (var link in Links)
        {
            link.BuildNavMesh();
        }
    }

    public void SpawnEnemies()
    {
        var AmountToSpawn = UnityEngine.Random.Range(0, EnemySpawnAmount);
        for (int i = 0; i < AmountToSpawn; i++)
        {
            if (i >= EnemySpawnsList.Count)
            {
                break;
            }
            var item = EnemySpawnsList[i];
            Enemies.Add(Instantiate(GameManager.Instance.EnemyPrefab, item.transform.position, item.transform.rotation, item.transform.parent).GetComponent<IEnemy>());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TriggerRoom()
    {
        Debug.Log("Triggered room");

        foreach (var item in Enemies)
        {
            item.IsActive = true;
        }
    }
}