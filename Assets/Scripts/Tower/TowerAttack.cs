using UnityEngine;
using System;

[RequireComponent(typeof(Tower))]
public sealed class TowerAttack : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayerMask = ~0;

    private Tower tower;
    private float cooldown;

    public event Action<Enemy> Fired;

    private void Awake()
    {
        tower = GetComponent<Tower>();
    }

    private void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown > 0f)
        {
            return;
        }

        Enemy target = FindNearestTarget();
        if (target == null)
        {
            return;
        }

        target.TakeDamage(tower.Damage);
        Fired?.Invoke(target);
        cooldown = tower.AttackInterval;
    }

    private Enemy FindNearestTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, tower.Range, enemyLayerMask);
        Enemy nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            if (!hit.TryGetComponent(out Enemy enemy) || enemy.IsDead)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearest = enemy;
                nearestDistance = distance;
            }
        }

        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Tower currentTower = tower != null ? tower : GetComponent<Tower>();
        if (currentTower == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, currentTower.Range);
    }
}
