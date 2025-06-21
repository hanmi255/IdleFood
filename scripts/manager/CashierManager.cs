using System.Linq;
using Godot;
using Godot.Collections;

public partial class CashierManager : Node
{
    [Export] public PackedScene cashierScene;
    [Export] public Marker2D spawnPos;

    private Array<Cashier> _cashierList = [];
    private CounterManager _counterManager;

    public override void _Ready()
    {
        _counterManager = GetNode<CounterManager>("%CounterManager");
        GameManager.Instance.OnCustomerRequested += OnCustomerRequested;
        AddCashier();
    }

    public void AddCashier()
    {
        var cashier = cashierScene.Instantiate<Cashier>();
        cashier.OnOrderCompleted += OnOrderCompleted;
        AddChild(cashier);
        cashier.Position = spawnPos.Position;
        _cashierList.Add(cashier);
    }

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

    private async void OnOrderCompleted(Cashier cashier)
    {
        if (_counterManager.GetNextAvailableCustomer() is Customer customer)
        {
            cashier.SetCustomer(customer);
            await cashier.TakeOrder();
        }
    }
}
