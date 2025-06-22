using Godot;

/// <summary>
/// 升级面板类，负责显示和管理物品升级相关信息
/// </summary>
public partial class UpgradePanel : Control
{
    private Label _level;              /// 等级显示标签
    private Label _itemName;           /// 物品名称显示标签
    private HBoxContainer _starHBox;   /// 星级图标容器，用于显示升级星级
    private ProgressBar _progressBar;  /// 升级进度条，显示升级进度百分比
    private Label _profit;             /// 利润显示标签
    private Label _cookTime;           /// 烹饪时间显示标签
    private Button _upgradeButton;     /// 升级按钮
    private ItemData _itemRef;         /// 关联的物品数据引用
    private int _currentLevel;         /// 当前升级等级（0-25）
    private int _currentStars = -1;    /// 当前星级数量，初始为-1

    /// <summary>
    /// 节点初始化，获取子节点引用并绑定事件
    /// </summary>
    public override void _Ready()
    {
        _level = GetNode<Label>("%Level");
        _itemName = GetNode<Label>("%ItemName");
        _starHBox = GetNode<HBoxContainer>("%StarHBox");
        _progressBar = GetNode<ProgressBar>("%ProgressBar");
        _profit = GetNode<Label>("%Profit");
        _cookTime = GetNode<Label>("%CookTime");
        _upgradeButton = GetNode<Button>("%UpgradeButton");
        _upgradeButton.Pressed += OnUpgradeButtonPressed;
    }

    /// <summary>
    /// 每帧更新，同步升级进度显示
    /// </summary>
    public override void _Process(double delta)
    {
        _progressBar.Value = _currentLevel / 25.0;
    }

    /// <summary>
    /// 初始化升级面板
    /// </summary>
    /// <param name="item">要关联的物品数据</param>
    public void InitUpgradePanel(ItemData item)
    {
        _itemRef = item;
        _itemName.Text = item.id;
        _progressBar.Value = 0;
        UpdateStars();
        _itemRef.OnStarReached += OnStarReached;
    }

    /// <summary>
    /// 更新所有UI元素显示当前状态
    /// </summary>
    private void UpdateStars()
    {
        _level.Text = $"Lv.{_itemRef.currentLevel}";
        _profit.Text = GameManager.FormatCoins(_itemRef.profit);
        _cookTime.Text = GameManager.FormatCoins(_itemRef.cookTime);
        _upgradeButton.Text = GameManager.FormatCoins(_itemRef.upgradeCost);
    }

    /// <summary>
    /// 处理星级提升事件
    /// </summary>
    private void OnStarReached()
    {
        _currentLevel = 0;
        _currentStars += 1;

        var star = _starHBox.GetChild<TextureRect>(_currentStars);
        star.Modulate = new Color(255, 255, 255);
    }

    /// <summary>
    /// 处理升级按钮点击事件
    /// </summary>
    private void OnUpgradeButtonPressed()
    {
        SoundManager.Instance.PlayUI();
        if (GameManager.Instance.currentCoins < _itemRef.upgradeCost)
            return;

        GameManager.Instance.currentCoins -= _itemRef.upgradeCost;
        _currentLevel += 1;
        _itemRef.UpdateItem();
        UpdateStars();
    }
}
