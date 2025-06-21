using Godot;
using Godot.Collections;

public partial class GameManager : Node
{
    [Signal] public delegate void OnCustomerRequestedEventHandler(Customer customer);
    [Signal] public delegate void OnCustomerOrderCompletedEventHandler(Customer customer);

    public static readonly ItemData ITEM_BURGER = GD.Load<ItemData>("res://data/item/item_burger.tres");
    public static readonly ItemData ITEM_COFFEE = GD.Load<ItemData>("res://data/item/item_coffee.tres");
    private static readonly PackedScene COIN_VFX = GD.Load<PackedScene>("res://scenes/vfx/coin_vfx.tscn");
    private static readonly Vector2 COFFEE_COUNTER_POS = new(415, 1250);
    private static readonly Vector2 BURGER_COUNTER_POS = new(665, 1250);

    public float currentCoins = 9999;

    public static GameManager Instance { get; private set; }

    public override void _Ready()
    {
        if (Instance == null)
            Instance = this;
        else
            QueueFree();
    }

    public static void PlayCoinVFX(Vector2 spawnPos)
    {
        var particleInstance = COIN_VFX.Instantiate<GpuParticles2D>();
        Instance.GetTree().Root.AddChild(particleInstance);
        SoundManager.Instance.PlayCoins();
        particleInstance.GlobalPosition = spawnPos with { Y = spawnPos.Y - 60 };
        particleInstance.Emitting = true;
        particleInstance.Finished += particleInstance.QueueFree;
    }

    public static ItemData GetRandomItem()
    {
        Array items = [ITEM_BURGER, ITEM_COFFEE];
        return (ItemData)items.PickRandom();
    }

    public static Vector2 GetItemCounterPos(ItemData item)
    {
        return item.type switch
        {
            ItemData.ItemType.Coffee => COFFEE_COUNTER_POS,
            ItemData.ItemType.Burger => BURGER_COUNTER_POS,
            _ => Vector2.Zero,
        };
    }
}