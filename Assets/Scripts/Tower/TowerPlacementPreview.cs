using UnityEngine;

[RequireComponent(typeof(TowerPlacement))]
public sealed class TowerPlacementPreview : MonoBehaviour
{
    [SerializeField] private Color validColor = new Color(0.35f, 1f, 0.45f, 0.45f);
    [SerializeField] private Color invalidColor = new Color(1f, 0.2f, 0.15f, 0.45f);
    [SerializeField] private float ringWidth = 0.035f;
    [SerializeField] private int ringSegments = 64;
    [SerializeField] private float resultDuration = 0.28f;
    [SerializeField] private Color successResultColor = new Color(0.3f, 1f, 0.48f, 0.95f);
    [SerializeField] private Color failureResultColor = new Color(1f, 0.18f, 0.12f, 0.95f);

    private TowerPlacement placement;
    private SpriteRenderer ghostRenderer;
    private LineRenderer rangeRenderer;
    private LineRenderer resultRenderer;
    private float failureTimer;
    private float resultTimer;
    private Color resultColor;

    public void Configure(Sprite previewSprite, Vector3 previewScale)
    {
        if (previewSprite != null)
        {
            ghostRenderer.sprite = previewSprite;
        }

        ghostRenderer.transform.localScale = previewScale;
    }

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
        UpdateResultFeedback();

        bool visible = (GameManager.Instance == null || GameManager.Instance.State == GameState.BuildPhase) &&
            !placement.IsPointerOverUi();

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

        GameObject result = new GameObject("Tower Placement Result");
        resultRenderer = result.AddComponent<LineRenderer>();
        resultRenderer.useWorldSpace = false;
        resultRenderer.loop = true;
        resultRenderer.positionCount = ringSegments;
        resultRenderer.startWidth = ringWidth * 2.2f;
        resultRenderer.endWidth = ringWidth * 2.2f;
        resultRenderer.material = new Material(Shader.Find("Sprites/Default"));
        resultRenderer.sortingOrder = 12;
        resultRenderer.enabled = false;
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
        PlayResultFeedback(position, failureResultColor);
    }

    private void HandlePlacementSucceeded(Vector3 position)
    {
        failureTimer = 0f;
        PlayResultFeedback(position, successResultColor);
    }

    private void PlayResultFeedback(Vector3 position, Color color)
    {
        resultRenderer.transform.position = position;
        resultColor = color;
        resultTimer = resultDuration;
        resultRenderer.enabled = true;
    }

    private void UpdateResultFeedback()
    {
        if (resultTimer <= 0f)
        {
            resultRenderer.enabled = false;
            return;
        }

        resultTimer -= Time.unscaledDeltaTime;
        float progress = 1f - Mathf.Clamp01(resultTimer / resultDuration);
        float radius = Mathf.Lerp(0.22f, 0.62f, progress);
        Color fadingColor = resultColor;
        fadingColor.a *= 1f - progress;
        resultRenderer.startColor = fadingColor;
        resultRenderer.endColor = fadingColor;

        for (int i = 0; i < ringSegments; i++)
        {
            float angle = i / (float)ringSegments * Mathf.PI * 2f;
            resultRenderer.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f));
        }
    }

    private Sprite CreateSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
    }
}
