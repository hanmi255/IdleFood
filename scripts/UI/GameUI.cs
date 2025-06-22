using Godot;
public partial class GameUI : CanvasLayer
{
    [Export] private float _cashierCost_1 = 50.0f;    /// 收银员等级1的成本价格
    [Export] private float _cashierCost_2 = 100.0f;   /// 收银员等级2的成本价格
    [Export] private float _cashierCost_3 = 150.0f;   /// 收银员等级3的成本价格
    [Export] private float _fasterCoffeeCost = 4000.0f;  /// 咖啡制作加速升级成本
    [Export] private float _fasterBurgerCost = 6000.0f;  /// 汉堡制作加速升级成本

    private UpgradePanel _upgradeCoffeePanel;    /// 咖啡升级面板
    private TextureButton _upgradeCoffeeButton;  /// 咖啡升级按钮
    private UpgradePanel _upgradeBurgerPanel;    /// 汉堡升级面板
    private TextureButton _upgradeBurgerButton;  /// 汉堡升级按钮
    private Label _currentCoins;                 /// 当前金币显示标签
    private Control _shopPanel;                  /// 商店面板
    private Button _shopButton;                  /// 商店开关按钮
    private Panel _newCashierCard_1;             /// 收银员等级1卡片
    private Button _newCashierButton_1;          /// 收银员等级1购买按钮
    private Panel _newCashierCard_2;             /// 收银员等级2卡片
    private Button _newCashierButton_2;          /// 收银员等级2购买按钮
    private Panel _newCashierCard_3;             /// 收银员等级3卡片
    private Button _newCashierButton_3;          /// 收银员等级3购买按钮
    private Panel _fasterCoffeeCard;             /// 咖啡加速升级卡片
    private Button _fasterCoffeeButton;          /// 咖啡加速升级按钮
    private Panel _fasterBurgerCard;             /// 汉堡加速升级卡片
    private Button _fasterBurgerButton;          /// 汉堡加速升级按钮
    private Control _optionsPanel;               /// 设置面板
    private Button _optionsButton;               /// 设置按钮
    private HSlider _musicHSlider;               /// 音乐调节滑块
    private HSlider _sfxHSlider;                 /// 音效调节滑块
    private int _musicIndex;                     /// 音乐索引
    private int _sfxIndex;                       /// 音效索引
    private ItemData _itemCoffee;                /// 咖啡物品数据引用
    private ItemData _itemBurger;                /// 汉堡物品数据引用

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

        _optionsPanel = GetNode<Control>("OptionsPanel");
        _optionsButton = GetNode<Button>("%OptionsButton");
        _musicHSlider = GetNode<HSlider>("%MusicHSlider");
        _sfxHSlider = GetNode<HSlider>("%SFXHSlider");
        _musicIndex = AudioServer.GetBusIndex("Music");
        _sfxIndex = AudioServer.GetBusIndex("SFX");

        _upgradeCoffeePanel.InitUpgradePanel(_itemCoffee);
        _upgradeBurgerPanel.InitUpgradePanel(_itemBurger);

        _upgradeCoffeeButton.Pressed += () => OnTogglePanelVisibility(_upgradeCoffeePanel, _upgradeBurgerPanel, _shopPanel, _optionsPanel);
        _upgradeBurgerButton.Pressed += () => OnTogglePanelVisibility(_upgradeBurgerPanel, _upgradeCoffeePanel, _shopPanel, _optionsPanel);
        _shopButton.Pressed += () => OnTogglePanelVisibility(_shopPanel, _upgradeCoffeePanel, _upgradeBurgerPanel, _optionsPanel);
        _optionsButton.Pressed += () => OnTogglePanelVisibility(_optionsPanel, _upgradeCoffeePanel, _upgradeBurgerPanel, _shopPanel);

        _newCashierButton_1.Pressed += () => OnNewCashierButtonPressed(_cashierCost_1, _newCashierCard_1);
        _newCashierButton_2.Pressed += () => OnNewCashierButtonPressed(_cashierCost_2, _newCashierCard_2);
        _newCashierButton_3.Pressed += () => OnNewCashierButtonPressed(_cashierCost_3, _newCashierCard_3);

        _fasterCoffeeButton.Pressed += () => OnFasterItemButtonPressed(_fasterCoffeeCost, _itemCoffee, _fasterCoffeeCard);
        _fasterBurgerButton.Pressed += () => OnFasterItemButtonPressed(_fasterBurgerCost, _itemBurger, _fasterBurgerCard);

        _musicHSlider.ValueChanged += value => OnHSliderValueChanged(_musicIndex, (float)value);
        _sfxHSlider.ValueChanged += value => OnHSliderValueChanged(_sfxIndex, (float)value);

        _newCashierButton_1.Text = GameManager.FormatCoins(_cashierCost_1);
        _newCashierButton_2.Text = GameManager.FormatCoins(_cashierCost_2);
        _newCashierButton_3.Text = GameManager.FormatCoins(_cashierCost_3);
        _fasterCoffeeButton.Text = GameManager.FormatCoins(_fasterCoffeeCost);
        _fasterBurgerButton.Text = GameManager.FormatCoins(_fasterBurgerCost);
    }

    /// <summary>
    /// 每帧更新，同步金币显示
    /// </summary>
    public override void _Process(double delta)
    {
        _currentCoins.Text = GameManager.FormatCoins(GameManager.Instance.currentCoins);
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

    /// <summary>
    /// 音效和音乐音量滑动条值改变时改变音量
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    private static void OnHSliderValueChanged(int index, float value)
    {
        AudioServer.SetBusVolumeDb(index, Mathf.LinearToDb(value));
    }
}
