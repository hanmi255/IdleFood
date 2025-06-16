using Godot;
using Godot.Collections;

public partial class CustomerManager : Node
{
    [Export] public Array<Marker2D> spawnPositions;
    [Export] public Array<CustomerData> customerSprites;
    [Export] public PackedScene customerScene;

    private Timer _timer;
    private CounterManager _counterManager;

    public override void _Ready()
    {
        _counterManager = GetNode<CounterManager>("%CounterManager");

        CallDeferred(nameof(SpawnCustomer));
        _timer = GetNode<Timer>("%SpawnTimer");

        _timer.Timeout += () =>
        {
            SpawnCustomer();
        };
    }

    private void SpawnCustomer()
    {
        var customer = customerScene.Instantiate() as Customer;
        AddChild(customer);

        // 设置Sprite
        var spriteData = customerSprites.PickRandom();
        customer.SetSprites(spriteData);

        // 设置Item 数量
        var item = GameManager.GetRandomItem();
        var quantity = new RandomNumberGenerator().RandiRange(1, 3);
        customer.InitItem(item, quantity);

        // 随机生成Customer 位置
        customer.Position = spawnPositions.PickRandom().Position;

        // 分配空闲的Counter
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