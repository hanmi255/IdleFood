using Godot;
using Godot.Collections;

public partial class GameManager : Node
{
    [Signal] public delegate void OnCustomerRequestedEventHandler(Customer customer);

    private static readonly ItemData ITEM_BURGER = GD.Load<ItemData>("res://data/item/item_burger.tres");
    private static readonly ItemData ITEM_COFFEE = GD.Load<ItemData>("res://data/item/item_coffee.tres");

    public static ItemData GetRandomItem()
    {
        Array items = [ITEM_BURGER, ITEM_COFFEE];
        return (ItemData)items.PickRandom();
    }
}