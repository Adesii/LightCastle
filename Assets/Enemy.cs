using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lean.Pool;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IEnemy
{
    public StatHolder EnemyStats;
    NavMeshAgent agent;

    public Animator animator;

    public void TakeDamage(float damage, Vector3 direction = default)
    {
        Debug.Log(damage);
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
        animator.SetTrigger("KnockBack");
        while (amount.magnitude > 0.1f && agent != null && agent.isActiveAndEnabled)
        {
            amount = Vector3.Lerp(amount, Vector3.zero, 0.1f);
            agent.Move(amount);
            await Task.Delay(TimeSpan.FromMilliseconds(10));
        }
    }

    private void Die()
    {

        GameManager.Instance.DropSouls(Mathf.RoundToInt(UnityEngine.Random.Range(GameManager.Instance.EnemyXPDrop.Evaluate(EnemyStats.Level - 2), GameManager.Instance.EnemyXPDrop.Evaluate(EnemyStats.Level))), this);
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        EnemyStats = GameManager.Instance.EnemyStats;
    }

    TimeSince lastAttack;
    public bool IsActive { get; set; } = false;

    // Update is called once per frame
    void Update()
    {
        if (!IsActive) return;
        animator.SetBool("Chasing", true);
        if (Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position) > 2f)
        {
            agent.SetDestination(GameManager.Instance.Player.transform.position);
            lastAttack = 0;
        }
        else if (lastAttack > 0.3f && lastAttack < 1f)
        {
            animator.SetTrigger("Attack");
        }
        else if (lastAttack > 1.5f)
        {
            GameManager.Instance.Player.TakeDamage(EnemyStats.Strength);
            lastAttack = 0;
        }

    }
}

interface IEnemy
{
    bool IsActive { get; set; }
    void TakeDamage(float damage, Vector3 Direction = default);

}
