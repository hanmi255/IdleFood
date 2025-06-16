using System.Linq;
using Godot;
using Godot.Collections;

public partial class CashierManager : Node
{
    [Export] public PackedScene cashierScene;
    [Export] public Marker2D spawnPos;

    private Array<Cashier> _cashierList = [];

    public override void _Ready()
    {
        GameManager gameManager = GetNode<GameManager>("/root/GameManager");
        gameManager.OnCustomerRequested += OnCustomerRequested;
        AddCashier();
    }

    public void AddCashier()
    {
        var cashier = cashierScene.Instantiate<Cashier>();
        AddChild(cashier);
        cashier.Position = spawnPos.Position;
        _cashierList.Add(cashier);
    }

    private async void OnCustomerRequested(Customer customer)
    {
        var freeCashier = new Array<Cashier>(_cashierList.Where(c => c.currentCustomer == null));
        if (freeCashier.Count == 0)
            return;

        var randomCashier = freeCashier.PickRandom();
        if (randomCashier != null)
        {
            randomCashier.SetCustomer(customer);
            await randomCashier.TakeOrderAsync();
        }
    }
}
