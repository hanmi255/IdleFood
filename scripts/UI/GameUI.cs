using Godot;
public partial class GameUI : CanvasLayer
{
    /// <summary>
    /// 收银员等级1的成本价格
    /// </summary>
    [Export] private float _cashierCost_1 = 50.0f;
    /// <summary>
    /// 收银员等级2的成本价格
    /// </summary>
    [Export] private float _cashierCost_2 = 100.0f;
    /// <summary>
    /// 收银员等级3的成本价格
    /// </summary>
    [Export] private float _cashierCost_3 = 150.0f;
    /// <summary>
    /// 咖啡制作加速升级成本
    /// </summary>
    [Export] private float _fasterCoffeeCost = 4000.0f;
    /// <summary>
    /// 汉堡制作加速升级成本
    /// </summary>
    [Export] private float _fasterBurgerCost = 6000.0f;

    /// <summary>
    /// 咖啡升级面板实例
    /// </summary>
    private UpgradePanel _upgradeCoffeePanel;
    /// <summary>
    /// 咖啡升级按钮实例
    /// </summary>
    private TextureButton _upgradeCoffeeButton;
    /// <summary>
    /// 汉堡升级面板实例
    /// </summary>
    private UpgradePanel _upgradeBurgerPanel;
    /// <summary>
    /// 汉堡升级按钮实例
    /// </summary>
    private TextureButton _upgradeBurgerButton;
    /// <summary>
    /// 当前金币显示标签
    /// </summary>
    private Label _currentCoins;
    /// <summary>
    /// 商店面板实例
    /// </summary>
    private Control _shopPanel;
    /// <summary>
    /// 商店开关按钮实例
    /// </summary>
    private Button _shopButton;
    /// <summary>
    /// 收银员等级1卡片
    /// </summary>
    private Panel _newCashierCard_1;
    /// <summary>
    /// 收银员等级1购买按钮
    /// </summary>
    private Button _newCashierButton_1;
    /// <summary>
    /// 收银员等级2卡片
    /// </summary>
    private Panel _newCashierCard_2;
    /// <summary>
    /// 收银员等级2购买按钮
    /// </summary>
    private Button _newCashierButton_2;
    /// <summary>
    /// 收银员等级3卡片
    /// </summary>
    private Panel _newCashierCard_3;
    /// <summary>
    /// 收银员等级3购买按钮
    /// </summary>
    private Button _newCashierButton_3;
    /// <summary>
    /// 咖啡加速升级卡片
    /// </summary>
    private Panel _fasterCoffeeCard;
    /// <summary>
    /// 咖啡加速升级按钮
    /// </summary>
    private Button _fasterCoffeeButton;
    /// <summary>
    /// 汉堡加速升级卡片
    /// </summary>
    private Panel _fasterBurgerCard;
    /// <summary>
    /// 汉堡加速升级按钮
    /// </summary>
    private Button _fasterBurgerButton;

    /// <summary>
    /// 咖啡物品数据引用
    /// </summary>
    private ItemData _itemCoffee;
    /// <summary>
    /// 汉堡物品数据引用
    /// </summary>
    private ItemData _itemBurger;

