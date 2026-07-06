using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class DefenderController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float attackRange = 3.5f;
    [SerializeField] private float attackDamage = 12f;
    [SerializeField] private float attackInterval = 0.35f;
    [SerializeField] private LayerMask enemyLayerMask = ~0;

    private float cooldown;

    private void Update()
    {
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

        transform.position += (Vector3)(input * moveSpeed * Time.deltaTime);
    }

    private void Attack()
    {
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

        target.TakeDamage(attackDamage);
        cooldown = attackInterval;
    }

    private Enemy FindNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayerMask);
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
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
