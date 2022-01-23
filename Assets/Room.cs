using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Linq;

public class Room : MonoBehaviour
{

    public bool StartRoom;
    public Door[] Doors;
    private NavMeshSurface[] Links;

    public Transform[] EnemySpawns;



    public RoomDesignation RoomDesignation;

    public List<Door> ConnectedDoors = new List<Door>();

    public int MinObjectSpawn = 2;
    public int MaxObjectSpawn = 5;

    // Start is called before the first frame update
    void Start()
    {

        var Objectspawns = GetComponentsInChildren<ObjectSpawn>();
        var spawns = Objectspawns.OrderBy(e => System.Guid.NewGuid()).ToList();

        var decidednum = Random.Range(MinObjectSpawn, MaxObjectSpawn);

        for (int i = 0; i < decidednum; i++)
        {
            if (i < spawns.Count)
            {
                spawns[i].spawn();
            }
        }

        Links = GetComponentsInChildren<NavMeshSurface>();
        foreach (NavMeshSurface link in Links)
        {
            link.BuildNavMesh();
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TriggerRoom()
    {
        Debug.Log("Triggered room");
    }
}
