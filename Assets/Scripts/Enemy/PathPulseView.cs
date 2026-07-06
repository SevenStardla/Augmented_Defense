using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public sealed class PathPulseView : MonoBehaviour
{
    [SerializeField] private Color idleColor = new Color(0.85f, 0.67f, 0.28f, 0.55f);
    [SerializeField] private Color pulseColor = new Color(1f, 0.92f, 0.38f, 1f);
    [SerializeField] private Color inactiveColor = new Color(0.25f, 0.22f, 0.16f, 0.45f);
    [SerializeField] private float pulseDuration = 0.35f;

    private LineRenderer line;
    private float pulseTimer;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StateChanged += HandleStateChanged;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StateChanged -= HandleStateChanged;
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.State == GameState.GameOver)
        {
            SetColor(inactiveColor);
            return;
        }

        if (pulseTimer > 0f)
        {
            pulseTimer -= Time.deltaTime;
            float t = Mathf.Clamp01(pulseTimer / pulseDuration);
            SetColor(Color.Lerp(idleColor, pulseColor, t));
            return;
        }

        SetColor(idleColor);
    }

    private void HandleStateChanged(GameState state)
    {
        if (state == GameState.WavePhase)
        {
            pulseTimer = pulseDuration;
        }
    }

    private void SetColor(Color color)
    {
        line.startColor = color;
        line.endColor = color;
    }
}
