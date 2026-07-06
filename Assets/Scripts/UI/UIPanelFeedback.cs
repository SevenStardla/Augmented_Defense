using UnityEngine;
using UnityEngine.UI;

public sealed class UIPanelFeedback : MonoBehaviour
{
    [SerializeField] private float showDuration = 0.25f;
    [SerializeField] private float targetAlpha = 0.72f;

    private Image image;
    private Vector3 baseScale;
    private float showTimer;
    private bool showing;

    private void Awake()
    {
        image = GetComponent<Image>();
        baseScale = transform.localScale;
    }

    private void OnEnable()
    {
        if (showing)
        {
            showTimer = 0f;
            transform.localScale = baseScale * 0.9f;
            SetAlpha(0f);
        }
    }

    private void Update()
    {
        if (!showing)
        {
            return;
        }

        showTimer += Time.unscaledDeltaTime;
        float t = Mathf.Clamp01(showTimer / showDuration);
        SetAlpha(Mathf.Lerp(0f, targetAlpha, t));
        transform.localScale = Vector3.Lerp(baseScale * 0.9f, baseScale, t);
    }

    public void Show()
    {
        showing = true;
        showTimer = 0f;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        showing = false;
        gameObject.SetActive(false);
    }

    private void SetAlpha(float alpha)
    {
        if (image == null)
        {
            return;
        }

        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
