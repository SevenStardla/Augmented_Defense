using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class CameraBackdrop : MonoBehaviour
{
    private Camera targetCamera;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Configure(Camera camera)
    {
        targetCamera = camera;
        UpdateLayout();
    }

    private void LateUpdate()
    {
        UpdateLayout();
    }

    private void UpdateLayout()
    {
        if (targetCamera == null || spriteRenderer.sprite == null || !targetCamera.orthographic)
        {
            return;
        }

        float cameraHeight = targetCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * targetCamera.aspect;
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        float coverScale = Mathf.Max(cameraWidth / spriteSize.x, cameraHeight / spriteSize.y);

        transform.position = new Vector3(
            targetCamera.transform.position.x,
            targetCamera.transform.position.y,
            5f);
        transform.localScale = new Vector3(coverScale, coverScale, 1f);
    }
}
