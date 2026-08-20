using UnityEngine;
using System;
using UnityEngine.EventSystems;

public sealed class TowerPlacement : MonoBehaviour
{
    [SerializeField] private Camera worldCamera;
    [SerializeField] private Tower towerPrefab;
    [SerializeField] private TowerData selectedTowerData;
    [SerializeField] private LayerMask blockedLayerMask;
    [SerializeField] private float placementRadius = 0.35f;
    [SerializeField] private bool restrictToPlacementBounds;
    [SerializeField] private Rect placementBounds;

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

    public void Configure(Camera camera, Tower prefab, TowerData towerData, LayerMask blockedLayers, Rect allowedBounds)
    {
        worldCamera = camera;
        towerPrefab = prefab;
        selectedTowerData = towerData;
        blockedLayerMask = blockedLayers;
        placementBounds = allowedBounds;
        restrictToPlacementBounds = true;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.State != GameState.BuildPhase)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && !IsPointerOverUi())
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

    public bool IsPointerOverUi()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
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
        if (restrictToPlacementBounds && !placementBounds.Contains(new Vector2(position.x, position.y)))
        {
            return false;
        }

        return Physics2D.OverlapCircle(position, placementRadius, blockedLayerMask) == null;
    }
}
