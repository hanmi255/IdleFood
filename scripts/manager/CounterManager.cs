using System.Linq;
using Godot;
using Godot.Collections;

public partial class CounterManager : Node
{
    [Export] public Array<Marker2D> counterPositions;

    private Dictionary<int, Customer> _counter = new()
    {
        { 0, null },
        { 1, null },
        { 2, null },
        { 3, null }
    };
    private int _lastCustomerIndex = -1;

    public override void _Ready()
    {
        GameManager.Instance.OnCustomerOrderCompleted += OnCustomerOrderCompleted;
    }

    public int GetFreeCounter()
    {
        foreach (var counter in _counter)
        {
            if (counter.Value == null)
                return counter.Key;
        }
        return -1;
    }

    public void AssignCustomer(Customer customer)
    {
        var index = GetFreeCounter();
        if (index != -1)
        {
            customer.counterPos = counterPositions[index].Position;
            _counter[index] = customer;
        }
    }

    public Customer GetNextAvailableCustomer()
    {
        var keys = _counter.Keys.OrderBy(k => k).ToList();
        for (int i = 0; i < keys.Count; i++)
        {
            _lastCustomerIndex = (_lastCustomerIndex + 1) % keys.Count;
            if (_counter.TryGetValue(keys[_lastCustomerIndex], out var customer) && 
                customer is Customer c && 
                c._waitingOrder && !c.beingServed)
            {
                return c;
            }
        }
        return null;
    }

    private void OnCustomerOrderCompleted(Customer customer)
    {
        foreach (var index in _counter)
        {
            if (index.Value == customer)
                _counter[index.Key] = null;
        }
    }
}
