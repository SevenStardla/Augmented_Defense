using UnityEngine;
using System;

public sealed class TowerPlacement : MonoBehaviour
{
    [SerializeField] private Camera worldCamera;
    [SerializeField] private Tower towerPrefab;
    [SerializeField] private TowerData selectedTowerData;
    [SerializeField] private LayerMask blockedLayerMask;
    [SerializeField] private float placementRadius = 0.35f;

    public TowerData SelectedTowerData => selectedTowerData;
    public float PlacementRadius => placementRadius;
    public event Action<Vector3> PlacementSucceeded;
    public event Action<Vector3> PlacementFailed;

    private void Awake()
    {
        if (worldCamera == null)
        {
            worldCamera = Camera.main;
        }
    }

    public void Configure(Camera camera, Tower prefab, TowerData towerData, LayerMask blockedLayers)
    {
        worldCamera = camera;
        towerPrefab = prefab;
        selectedTowerData = towerData;
        blockedLayerMask = blockedLayers;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.State != GameState.BuildPhase && GameManager.Instance.State != GameState.AugmentPhase)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceAtMouse();
        }
    }

    public void SelectTower(TowerData towerData)
    {
        selectedTowerData = towerData;
    }

    public Vector3 GetMouseWorldPosition()
    {
        if (worldCamera == null)
        {
            return Vector3.zero;
        }

        Vector3 mouseWorld = worldCamera.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(mouseWorld.x, mouseWorld.y, 0f);
    }

    public bool CanPlaceAt(Vector3 position)
    {
        return CanPlace(position);
    }

    private void TryPlaceAtMouse()
    {
        if (worldCamera == null || towerPrefab == null || selectedTowerData == null)
        {
            return;
        }

        Vector3 position = GetMouseWorldPosition();

        if (!CanPlace(position))
        {
            PlacementFailed?.Invoke(position);
            return;
        }

        if (EconomyManager.Instance != null && !EconomyManager.Instance.TrySpend(selectedTowerData.cost))
        {
            PlacementFailed?.Invoke(position);
            return;
        }

        Tower tower = Instantiate(towerPrefab, position, Quaternion.identity);
        tower.gameObject.SetActive(true);
        tower.Initialize(selectedTowerData);
        PlacementSucceeded?.Invoke(position);
    }

    private bool CanPlace(Vector3 position)
    {
        return Physics2D.OverlapCircle(position, placementRadius, blockedLayerMask) == null;
    }
}
