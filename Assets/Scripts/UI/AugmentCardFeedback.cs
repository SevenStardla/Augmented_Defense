using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(Image))]
public sealed class AugmentCardFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private static readonly Color HoverTint = new Color(1.18f, 1.18f, 1.18f, 1f);

    private Button button;
    private Image image;
    private Color baseColor;
    private Vector3 baseScale;
    private bool pointerOver;
    private bool pointerDown;
    private float selectionTimer;

    public const float SelectionFeedbackDuration = 0.14f;

    private void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        button.transition = Selectable.Transition.None;
        baseColor = image.color;
        baseScale = transform.localScale;
    }

    private void OnEnable()
    {
        pointerOver = false;
        pointerDown = false;
        selectionTimer = 0f;
        transform.localScale = baseScale;
        image.color = baseColor;
    }

    private void Update()
    {
        bool showingSelection = selectionTimer > 0f;
        if (showingSelection)
        {
            selectionTimer -= Time.unscaledDeltaTime;
        }

        bool canInteract = button != null && button.interactable;
        float scale = showingSelection ? 0.94f : canInteract && pointerDown ? 0.97f : canInteract && pointerOver ? 1.04f : 1f;
        Color targetColor = showingSelection || canInteract && pointerOver ? baseColor * HoverTint : baseColor;
        targetColor.a = baseColor.a;

        transform.localScale = Vector3.Lerp(transform.localScale, baseScale * scale, Time.unscaledDeltaTime * 18f);
        image.color = Color.Lerp(image.color, targetColor, Time.unscaledDeltaTime * 18f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerOver = false;
        pointerDown = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button != null && button.interactable)
        {
            pointerDown = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;
    }

    public void PlaySelectionFeedback()
    {
        pointerDown = false;
        selectionTimer = SelectionFeedbackDuration;
    }
}