    /// <summary>
    /// 节点初始化，绑定子节点和事件
    /// </summary>
    public override void _Ready()
    {
        _itemCoffee = GameManager.Instance._itemCoffee;
        _itemBurger = GameManager.Instance._itemBurger;

        _upgradeCoffeePanel = GetNode<UpgradePanel>("UpgradeCoffeePanel");
        _upgradeCoffeeButton = GetNode<TextureButton>("UpgradeCoffeeButton");

        _upgradeBurgerPanel = GetNode<UpgradePanel>("UpgradeBurgerPanel");
        _upgradeBurgerButton = GetNode<TextureButton>("UpgradeBurgerButton");

        _currentCoins = GetNode<Label>("%CurrentCoins");

        _shopPanel = GetNode<Control>("ShopPanel");
        _shopButton = GetNode<Button>("%ShopButton");

        _newCashierCard_1 = GetNode<Panel>("%NewCashierCard_1");
        _newCashierButton_1 = GetNode<Button>("%NewCashierButton_1");

        _fasterCoffeeCard = GetNode<Panel>("%FasterCoffeeCard");
        _fasterCoffeeButton = GetNode<Button>("%FasterCoffeeButton");

        _newCashierCard_2 = GetNode<Panel>("%NewCashierCard_2");
        _newCashierButton_2 = GetNode<Button>("%NewCashierButton_2");

        _fasterBurgerCard = GetNode<Panel>("%FasterBurgerCard");
        _fasterBurgerButton = GetNode<Button>("%FasterBurgerButton");

        _newCashierCard_3 = GetNode<Panel>("%NewCashierCard_3");
        _newCashierButton_3 = GetNode<Button>("%NewCashierButton_3");

        _upgradeCoffeePanel.InitUpgradePanel(_itemCoffee);
        _upgradeBurgerPanel.InitUpgradePanel(_itemBurger);

        _upgradeCoffeeButton.Pressed += () => OnTogglePanelVisibility(_upgradeCoffeePanel, _upgradeBurgerPanel, _shopPanel);
        _upgradeBurgerButton.Pressed += () => OnTogglePanelVisibility(_upgradeBurgerPanel, _upgradeCoffeePanel, _shopPanel);
        _shopButton.Pressed += () => OnTogglePanelVisibility(_shopPanel, _upgradeCoffeePanel, _upgradeBurgerPanel);

        _newCashierButton_1.Pressed += () => OnNewCashierButtonPressed(_cashierCost_1, _newCashierCard_1);
        _newCashierButton_2.Pressed += () => OnNewCashierButtonPressed(_cashierCost_2, _newCashierCard_2);
        _newCashierButton_3.Pressed += () => OnNewCashierButtonPressed(_cashierCost_3, _newCashierCard_3);

        _fasterCoffeeButton.Pressed += () => OnFasterItemButtonPressed(_fasterCoffeeCost, _itemCoffee, _fasterCoffeeCard);
        _fasterBurgerButton.Pressed += () => OnFasterItemButtonPressed(_fasterBurgerCost, _itemBurger, _fasterBurgerCard);

        _newCashierButton_1.Text = _cashierCost_1.ToString();
        _newCashierButton_2.Text = _cashierCost_2.ToString();
        _newCashierButton_3.Text = _cashierCost_3.ToString();
        _fasterCoffeeButton.Text = _fasterCoffeeCost.ToString();
        _fasterBurgerButton.Text = _fasterBurgerCost.ToString();
    }

    /// <summary>
    /// 每帧更新，同步金币显示
    /// </summary>
    public override void _Process(double delta)
    {
        _currentCoins.Text = GameManager.Instance.currentCoins.ToString();
    }

    /// <summary>
    /// 切换面板可见性
    /// </summary>
    /// <param name="showPanel">要显示的面板</param>
    /// <param name="hidePanels">需要隐藏的其他面板</param>
    private static void OnTogglePanelVisibility(Control showPanel, params Control[] hidePanels)
    {
        SoundManager.Instance.PlayUI();
        foreach (var panel in hidePanels)
        {
            panel.Hide();
        }
        showPanel.Visible = !showPanel.Visible;
    }

    /// <summary>
    /// 处理收银员购买按钮点击
    /// </summary>
    /// <param name="cashierCost">购买所需费用</param>
    /// <param name="panel">关联的UI面板</param>
    private static void OnNewCashierButtonPressed(float cashierCost, Panel panel)
    {
        SoundManager.Instance.PlayUI();
        if (GameManager.Instance.currentCoins >= cashierCost)
        {
            GameManager.Instance.currentCoins -= cashierCost;
            GameManager.Instance.EmitSignal("OnSpawnNewCashier");
            panel.Hide();
        }
    }

    /// <summary>
    /// 处理物品加速升级按钮点击
    /// </summary>
    /// <param name="fasterItemCost">升级所需费用</param>
    /// <param name="item">要加速的物品数据</param>
    /// <param name="panel">关联的UI面板</param>
    private static void OnFasterItemButtonPressed(float fasterItemCost, ItemData item, Panel panel)
    {
        SoundManager.Instance.PlayUI();
        if (GameManager.Instance.currentCoins >= fasterItemCost)
        {
            GameManager.Instance.currentCoins -= fasterItemCost;
            item.cookTime = 2.0f;
            panel.Hide();
        }
    }
}
