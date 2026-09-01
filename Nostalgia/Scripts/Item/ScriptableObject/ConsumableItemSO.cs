using UnityEngine;
using UnityEngine.Localization;

public abstract class ConsumableItemSO : ScriptableObject
{
    [Header("Default Item Config")]
    [SerializeField] protected LocalizedString itemName;
    public LocalizedString ItemName => itemName;

    [SerializeField] protected LocalizedString description;
    public LocalizedString Description => description;

    [SerializeField] protected Sprite icon;
    public Sprite Icon => icon;

    public abstract void Use(Player usingPlayer);
}