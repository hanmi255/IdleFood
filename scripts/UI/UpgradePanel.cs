using Godot;

public partial class UpgradePanel : Control
{
    private Label _level;
    private Label _itemName;
    private HBoxContainer _starHBox;
    private ProgressBar _progressBar;
    private Label _profit;
    private Label _cookTime;
    private Button _upgradeButton;
    private ItemData _itemRef;

    private int _currentLevel;
    private int _currentStars = -1;

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

    public override void _Process(double delta)
    {
        _progressBar.Value = _currentLevel / 25.0;
    }

    public void InitUpgradePanel(ItemData item)
    {
        _itemRef = item;
        _itemName.Text = item.id;
        _progressBar.Value = 0;
        UpdateStars();
        _itemRef.OnStarReached += OnStarReached;
    }

    private void UpdateStars()
    {
        _level.Text = $"Lv.{_itemRef.currentLevel}";
        _profit.Text = _itemRef.profit.ToString();
        _cookTime.Text = _itemRef.cookTime.ToString();
        _upgradeButton.Text = _itemRef.upgradeCost.ToString();
    }

    private void OnStarReached()
    {
        _currentLevel = 0;
        _currentStars += 1;

        var star = _starHBox.GetChild<TextureRect>(_currentStars);
        star.Modulate = new Color(255, 255, 255);
    }

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
