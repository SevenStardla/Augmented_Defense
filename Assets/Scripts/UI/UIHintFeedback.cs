using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public sealed class UIHintFeedback : MonoBehaviour
{
    [SerializeField] private float startupVisibleTime = 3f;
    [SerializeField] private float fadedAlpha = 0.45f;
    [SerializeField] private float gameOverAlpha = 0.2f;

    private Text text;
    private UITextFeedback feedback;
    private DefenderController defender;
    private TowerPlacement placement;
    private Color baseColor;
    private bool moved;
    private bool fired;
    private bool placed;
    private float age;

    public void Configure(DefenderController playerDefender, TowerPlacement towerPlacement)
    {
        defender = playerDefender;
        placement = towerPlacement;
        Subscribe();
        RefreshText();
    }

    private void Awake()
    {
        text = GetComponent<Text>();
        feedback = GetComponent<UITextFeedback>();
        baseColor = text.color;
    }

    private void OnEnable()
    {
        Subscribe();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StateChanged += HandleStateChanged;
        }
    }

    private void OnDisable()
    {
        if (defender != null)
        {
            defender.MoveInputChanged -= HandleMoveInputChanged;
            defender.Fired -= HandleFired;
        }

        if (placement != null)
        {
            placement.PlacementSucceeded -= HandlePlacementSucceeded;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StateChanged -= HandleStateChanged;
        }
    }

    private void Update()
    {
        age += Time.unscaledDeltaTime;
        float targetAlpha = age >= startupVisibleTime || moved || fired || placed ? fadedAlpha : baseColor.a;
        if (GameManager.Instance != null && GameManager.Instance.State == GameState.GameOver)
        {
            targetAlpha = gameOverAlpha;
        }

        Color color = text.color;
        color.a = Mathf.Lerp(color.a, targetAlpha, Time.unscaledDeltaTime * 6f);
        text.color = color;
    }

    private void Subscribe()
    {
        if (!isActiveAndEnabled)
        {
            return;
        }

        if (defender != null)
        {
            defender.MoveInputChanged -= HandleMoveInputChanged;
            defender.Fired -= HandleFired;
            defender.MoveInputChanged += HandleMoveInputChanged;
            defender.Fired += HandleFired;
        }

        if (placement != null)
        {
            placement.PlacementSucceeded -= HandlePlacementSucceeded;
            placement.PlacementSucceeded += HandlePlacementSucceeded;
        }
    }

    private void HandleMoveInputChanged(Vector2 input)
    {
        if (moved || input.sqrMagnitude <= 0.01f)
        {
            return;
        }

        moved = true;
        PlayAcknowledge();
        RefreshText();
    }

    private void HandleFired(Enemy enemy)
    {
        if (fired)
        {
            return;
        }

        fired = true;
        PlayAcknowledge();
        RefreshText();
    }

    private void HandlePlacementSucceeded(Vector3 position)
    {
        if (placed)
        {
            return;
        }

        placed = true;
        PlayAcknowledge();
        RefreshText();
    }

    private void HandleStateChanged(GameState state)
    {
        if (state == GameState.GameOver && feedback != null)
        {
            feedback.SetDimmed(true);
        }
    }

    private void PlayAcknowledge()
    {
        if (feedback != null)
        {
            feedback.PlayFlash(Color.white, 1.04f);
        }
    }

    private void RefreshText()
    {
        string move = moved ? "WASD moved" : "WASD move";
        string attack = fired ? "Space fired" : "Space shoot";
        string build = placed ? "Tower placed" : "Left click place tower";
        text.text = $"{move}   {attack}   {build}";
    }
}
