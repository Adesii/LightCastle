using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IEnemy
{
    public StatHolder EnemyStats;
    NavMeshAgent agent;

    public void TakeDamage(float damage, Vector3 direction = default)
    {
        Debug.Log("Enemy took damage");
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
        agent.SetDestination(GameManager.Instance.Player.transform.position);

    }
}

interface IEnemy
{
    void TakeDamage(float damage, Vector3 Direction = default);

}
