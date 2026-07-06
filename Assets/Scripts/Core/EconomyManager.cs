using System;
using UnityEngine;

public sealed class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    [SerializeField] private int startingGold = 120;

    public int Gold { get; private set; }

    public event Action<int> GoldChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Gold = startingGold;
    }

    private void Start()
    {
        GoldChanged?.Invoke(Gold);
    }

    public bool CanAfford(int amount)
    {
        return Gold >= amount;
    }

    public bool TrySpend(int amount)
    {
        if (!CanAfford(amount))
        {
            return false;
        }

        Gold -= amount;
        GoldChanged?.Invoke(Gold);
        return true;
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Gold += amount;
        GoldChanged?.Invoke(Gold);
    }
}
