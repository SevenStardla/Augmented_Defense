using UnityEngine;

[RequireComponent(typeof(TowerAttack))]
public sealed class TowerTargetLine : MonoBehaviour
{
    [SerializeField] private Color lineColor = new Color(1f, 0.9f, 0.3f, 0.85f);
    [SerializeField] private float lineDuration = 0.08f;
    [SerializeField] private float lineWidth = 0.04f;

    private TowerAttack towerAttack;
    private LineRenderer line;
    private Transform target;
    private float timer;

    private void Awake()
    {
        towerAttack = GetComponent<TowerAttack>();
        line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth * 0.5f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = lineColor;
        line.endColor = lineColor;
        line.enabled = false;
    }

    private void OnEnable()
    {
        towerAttack.Fired += HandleFired;
    }

    private void OnDisable()
    {
        towerAttack.Fired -= HandleFired;
    }

    private void LateUpdate()
    {
        if (timer <= 0f || target == null)
        {
            line.enabled = false;
            return;
        }

        timer -= Time.deltaTime;
        Color current = lineColor;
        current.a *= Mathf.Clamp01(timer / lineDuration);
        line.startColor = current;
        line.endColor = current;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, target.position);
        line.enabled = true;
    }

    private void HandleFired(Enemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        target = enemy.transform;
        timer = lineDuration;
    }
}
