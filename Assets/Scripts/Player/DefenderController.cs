using UnityEngine;
using System;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class DefenderController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float attackRange = 3.5f;
    [SerializeField] private float attackDamage = 12f;
    [SerializeField] private float attackInterval = 0.35f;
    [SerializeField] private LayerMask enemyLayerMask = ~0;

    private float cooldown;

    public event Action<Vector2> MoveInputChanged;
    public event Action<Enemy> Fired;

    private void Update()
    {
        if (GameManager.Instance != null && (GameManager.Instance.State == GameState.GameOver || GameManager.Instance.State == GameState.Clear))
        {
            return;
        }

        Move();
        Attack();
    }

    public void Configure(float speed, float range, float damage, float interval, LayerMask enemyLayers)
    {
        moveSpeed = speed;
        attackRange = range;
        attackDamage = damage;
        attackInterval = interval;
        enemyLayerMask = enemyLayers;
    }

    private void Move()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

        MoveInputChanged?.Invoke(input);
        transform.position += (Vector3)(input * moveSpeed * Time.deltaTime);
    }

    private void Attack()
    {
        if (GameManager.Instance != null && GameManager.Instance.State != GameState.WavePhase)
        {
            return;
        }

        cooldown -= Time.deltaTime;
        if (!Input.GetKey(KeyCode.Space) || cooldown > 0f)
        {
            return;
        }

        Enemy target = FindNearestEnemy();
        if (target == null)
        {
            return;
        }

        float multiplier = RuntimeAugmentStats.Instance != null ? RuntimeAugmentStats.Instance.DefenderDamageMultiplier : 1f;
        target.TakeDamage(attackDamage * multiplier);
        Fired?.Invoke(target);
        float attackSpeedMultiplier = RuntimeAugmentStats.Instance != null ? RuntimeAugmentStats.Instance.DefenderAttackSpeedMultiplier : 1f;
        cooldown = attackInterval / attackSpeedMultiplier;
    }

    private Enemy FindNearestEnemy()
    {
        float rangeMultiplier = RuntimeAugmentStats.Instance != null ? RuntimeAugmentStats.Instance.DefenderRangeMultiplier : 1f;
        float currentRange = attackRange * rangeMultiplier;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, currentRange, enemyLayerMask);
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
                nearestDistance = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        float rangeMultiplier = RuntimeAugmentStats.Instance != null ? RuntimeAugmentStats.Instance.DefenderRangeMultiplier : 1f;
        Gizmos.DrawWireSphere(transform.position, attackRange * rangeMultiplier);
    }
}
