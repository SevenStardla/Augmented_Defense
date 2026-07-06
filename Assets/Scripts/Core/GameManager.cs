using System;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameState initialState = GameState.BuildPhase;

    public GameState State { get; private set; }

    public event Action<GameState> StateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        State = initialState;
    }

    public void SetState(GameState nextState)
    {
        if (State == nextState)
        {
            return;
        }

        State = nextState;
        StateChanged?.Invoke(State);
    }

    public void StartGame()
    {
        SetState(GameState.BuildPhase);
    }

    public void StartWave()
    {
        SetState(GameState.WavePhase);
    }

    public void EnterAugmentPhase()
    {
        SetState(GameState.AugmentPhase);
    }

    public void GameOver()
    {
        SetState(GameState.GameOver);
        Time.timeScale = 0f;
    }

    public void Clear()
    {
        SetState(GameState.Clear);
        Time.timeScale = 0f;
    }

    public void RestartRuntime()
    {
        Time.timeScale = 1f;
        SetState(initialState);
    }
}
