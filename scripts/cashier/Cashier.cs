using System.Threading.Tasks;
using Godot;

/// <summary>
/// 收银员类，负责处理顾客订单、移动、烹饪和订单交付
/// </summary>
public partial class Cashier : Node2D
{
    /// <summary>
    /// 订单完成事件，在订单成功交付后触发
    /// </summary>
    /// <param name="cashier">完成订单的收银员实例</param>
    [Signal] public delegate void OnOrderCompletedEventHandler(Cashier cashier);

    [Export] private float _moveSpeed = 50.0f;             /// 移动速度属性，控制收银员移动速度

    private AnimationPlayer _animationPlayer;              /// 动画播放器，用于控制收银员动画播放
    private CookBar _cookBar;                              /// 烹饪进度条，用于显示烹饪完成度
    private Vector2 _counterPos;                           /// 柜台坐标位置，用于收银员移动目标点
    private ItemData _itemRequest;                         /// 当前订单请求的商品数据
    private Vector2 _itemCounterPos;                       /// 商品柜台坐标位置，用于收银员取货目标点
    public Customer CurrentCustomer { get; private set; }  /// 当前正在服务的顾客实例

    /// <summary>
    /// 初始化节点，获取动画播放器和烹饪进度条节点
    /// </summary>
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

    /// <summary>
    /// 设置当前服务的顾客
    /// </summary>
    /// <param name="customer">需要服务的顾客实例</param>
    public void SetCustomer(Customer customer)
    {
        CurrentCustomer = customer;
        customer.isBeingServed = true;
        _counterPos = customer.Position with { Y = customer.Position.Y + 160 };
        _itemRequest = customer.RequestItem;
        _itemCounterPos = GameManager.Instance.GetItemCounterPos(_itemRequest);
    }

    /// <summary>
    /// 执行接单流程，移动到柜台并展示订单UI
    /// </summary>
    /// <returns>异步任务</returns>
    public async Task TakeOrder()
    {
        MoveToCounter();
        await Task.Delay(1100);
        CurrentCustomer.ShowOrderUI();
        MoveToItemPos();
    }

    /// <summary>
    /// 移动到柜台位置
    /// </summary>
    private void MoveToCounter()
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "position", _counterPos, 1.0f);
        _animationPlayer.Play("move");
    }

    /// <summary>
    /// 移动到商品位置并开始烹饪
    /// </summary>
    private void MoveToItemPos()
    {
        var tween = CreateTween();
        tween.TweenInterval(0.5);
        tween.TweenProperty(this, "position", _itemCounterPos, 1.0f);
        _animationPlayer.Play("move");
        tween.TweenInterval(0.05);
        tween.Finished += StartCookTime;
    }

    /// <summary>
    /// 交付订单并处理后续逻辑
    /// </summary>
    /// <returns>异步任务</returns>
    private async Task DeliverOrder()
    {
        MoveToCounter();
        await Task.Delay(1100);
        CurrentCustomer.ReceiveOrder();
        GameManager.Instance.currentCoins += _itemRequest.profit;
        GameManager.Instance.PlayCoinVFX(GlobalPosition);

        if (CurrentCustomer.currentOrderStatus > 0)
            MoveToItemPos();
        else
        {
            _animationPlayer.Play("idle");
            CurrentCustomer = null;
            EmitSignal("OnOrderCompleted", this);
        }
    }

    /// <summary>
    /// 开始烹饪倒计时
    /// </summary>
    private void StartCookTime()
    {
        _animationPlayer.Play("idle");
        _cookBar.Show();
        _cookBar.CookItem(_itemRequest.cookTime);
    }
}