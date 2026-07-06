using System;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public sealed class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;

    public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }
    public int CoreDamage => data != null ? data.coreDamage : 10;
    public int GoldReward => data != null ? data.goldReward : 5;

    public event Action<Enemy> Died;
    public event Action<Enemy> ReachedCore;

    private EnemyMovement movement;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
    }

    public void Initialize(EnemyData enemyData, Transform[] path, CoreHealth core)
    {
        data = enemyData;
        IsDead = false;
        CurrentHealth = data != null ? data.maxHealth : 30;
        movement.Initialize(path, core, this);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f)
        {
            return;
        }

        CurrentHealth -= Mathf.CeilToInt(amount);
        if (CurrentHealth <= 0)
        {
            Die(true);
        }
    }

    public float GetMoveSpeed()
    {
        return data != null ? data.moveSpeed : 1f;
    }

    public void NotifyReachedCore()
    {
        if (IsDead)
        {
            return;
        }

        ReachedCore?.Invoke(this);
        Die(false);
    }

    private void Die(bool grantReward)
    {
        if (IsDead)
        {
            return;
        }

        IsDead = true;

        if (grantReward)
        {
            EconomyManager.Instance?.AddGold(GoldReward);
        }

        Died?.Invoke(this);
        Destroy(gameObject);
    }
}
