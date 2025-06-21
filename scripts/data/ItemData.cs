using Godot;

[GlobalClass]
public partial class ItemData : Resource
{

    [Signal] public delegate void OnStarReachedEventHandler();

    public enum ItemType
    {
        Coffee,
        Burger
    }

    [Export] public string id;
    [Export] public ItemType type;
    [Export] public Texture2D sprite;

    [ExportGroup("Cook")]
    [Export] public float cookTime = 10.0f;
    [Export] public float cookTimeReducePercent = 0.50f;

    [ExportGroup("Upgrade")]
    [Export] public float upgradeCost = 3.0f;
    [Export] public float upgradeMultiplier = 1.3f;

    [ExportGroup("Profit")]
    [Export] public float profit = 4.0f;
    [Export] public float profitMultiplier = 1.2f;

    [ExportCategory("Level")]
    [Export] public int maxLevel = 50;

    public int currentLevel = 0;

    public void UpdateItem()
    {
        if (currentLevel >= maxLevel)
            return;

        currentLevel += 1;
        upgradeCost = Mathf.Ceil(upgradeCost * upgradeMultiplier);
        profit = Mathf.Ceil(profit * profitMultiplier);
        if (currentLevel % 25 == 0)
        {
            cookTime = Mathf.Ceil(cookTime * cookTimeReducePercent);
            upgradeCost *= 3;
            profit *= 3;
            EmitSignal("OnStarReached");
        }
    }
}
