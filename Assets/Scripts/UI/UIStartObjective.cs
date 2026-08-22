using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public sealed class UIStartObjective : MonoBehaviour
{
    [SerializeField] private float holdDuration = 4.5f;
    [SerializeField] private float fadeDuration = 1.5f;

    private Text label;
    private Color baseColor;
    private float age;

    private void Awake()
    {
        label = GetComponent<Text>();
        baseColor = label.color;
    }

    private void Update()
    {
        age += Time.unscaledDeltaTime;
        if (age <= holdDuration)
        {
            return;
        }

        float fadeProgress = Mathf.Clamp01((age - holdDuration) / fadeDuration);
        Color color = baseColor;
        color.a = 1f - fadeProgress;
        label.color = color;

        if (fadeProgress >= 1f)
        {
            gameObject.SetActive(false);
        }
    }
}
