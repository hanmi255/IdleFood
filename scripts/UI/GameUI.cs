using Godot;
public partial class GameUI : CanvasLayer
{
    private UpgradePanel _coffeePanel;
    private UpgradePanel _burgerPanel;
    private TextureButton _coffeeButton;
    private TextureButton _burgerButton;
    private Label _currentCoins;

    public override void _Ready()
    {
        _coffeePanel = GetNode<UpgradePanel>("CoffeePanel");
        _burgerPanel = GetNode<UpgradePanel>("BurgerPanel");
        _coffeeButton = GetNode<TextureButton>("CoffeeButton");
        _burgerButton = GetNode<TextureButton>("BurgerButton");
        _currentCoins = GetNode<Label>("%CurrentCoins");

        _coffeePanel.InitUpgradePanel(GameManager.ITEM_COFFEE);
        _burgerPanel.InitUpgradePanel(GameManager.ITEM_BURGER);

        _coffeeButton.Pressed += () => TogglePanel(_coffeePanel, _burgerPanel);
        _burgerButton.Pressed += () => TogglePanel(_burgerPanel, _coffeePanel);
    }

    public override void _Process(double delta)
    {
        _currentCoins.Text = GameManager.Instance.currentCoins.ToString();
    }

    private static void TogglePanel(UpgradePanel showPanel, UpgradePanel hidePanel)
    {
        SoundManager.Instance.PlayUI();
        hidePanel.Hide();
        showPanel.Visible = !showPanel.Visible;
    }
}
