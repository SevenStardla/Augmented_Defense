using UnityEngine;

[RequireComponent(typeof(DefenderController))]
public sealed class DefenderAttackVfx : MonoBehaviour
{
    [SerializeField] private Color lineColor = new Color(0.35f, 1f, 0.8f, 0.9f);
    [SerializeField] private float lineDuration = 0.08f;
    [SerializeField] private float lineWidth = 0.045f;

    private DefenderController controller;
    private LineRenderer line;
    private Transform target;
    private float timer;

    private void Awake()
    {
        controller = GetComponent<DefenderController>();
        line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth * 0.55f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = lineColor;
        line.endColor = lineColor;
        line.enabled = false;
    }

    private void OnEnable()
    {
        controller.Fired += HandleFired;
    }

    private void OnDisable()
    {
        controller.Fired -= HandleFired;
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
