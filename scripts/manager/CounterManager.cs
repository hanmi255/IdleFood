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
}
