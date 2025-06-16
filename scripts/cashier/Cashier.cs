using System.Threading.Tasks;
using Godot;

public partial class Cashier : Node2D
{
    [Export] public float moveSpeed = 50.0f;
    private AnimationPlayer _animationPlayer;

    public Customer currentCustomer;
    public Vector2 counterPos;

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void SetCustomer(Customer customer)
    {
        currentCustomer = customer;
        customer.beingServed = true;
        counterPos = new Vector2(customer.Position.X, customer.Position.Y + 160);
    }

    public async Task TakeOrderAsync()
    {
        MoveToCounter();
        await Task.Delay(1100);
        currentCustomer.ShowOrderUI();
        MoveToItemPos();
    }

    private void MoveToCounter()
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "position", counterPos, 1.0f);
        _animationPlayer.Play("move");
    }

    // TODO:
    // Create tween
    // Move to item position
    private void MoveToItemPos()
    {
        _animationPlayer.Play("move");
    }
}