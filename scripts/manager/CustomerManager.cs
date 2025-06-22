using Godot;
using Godot.Collections;

/// <summary>
/// 顾客管理类，负责顾客生成、位置分配和初始行为控制
/// </summary>
public partial class CustomerManager : Node
{
    /// <summary>
    /// 顾客生成位置标记点数组，定义各可能的生成点
    /// </summary>
    [Export] private Array<Marker2D> _spawnPositions;
    /// <summary>
    /// 顾客外观数据列表，用于随机分配顾客外观
    /// </summary>
    [Export] private Array<CustomerData> _customerSprites;
    /// <summary>
    /// 顾客场景预制体，用于动态生成顾客实例
    /// </summary>
    [Export] private PackedScene _customerScene;

    /// <summary>
    /// 顾客生成定时器，控制生成间隔
    /// </summary>
    private Timer _timer;
    /// <summary>
    /// 柜台管理器引用，用于柜台分配和状态查询
    /// </summary>
    private CounterManager _counterManager;

    /// <summary>
    /// 节点初始化，连接信号和启动定时器
    /// </summary>
    public override void _Ready()
    {
        _counterManager = GetNode<CounterManager>("%CounterManager");

        CallDeferred(nameof(SpawnCustomer));
        _timer = GetNode<Timer>("%SpawnTimer");

        _timer.Timeout += SpawnCustomer;
    }

    /// <summary>
    /// 生成新顾客并设置初始状态
    /// </summary>
    private void SpawnCustomer()
    {
        var customer = _customerScene.Instantiate() as Customer;
        AddChild(customer);

        var spriteData = _customerSprites.PickRandom();
        customer.SetSprites(spriteData);

        var item = GameManager.Instance.GetRandomItem();
        var quantity = new RandomNumberGenerator().RandiRange(1, 3);
        customer.InitItem(item, quantity);

        customer.Position = _spawnPositions.PickRandom().Position;

        if (_counterManager.GetFreeCounter() != -1)
        {
            _counterManager.AssignCustomer(customer);
            customer.MoveToCounter();
        }
        else
        {
            customer.PlayMoveAnim();
            var tween = CreateTween();
            tween.TweenProperty(customer, "position", customer.Position + Vector2.Right * 1350, 5.0);
            tween.Finished += () =>
            {
                customer.QueueFree();
            };
        }
    }
}