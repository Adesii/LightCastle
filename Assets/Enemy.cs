using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IEnemy
{
    public StatHolder EnemyStats;
    NavMeshAgent agent;

    public void TakeDamage(float damage)
    {
        Debug.Log("Enemy took damage");
        EnemyStats.Health -= damage;
        if (EnemyStats.Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        EnemyStats = new StatHolder();
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(FindObjectOfType<PlayerCharacter>().transform.position);

    }
}

interface IEnemy
{
    void TakeDamage(float damage);

}
