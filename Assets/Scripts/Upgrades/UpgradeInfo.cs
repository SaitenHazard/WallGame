using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "UpgradeInfo", menuName = "UpgradeInfo")]
public class UpgradeInfo : ScriptableObject
{
    public Upgrade upgradePrefab;
    public UpgradeName id;
    public string title;
    public string subtitle;

    public Rarity rarety;

    [TextArea]
    public string description;

    [Header("For Active Upgrades")]
    public float cooldown;

    [Header("Images")]
    public Sprite foreground;
    public Sprite glow;
    public Sprite background;

    [Header("Smite me down, Odin")]
    public int lineCount = 1;
}

public enum Rarity
{
    Common, Rare, Epic
}