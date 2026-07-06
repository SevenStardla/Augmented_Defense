using UnityEngine;

public sealed class TowerPlacement : MonoBehaviour
{
    [SerializeField] private Camera worldCamera;
    [SerializeField] private Tower towerPrefab;
    [SerializeField] private TowerData selectedTowerData;
    [SerializeField] private LayerMask blockedLayerMask;
    [SerializeField] private float placementRadius = 0.35f;

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

    private void TryPlaceAtMouse()
    {
        if (worldCamera == null || towerPrefab == null || selectedTowerData == null)
        {
            return;
        }

        Vector3 mouseWorld = worldCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 position = new Vector3(mouseWorld.x, mouseWorld.y, 0f);

        if (!CanPlace(position))
        {
            return;
        }

        if (EconomyManager.Instance != null && !EconomyManager.Instance.TrySpend(selectedTowerData.cost))
        {
            return;
        }

        Tower tower = Instantiate(towerPrefab, position, Quaternion.identity);
        tower.gameObject.SetActive(true);
        tower.Initialize(selectedTowerData);
    }

    private bool CanPlace(Vector3 position)
    {
        return Physics2D.OverlapCircle(position, placementRadius, blockedLayerMask) == null;
    }
}
