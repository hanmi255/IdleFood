using System.Linq;
using Godot;
using Godot.Collections;

/// <summary>
/// 柜台管理类，负责顾客分配、柜台管理和订单完成处理
/// </summary>
public partial class CounterManager : Node
{
    [Export] private Array<Marker2D> _counterPositions;  /// 柜台位置标记点数组，定义各柜台的坐标位置

    private Dictionary<int, Customer> _counter = new()  /// 柜台顾客映射表，记录各柜台当前服务的顾客
    {
        { 0, null },
        { 1, null },
        { 2, null },
        { 3, null }
    };

    private int _lastCustomerIndex = -1;  /// 上一个分配的顾客索引，用于轮询分配算法

    /// <summary>
    /// 节点初始化，连接订单完成信号
    /// </summary>
    public override void _Ready()
    {
        GameManager.Instance.OnCustomerOrderCompleted += OnCustomerOrderCompleted;
    }

    /// <summary>
    /// 获取第一个空闲柜台索引
    /// </summary>
    /// <returns>空闲柜台索引，-1表示无空闲</returns>
    public int GetFreeCounter()
    {
        foreach (var counter in _counter)
        {
            if (counter.Value == null)
                return counter.Key;
        }
        return -1;
    }

    /// <summary>
    /// 将顾客分配到空闲柜台
    /// </summary>
    /// <param name="customer">需要分配的顾客实例</param>
    public void AssignCustomer(Customer customer)
    {
        var index = GetFreeCounter();
        if (index != -1)
        {
            customer.counterPos = _counterPositions[index].Position;
            _counter[index] = customer;
        }
    }

    /// <summary>
    /// 获取下一个可服务的顾客（轮询算法）
    /// </summary>
    /// <returns>下一个可服务的顾客实例</returns>
    public Customer GetNextAvailableCustomer()
    {
        var keys = _counter.Keys.OrderBy(k => k).ToList();
        for (int i = 0; i < keys.Count; i++)
        {
            _lastCustomerIndex = (_lastCustomerIndex + 1) % keys.Count;
            if (_counter.TryGetValue(keys[_lastCustomerIndex], out var customer) &&
                customer is Customer c &&
                c._waitingOrder && !c.isBeingServed)
            {
                return c;
            }
        }
        return null;
    }

    /// <summary>
    /// 处理订单完成事件，释放对应柜台
    /// </summary>
    /// <param name="customer">完成订单的顾客实例</param>
    private void OnCustomerOrderCompleted(Customer customer)
    {
        foreach (var index in _counter)
        {
            if (index.Value == customer)
                _counter[index.Key] = null;
        }
    }
}
