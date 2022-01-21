using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class Room : MonoBehaviour
{
    public Door[] Doors;
    private NavMeshSurface[] Links;
    // Start is called before the first frame update
    void Start()
    {
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
}
