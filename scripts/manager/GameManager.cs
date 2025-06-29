using Godot;
using Godot.Collections;

/// <summary>
/// 游戏管理类，负责全局游戏状态、资源管理和事件调度
/// </summary>
public partial class GameManager : Node
{
    /// <summary>
    /// 顾客请求服务事件，当顾客需要服务时触发
    /// </summary>
    /// <param name="customer">发起请求的顾客实例</param>
    [Signal] public delegate void OnCustomerRequestedEventHandler(Customer customer);
    /// <summary>
    /// 顾客订单完成事件，当顾客收到订单时触发
    /// </summary>
    /// <param name="customer">完成订单的顾客实例</param>
    [Signal] public delegate void OnCustomerOrderCompletedEventHandler(Customer customer);
    /// <summary>
    /// 生成新收银员事件，用于通知创建新收银员
    /// </summary>
    [Signal] public delegate void OnSpawnNewCashierEventHandler();

    [Export] public ItemData _itemCoffee;  /// 咖啡物品数据，定义咖啡相关属性
    [Export] public ItemData _itemBurger;  /// 汉堡物品数据，定义汉堡相关属性

    private readonly PackedScene COIN_VFX = GD.Load<PackedScene>("res://scenes/vfx/coin_vfx.tscn");  /// 金币特效预制体，用于展示金币获取动画
    private readonly Vector2 COFFEE_COUNTER_POS = new(415, 1250);                             /// 咖啡柜台世界坐标位置
    private readonly Vector2 BURGER_COUNTER_POS = new(665, 1250);                             /// 汉堡柜台世界坐标位置
    public float currentCoins = 99999;                                                                    /// 当前游戏金币总数
    public static GameManager Instance { get; private set; }                                              /// 游戏管理器单例实例

    /// <summary>
    /// 节点初始化，创建单例并初始化金币数量
    /// </summary>
    public override void _Ready()
    {
        if (Instance == null)
            Instance = this;
        else
            QueueFree();
    }

    /// <summary>
    /// 播放金币特效并播放音效
    /// </summary>
    /// <param name="spawnPos">特效生成位置</param>
    public void PlayCoinVFX(Vector2 spawnPos)
    {
        var particleInstance = COIN_VFX.Instantiate<GpuParticles2D>();
        Instance.GetTree().Root.AddChild(particleInstance);
        SoundManager.Instance.PlayCoins();
        particleInstance.GlobalPosition = spawnPos with { Y = spawnPos.Y - 60 };
        particleInstance.Emitting = true;
        particleInstance.Finished += particleInstance.QueueFree;
    }

    /// <summary>
    /// 随机获取一个物品类型
    /// </summary>
    /// <returns>随机选择的物品数据</returns>
    public ItemData GetRandomItem()
    {
        Array items = [_itemCoffee, _itemBurger];
        return (ItemData)items.PickRandom();
    }

    /// <summary>
    /// 根据物品类型获取对应柜台坐标
    /// </summary>
    /// <param name="item">需要查询的物品数据</param>
    /// <returns>对应柜台的世界坐标</returns>
    public Vector2 GetItemCounterPos(ItemData item)
    {
        return item.type switch
        {
            ItemData.ItemType.Coffee => COFFEE_COUNTER_POS,
            ItemData.ItemType.Burger => BURGER_COUNTER_POS,
            _ => Vector2.Zero,
        };
    }

    /// <summary>
    /// 格式化金币数量
    /// </summary>
    /// <param name="amount"></param>
    /// <returns>格式化后的字符串</returns>
    public static string FormatCoins(float amount)
    {
        var suffixes = new Array { "", "K", "M", "B", "T", "Q" };
        var index = 0;
        var displayAmount = amount;
        while (displayAmount >= 1000 && index < suffixes.Count - 1)
        {
            displayAmount /= 1000;
            index++;
        }
        return RoundToOneDecimal(displayAmount).ToString() + suffixes[index];
    }

    /// <summary>
    /// 将浮点数值四舍五入到小数点后一位
    /// </summary>
    /// <param name="value">需要四舍五入的原始数值</param>
    /// <returns>保留一位小数的四舍五入结果</returns>
    private static double RoundToOneDecimal(float value)
    {
        return Mathf.Floor(value * 10f + 0.5) / 10f;
    }
}