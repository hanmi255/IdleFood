using System.Threading.Tasks;
using Godot;

public partial class Cashier : Node2D
{
    [Export] public float moveSpeed = 50.0f;

    [Signal] public delegate void OnOrderCompletedEventHandler(Cashier cashier);

    private AnimationPlayer _animationPlayer;
    private CookBar _cookBar;

    private Vector2 _counterPos;
    private ItemData _itemRequest;
    private Vector2 _itemCounterPos;

    public Customer CurrentCustomer { get; private set; }

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _cookBar = GetNode<CookBar>("CookBar");
        _cookBar.OnCookFinished += async () =>
        {
            _cookBar.Hide();
            _cookBar.ResetBar();
            await DeliverOrder();
        };
    }

    public void SetCustomer(Customer customer)
    {
        CurrentCustomer = customer;
        customer.beingServed = true;
        _counterPos = customer.Position with { Y = customer.Position.Y + 160 };
        _itemRequest = customer.RequestItem;
        _itemCounterPos = GameManager.GetItemCounterPos(_itemRequest);
    }

    public async Task TakeOrder()
    {
        MoveToCounter();
        await Task.Delay(1100);
        CurrentCustomer.ShowOrderUI();
        MoveToItemPos();
    }

    private void MoveToCounter()
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "position", _counterPos, 1.0f);
        _animationPlayer.Play("move");
    }

    private void MoveToItemPos()
    {
        var tween = CreateTween();
        tween.TweenInterval(0.5);
        tween.TweenProperty(this, "position", _itemCounterPos, 1.0f);
        _animationPlayer.Play("move");
        tween.TweenInterval(0.05);
        tween.Finished += StartCookTime;
    }

    private async Task DeliverOrder()
    {
        MoveToCounter();
        await Task.Delay(1100);
        CurrentCustomer.ReceiveOrder();
        GameManager.Instance.currentCoins += _itemRequest.profit;
        GameManager.PlayCoinVFX(GlobalPosition);

        if (CurrentCustomer.currentOrderStatus > 0)
            MoveToItemPos();
        else
        {
            _animationPlayer.Play("idle");
            CurrentCustomer = null;
            EmitSignal("OnOrderCompleted", this);
        }
    }

    private void StartCookTime()
    {
        _animationPlayer.Play("idle");
        _cookBar.Show();
        _cookBar.CookItem(_itemRequest.cookTime);
    }
}