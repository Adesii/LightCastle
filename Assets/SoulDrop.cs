using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SoulDrop : MonoBehaviour
{
    public int DropAmount = 10;
    public TimeSince Dropped;
    public float DropSpeed => 5f;

    public Vector3 DropDirection;

    private void OnEnable()
    {
        Dropped = 0;
    }

    private void Update()
    {
        if (Dropped < 1.5f)
        {

            if (Dropped < 0.3f)
                transform.position = Vector3.MoveTowards(transform.position, transform.position + (DropDirection * Dropped), DropSpeed * Time.deltaTime);

        }
        else
        {
            var player = GameManager.Instance.Player;
            var speedup = Dropped - 1.5f;
            transform.position = Vector3.MoveTowards(transform.position, GameManager.Instance.Player.transform.position, DropSpeed * (speedup * speedup) * Time.deltaTime);
            if (Vector3.Distance(transform.position, player.transform.position) < 0.1f)
            {
                GameManager.Instance.AddXP(DropAmount);
                LeanPool.Despawn(gameObject);
            }
        }
    }

}
