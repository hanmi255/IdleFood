using System.Linq;
using Godot;
using Godot.Collections;

/// <summary>
/// 收银员管理类，负责收银员的生成、调度和订单分配
/// </summary>
public partial class CashierManager : Node
{
    [Export] private PackedScene _cashierScene;  /// 收银员场景预制体，用于动态生成收银员实例
    [Export] private Marker2D _spawnPos;         /// 生成位置标记点，确定收银员初始坐标

    private Array<Cashier> _cashierList = [];  /// 收银员实例列表，存储当前所有活跃的收银员
    private CounterManager _counterManager;    /// 柜台管理器引用，用于获取待服务顾客

    /// <summary>
    /// 节点初始化，连接信号和初始化第一个收银员
    /// </summary>
    public override void _Ready()
    {
        _counterManager = GetNode<CounterManager>("%CounterManager");
        GameManager.Instance.OnCustomerRequested += OnCustomerRequested;
        GameManager.Instance.OnSpawnNewCashier += AddCashier;
        AddCashier();
    }

    /// <summary>
    /// 添加新的收银员实例到场景中
    /// </summary>
    public void AddCashier()
    {
        var cashier = _cashierScene.Instantiate<Cashier>();
        cashier.OnOrderCompleted += OnOrderCompleted;
        AddChild(cashier);
        cashier.Position = _spawnPos.Position;
        _cashierList.Add(cashier);
    }

    /// <summary>
    /// 处理顾客请求事件，分配空闲收银员
    /// </summary>
    /// <param name="customer">请求服务的顾客实例</param>
    private async void OnCustomerRequested(Customer customer)
    {
        var freeCashier = new Array<Cashier>(_cashierList.Where(c => c.CurrentCustomer == null));
        if (freeCashier.Count == 0)
            return;

        var randomCashier = freeCashier.PickRandom();
        if (randomCashier != null)
        {
            randomCashier.SetCustomer(customer);
            await randomCashier.TakeOrder();
        }
    }

    /// <summary>
    /// 处理订单完成事件，继续服务下一个顾客
    /// </summary>
    /// <param name="cashier">完成订单的收银员实例</param>
    private async void OnOrderCompleted(Cashier cashier)
    {
        if (_counterManager.GetNextAvailableCustomer() is Customer customer)
        {
            cashier.SetCustomer(customer);
            await cashier.TakeOrder();
        }
    }
}
