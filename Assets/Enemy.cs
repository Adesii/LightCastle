using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lean.Pool;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IEnemy
{
    public StatHolder EnemyStats;
    NavMeshAgent agent;

    public void TakeDamage(float damage, Vector3 direction = default)
    {
        //Debug.Log("Enemy took damage");
        EnemyStats.Health -= damage;
        Knockback(direction);
        if (EnemyStats.Health <= 0)
        {
            Die();
        }
    }

    private async void Knockback(Vector3 direction)
    {
        Vector3 amount = direction * 0.5f;
        while (amount.magnitude > 0.1f && agent != null && agent.isActiveAndEnabled)
        {
            amount = Vector3.Lerp(amount, Vector3.zero, 0.1f);
            agent.Move(amount);
            await Task.Delay(TimeSpan.FromMilliseconds(10));
        }
    }

    private void Die()
    {

        GameManager.Instance.DropSouls(50, this);
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        EnemyStats = new StatHolder();
    }

    TimeSince lastAttack;

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position) > 2f)
        {
            agent.SetDestination(GameManager.Instance.Player.transform.position);
            lastAttack = 0;
        }
        else if (lastAttack > 1f)
        {
            GameManager.Instance.Player.TakeDamage(10);
            lastAttack = 0;
        }

    }
}

interface IEnemy
{
    void TakeDamage(float damage, Vector3 Direction = default);

}
