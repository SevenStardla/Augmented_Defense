using UnityEngine;

[CreateAssetMenu(menuName = "Augmented Defense/Augment Data")]
public sealed class AugmentData : ScriptableObject
{
    public string displayName;
    [TextArea] public string description;
    public AugmentType type;
    public bool canStack = true;
    public float value = 0.1f;
}
