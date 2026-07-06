using UnityEngine;

[RequireComponent(typeof(TowerPlacement))]
public sealed class TowerPlacementPreview : MonoBehaviour
{
    [SerializeField] private Color validColor = new Color(0.35f, 1f, 0.45f, 0.45f);
    [SerializeField] private Color invalidColor = new Color(1f, 0.2f, 0.15f, 0.45f);
    [SerializeField] private float ringWidth = 0.035f;
    [SerializeField] private int ringSegments = 64;

    private TowerPlacement placement;
    private SpriteRenderer ghostRenderer;
    private LineRenderer rangeRenderer;
    private float failureTimer;

    private void Awake()
    {
        placement = GetComponent<TowerPlacement>();
        CreatePreviewObjects();
    }

    private void OnEnable()
    {
        placement.PlacementFailed += HandlePlacementFailed;
        placement.PlacementSucceeded += HandlePlacementSucceeded;
    }

    private void OnDisable()
    {
        placement.PlacementFailed -= HandlePlacementFailed;
        placement.PlacementSucceeded -= HandlePlacementSucceeded;
    }

    private void Update()
    {
        bool visible = GameManager.Instance == null ||
            GameManager.Instance.State == GameState.BuildPhase ||
            GameManager.Instance.State == GameState.AugmentPhase;

        if (!visible || placement.SelectedTowerData == null)
        {
            SetVisible(false);
            return;
        }

        SetVisible(true);
        Vector3 position = placement.GetMouseWorldPosition();
        ghostRenderer.transform.position = position;
        rangeRenderer.transform.position = position;

        bool canAfford = EconomyManager.Instance == null || EconomyManager.Instance.CanAfford(placement.SelectedTowerData.cost);
        bool canPlace = placement.CanPlaceAt(position) && canAfford;
        Color color = failureTimer > 0f ? invalidColor : canPlace ? validColor : invalidColor;
        ghostRenderer.color = color;
        rangeRenderer.startColor = color;
        rangeRenderer.endColor = color;

        failureTimer = Mathf.Max(0f, failureTimer - Time.deltaTime);
        DrawRing(placement.SelectedTowerData.range);
    }

    private void CreatePreviewObjects()
    {
        GameObject ghost = new GameObject("Tower Placement Ghost");
        ghostRenderer = ghost.AddComponent<SpriteRenderer>();
        ghostRenderer.sprite = CreateSprite();
        ghostRenderer.sortingOrder = 4;
        ghostRenderer.enabled = false;

        GameObject range = new GameObject("Tower Placement Range");
        rangeRenderer = range.AddComponent<LineRenderer>();
        rangeRenderer.useWorldSpace = false;
        rangeRenderer.loop = true;
        rangeRenderer.positionCount = ringSegments;
        rangeRenderer.startWidth = ringWidth;
        rangeRenderer.endWidth = ringWidth;
        rangeRenderer.material = new Material(Shader.Find("Sprites/Default"));
        rangeRenderer.enabled = false;
    }

    private void DrawRing(float radius)
    {
        for (int i = 0; i < ringSegments; i++)
        {
            float angle = i / (float)ringSegments * Mathf.PI * 2f;
            rangeRenderer.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f));
        }
    }

    private void SetVisible(bool visible)
    {
        ghostRenderer.enabled = visible;
        rangeRenderer.enabled = visible;
    }

    private void HandlePlacementFailed(Vector3 position)
    {
        failureTimer = 0.15f;
    }

    private void HandlePlacementSucceeded(Vector3 position)
    {
        failureTimer = 0f;
    }

    private Sprite CreateSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
    }
}
