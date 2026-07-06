using UnityEngine;

[RequireComponent(typeof(Tower))]
public sealed class TowerRangePreview : MonoBehaviour
{
    [SerializeField] private Color rangeColor = new Color(1f, 0.82f, 0.25f, 0.22f);
    [SerializeField] private float lineWidth = 0.025f;
    [SerializeField] private int segments = 72;

    private Tower tower;
    private LineRenderer ring;
    private SpriteRenderer spriteRenderer;
    private bool pointerOver;

    private void Awake()
    {
        tower = GetComponent<Tower>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        CreateRing();
    }

    private void Update()
    {
        if (ring == null)
        {
            return;
        }

        bool visible = pointerOver || GameManager.Instance == null || GameManager.Instance.State == GameState.BuildPhase || GameManager.Instance.State == GameState.AugmentPhase;
        ring.enabled = visible;
        if (visible)
        {
            DrawRing(tower.Range);
        }
    }

    private void OnMouseEnter()
    {
        pointerOver = true;
    }

    private void OnMouseExit()
    {
        pointerOver = false;
    }

    private void CreateRing()
    {
        GameObject ringObject = new GameObject("Tower Range Preview");
        ringObject.transform.SetParent(transform, false);
        ring = ringObject.AddComponent<LineRenderer>();
        ring.useWorldSpace = false;
        ring.loop = true;
        ring.positionCount = segments;
        ring.startWidth = lineWidth;
        ring.endWidth = lineWidth;
        ring.material = new Material(Shader.Find("Sprites/Default"));
        ring.startColor = rangeColor;
        ring.endColor = rangeColor;
        ring.sortingOrder = spriteRenderer != null ? spriteRenderer.sortingOrder - 1 : 0;
        ring.enabled = false;
    }

    private void DrawRing(float radius)
    {
        for (int i = 0; i < segments; i++)
        {
            float angle = i / (float)segments * Mathf.PI * 2f;
            ring.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f));
        }
    }
}
