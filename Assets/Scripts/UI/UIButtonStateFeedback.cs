using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public sealed class UIButtonStateFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool disableDuringWave = true;
    [SerializeField] private bool hideWhenCleared;
    [SerializeField] private float pulseAmount = 0.04f;

    private Button button;
    private Image image;
    private Color baseColor;
    private Vector3 baseScale;
    private bool pointerOver;
    private bool pointerDown;

    public void Configure(bool shouldDisableDuringWave, bool shouldHideWhenCleared)
    {
        disableDuringWave = shouldDisableDuringWave;
        hideWhenCleared = shouldHideWhenCleared;

        if (GameManager.Instance != null)
        {
            HandleStateChanged(GameManager.Instance.State);
        }
    }

    private void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        baseScale = transform.localScale;
        baseColor = image != null ? image.color : Color.white;
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StateChanged += HandleStateChanged;
            HandleStateChanged(GameManager.Instance.State);
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StateChanged -= HandleStateChanged;
        }
    }

    private void Update()
    {
        float pulse = button.interactable ? 1f + Mathf.Sin(Time.unscaledTime * 5.2f) * pulseAmount : 1f;
        float press = pointerDown ? 0.94f : pointerOver ? 1.04f : 1f;
        transform.localScale = Vector3.Lerp(transform.localScale, baseScale * pulse * press, Time.unscaledDeltaTime * 16f);

        if (image != null)
        {
            Color target = button.interactable ? baseColor : baseColor * 0.55f;
            target.a = button.interactable ? baseColor.a : 0.65f;
            image.color = Color.Lerp(image.color, target, Time.unscaledDeltaTime * 14f);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;
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

    private void HandleStateChanged(GameState state)
    {
        if (hideWhenCleared && state == GameState.Clear)
        {
            gameObject.SetActive(false);
            return;
        }

        if (disableDuringWave)
        {
            button.interactable = state == GameState.BuildPhase || state == GameState.AugmentPhase;
        }
    }
}
