using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectSpawn : MonoBehaviour
{

    public ObstacleType Type;


    public enum ObstacleType
    {
        Small,
        Medium,
        Large
    }


    private void OnDrawGizmosSelected()
    {
        switch (Type)
        {
            case ObstacleType.Small:
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(transform.position + Vector3.up * 5, new Vector3(2, 10, 2));
                break;
            case ObstacleType.Medium:
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(transform.position + Vector3.up * 5, new Vector3(8, 10, 8));
                break;
            case ObstacleType.Large:
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(transform.position + Vector3.up * 5, new Vector3(15, 10, 15));
                break;
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    internal void spawn()
    {
        switch (Type)
        {
            case ObstacleType.Small:
                var small = GameManager.Instance.SmallObjects.Random();
                Instantiate(small, transform.position, small.transform.rotation * Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0), transform);
                break;
            case ObstacleType.Medium:
                var medium = GameManager.Instance.MediumObjects.Random();
                Instantiate(medium, transform.position, medium.transform.rotation, transform);
                break;
            case ObstacleType.Large:
                var large = GameManager.Instance.LargeObjects.Random();
                Instantiate(large, transform.position, large.transform.rotation, transform);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
