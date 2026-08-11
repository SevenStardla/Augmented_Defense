using UnityEngine;
using UnityEngine.UI;

public sealed class GameSpeedController : MonoBehaviour
{
    private static readonly float[] SpeedSteps = { 1f, 2f, 3f };

    private Button button;
    private Text label;
    private int speedIndex;

    public float CurrentSpeed => SpeedSteps[speedIndex];

    public void Configure(Button speedButton, Text speedLabel)
    {
        button = speedButton;
        label = speedLabel;
        speedIndex = 0;

        if (button != null)
        {
            button.onClick.AddListener(CycleSpeed);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StateChanged += HandleStateChanged;
            HandleStateChanged(GameManager.Instance.State);
        }
        else
        {
            ApplySpeed();
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(CycleSpeed);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StateChanged -= HandleStateChanged;
        }
    }

    private void CycleSpeed()
    {
        if (GameManager.Instance != null &&
            (GameManager.Instance.State == GameState.GameOver || GameManager.Instance.State == GameState.Clear))
        {
            return;
        }

        speedIndex = (speedIndex + 1) % SpeedSteps.Length;
        ApplySpeed();
    }

    private void HandleStateChanged(GameState state)
    {
        bool terminal = state == GameState.GameOver || state == GameState.Clear;
        if (button != null)
        {
            button.interactable = !terminal;
        }

        if (terminal)
        {
            UpdateLabel(0f);
            return;
        }

        ApplySpeed();
    }

    private void ApplySpeed()
    {
        Time.timeScale = CurrentSpeed;
        UpdateLabel(CurrentSpeed);
    }

    private void UpdateLabel(float speed)
    {
        if (label != null)
        {
            label.text = speed > 0f ? $"Speed {speed:0}x" : "Speed --";
        }
    }
}
