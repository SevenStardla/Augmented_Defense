using UnityEngine;

[CreateAssetMenu(menuName = "Augmented Defense/Augment Data")]
public sealed class AugmentData : ScriptableObject
{
    public string id;
    public string displayName;
    [TextArea] public string description;
    public AugmentType type;
    public bool canStack = true;
    [Min(1)] public int maxStacks = 1;
    public float value = 0.1f;

    public string Key => string.IsNullOrWhiteSpace(id) ? name : id;
}
