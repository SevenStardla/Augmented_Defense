using System;
using UnityEngine;

public sealed class CoreHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }

    public event Action<int, int> HealthChanged;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    private void Start()
    {
        HealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || CurrentHealth <= 0)
        {
            return;
        }

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        HealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth == 0)
        {
            GameManager.Instance?.GameOver();
        }
    }
}
