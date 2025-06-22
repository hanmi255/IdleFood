using Godot;

/// <summary>
/// 物品数据类，管理物品属性、升级逻辑和经济参数
/// </summary>
[GlobalClass]
public partial class ItemData : Resource
{
    /// <summary>
    /// 星级升级事件，在达到特定升级条件时触发
    /// </summary>
    [Signal] public delegate void OnStarReachedEventHandler();

    /// <summary>
    /// 物品类型枚举，定义可扩展的物品分类
    /// </summary>
    public enum ItemType
    {
        Coffee,
        Burger
    }

    /// <summary>
    /// 物品唯一标识符
    /// </summary>
    [Export] public string id;
    /// <summary>
    /// 物品类型，定义物品的基础分类
    /// </summary>
    [Export] public ItemType type;
    /// <summary>
    /// 物品显示用的纹理资源
    /// </summary>
    [Export] public Texture2D sprite;

    /// <summary>
    /// 烹饪时间参数，单位秒
    /// </summary>
    [ExportGroup("Cook")]
    [Export] public float cookTime = 10.0f;
    /// <summary>
    /// 烹饪时间减少百分比，用于升级计算
    /// </summary>
    [Export] private float _cookTimeReducePercent = 0.50f;

    /// <summary>
    /// 升级基础成本
    /// </summary>
    [ExportGroup("Upgrade")]
    [Export] public float upgradeCost = 3.0f;
    /// <summary>
    /// 升级成本乘数，控制升级成本增长曲线
    /// </summary>
    [Export] private float _upgradeMultiplier = 1.3f;

    /// <summary>
    /// 基础利润值
    /// </summary>
    [ExportGroup("Profit")]
    [Export] public float profit = 4.0f;
    /// <summary>
    /// 利润乘数，控制利润增长曲线
    /// </summary>
    [Export] private float _profitMultiplier = 1.2f;

    /// <summary>
    /// 最大等级限制
    /// </summary>
    [ExportCategory("Level")]
    [Export] private int _maxLevel = 50;

    /// <summary>
    /// 当前物品等级
    /// </summary>
    public int currentLevel = 0;

    /// <summary>
    /// 执行物品升级逻辑，更新所有相关参数
    /// </summary>
    public void UpdateItem()
    {
        if (currentLevel >= _maxLevel)
            return;

        currentLevel += 1;
        upgradeCost = Mathf.Ceil(upgradeCost * _upgradeMultiplier);
        profit = Mathf.Ceil(profit * _profitMultiplier);
        if (currentLevel % 25 == 0)
        {
            cookTime = Mathf.Ceil(cookTime * _cookTimeReducePercent);
            upgradeCost *= 3;
            profit *= 3;
            EmitSignal("OnStarReached");
        }
    }
}
