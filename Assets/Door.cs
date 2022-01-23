using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class Door : MonoBehaviour
{

    public NavMeshLink linkData;

    public bool OnlyConnectDoor;
    public Vector3 OutDirection => -transform?.right ?? Vector3.down;

    public Room ConnectedTo;

    public Room ConnectedFrom;


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + OutDirection * 2);
        //Gizmos.DrawCube(transform.position + OutDirection * 20, Vector3.one * 20f);
    }

}
