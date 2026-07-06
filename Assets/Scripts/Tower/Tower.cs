using UnityEngine;

[RequireComponent(typeof(TowerAttack))]
public sealed class Tower : MonoBehaviour
{
    [SerializeField] private TowerData data;

    public TowerData Data => data;
    public float Damage => data != null ? data.damage : 10f;
    public float AttackInterval => data != null ? data.attackInterval : 1f;
    public float Range => data != null ? data.range : 4f;

    public void Initialize(TowerData towerData)
    {
        data = towerData;
    }
}
