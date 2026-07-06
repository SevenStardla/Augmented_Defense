using UnityEngine;

public sealed class EnemyMovement : MonoBehaviour
{
    private Transform[] path;
    private CoreHealth core;
    private Enemy enemy;
    private int pathIndex;

    public void Initialize(Transform[] waypoints, CoreHealth targetCore, Enemy owner)
    {
        path = waypoints;
        core = targetCore;
        enemy = owner;
        pathIndex = 0;

        if (path != null && path.Length > 0)
        {
            transform.position = path[0].position;
            pathIndex = 1;
        }
    }

    private void Update()
    {
        if (enemy == null || enemy.IsDead || path == null || path.Length == 0)
        {
            return;
        }

        if (pathIndex >= path.Length)
        {
            core?.TakeDamage(enemy.CoreDamage);
            enemy.NotifyReachedCore();
            return;
        }

        Vector3 target = path[pathIndex].position;
        float step = enemy.GetMoveSpeed() * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);

        if (Vector3.Distance(transform.position, target) <= 0.01f)
        {
            pathIndex++;
        }
    }
}
