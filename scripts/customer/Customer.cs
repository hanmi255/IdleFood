using Godot;

/// <summary>
/// 顾客类，管理顾客行为、状态和订单交互
/// </summary>
public partial class Customer : Node2D
{
    private Sprite2D _face;                            /// 顾客面部精灵，用于显示表情动画
    private Sprite2D _body;                            /// 顾客身体精灵，用于显示身体动画
    private Sprite2D _handLeft;                        /// 左手精灵，用于显示手势动画
    private Sprite2D _handRight;                       /// 右手精灵，用于显示手势动画
    private AnimationPlayer _animPlayer;               /// 动画播放器，控制顾客动画状态
    private Control _itemBox;                          /// 物品显示容器，用于展示订单UI
    private TextureRect _itemIcon;                     /// 物品图标显示组件
    private Label _itemLabel;                          /// 物品数量显示标签
    private int _requestQuantity;                      /// 订单请求数量，初始化时设置

    public bool _waitingOrder = false;                 /// 是否正在等待订单完成
    public bool isBeingServed = false;                 /// 是否正在被服务
    public int currentOrderStatus;                     /// 当前订单状态（剩余数量）
    public ItemData RequestItem { get; private set; }  /// 当前请求的物品数据
    public Vector2 counterPos;                         /// 柜台位置坐标，用于移动目标

    /// <summary>
    /// 节点初始化，获取所有子节点引用
    /// </summary>
    public override void _Ready()
    {
        _body = GetNode<Sprite2D>("%Body");
        _face = GetNode<Sprite2D>("%Face");
        _handLeft = GetNode<Sprite2D>("%HandLeft");
        _handRight = GetNode<Sprite2D>("%HandRight");
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _itemBox = GetNode<Control>("ItemBox");
        _itemIcon = GetNode<TextureRect>("%ItemIcon");
        _itemLabel = GetNode<Label>("%ItemLabel");
    }

    /// <summary>
    /// 每帧更新，同步订单状态显示
    /// </summary>
    public override void _Process(double delta)
    {
        _itemLabel.Text = currentOrderStatus.ToString();
    }

    /// <summary>
    /// 设置顾客外观精灵
    /// </summary>
    /// <param name="data">包含精灵资源的顾客数据</param>
    public void SetSprites(CustomerData data)
    {
        _body.Texture = data.Body;
        _face.Texture = data.Face;
        _handLeft.Texture = data.Hand;
        _handRight.Texture = data.Hand;
    }

    /// <summary>
    /// 接收订单并减少剩余数量
    /// </summary>
    public void ReceiveOrder()
    {
        currentOrderStatus -= 1;
        if (currentOrderStatus <= 0)
            OrderCompleted();
    }

    /// <summary>
    /// 订单完成处理，触发动画和信号
    /// </summary>
    private void OrderCompleted()
    {
        _itemBox.Hide();
        _waitingOrder = false;
        var counterTopPos = counterPos.Y - 130;

        var tween = CreateTween();
        tween.TweenProperty(this, "position", new Vector2(counterPos.X, counterTopPos), 1.0);
        tween.TweenInterval(0.2);
        tween.TweenProperty(this, "position", new Vector2(counterPos.X + 800, counterTopPos), 3.0);
        tween.TweenInterval(0.2);
        tween.Finished += () =>
        {
            GameManager.Instance.EmitSignal("OnCustomerOrderCompleted", this);
        };
    }

    /// <summary>
    /// 初始化订单物品和数量
    /// </summary>
    /// <param name="item">请求的物品数据</param>
    /// <param name="quantity">需要的数量</param>
    public void InitItem(ItemData item, int quantity)
    {
        RequestItem = item;
        _requestQuantity = quantity;
        currentOrderStatus = quantity;
    }

    /// <summary>
    /// 移动到柜台位置并播放动画
    /// </summary>
    public void MoveToCounter()
    {
        PlayMoveAnim();

        var tween = CreateTween();
        tween.TweenProperty(this, "position", new Vector2(counterPos.X, Position.Y), 1.0);
        tween.TweenInterval(0.2);
        tween.TweenProperty(this, "position", counterPos, 1.0);
        tween.TweenInterval(0.5);
        tween.Finished += () =>
        {
            _animPlayer.Play("idle");
            _waitingOrder = true;
            GameManager.Instance.EmitSignal("OnCustomerRequested", this);
        };
    }

    /// <summary>
    /// 播放移动动画
    /// </summary>
    public void PlayMoveAnim()
    {
        _animPlayer.Play("move");
    }

    /// <summary>
    /// 显示订单UI界面
    /// </summary>
    public void ShowOrderUI()
    {
        _itemBox.Show();
        _itemIcon.Texture = RequestItem.sprite;
        _itemLabel.Text = _requestQuantity.ToString();
    }
}

