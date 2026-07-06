using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public sealed class UITextFeedback : MonoBehaviour
{
    [SerializeField] private float flashDuration = 0.22f;
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeAmount = 6f;
    [SerializeField] private Color dangerColor = new Color(1f, 0.28f, 0.25f, 1f);

    private Text text;
    private Color baseColor;
    private Vector3 baseScale;
    private Vector2 basePosition;
    private RectTransform rectTransform;
    private Color flashColor;
    private float flashScale = 1f;
    private float flashTimer;
    private float shakeTimer;
    private bool danger;
    private bool dimmed;

    private void Awake()
    {
        text = GetComponent<Text>();
        rectTransform = text.rectTransform;
        baseColor = text.color;
        baseScale = rectTransform.localScale;
        basePosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        Color targetColor = dimmed ? baseColor * 0.7f : danger ? dangerColor : baseColor;
        targetColor.a = dimmed ? 0.7f : baseColor.a;

        if (flashTimer > 0f)
        {
            flashTimer -= Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(flashTimer / flashDuration);
            text.color = Color.Lerp(targetColor, flashColor, t);
            rectTransform.localScale = Vector3.Lerp(baseScale, baseScale * flashScale, t);
        }
        else
        {
            text.color = Color.Lerp(text.color, targetColor, Time.unscaledDeltaTime * 12f);
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, baseScale, Time.unscaledDeltaTime * 14f);
        }

        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.unscaledDeltaTime;
            float strength = Mathf.Clamp01(shakeTimer / shakeDuration) * shakeAmount;
            rectTransform.anchoredPosition = basePosition + new Vector2(Mathf.Sin(Time.unscaledTime * 80f) * strength, 0f);
        }
        else
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, basePosition, Time.unscaledDeltaTime * 18f);
        }
    }

    public void PlayFlash(Color color, float scale)
    {
        flashColor = color;
        flashScale = scale;
        flashTimer = flashDuration;
    }

    public void CaptureBaseState()
    {
        baseColor = text.color;
        baseScale = rectTransform.localScale;
        basePosition = rectTransform.anchoredPosition;
    }

    public void PlayShake()
    {
        shakeTimer = shakeDuration;
    }

    public void SetDanger(bool isDanger)
    {
        danger = isDanger;
    }

    public void SetDimmed(bool isDimmed)
    {
        dimmed = isDimmed;
    }
}
