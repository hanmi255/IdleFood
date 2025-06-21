using Godot;

public partial class Customer : Node2D
{
    private Sprite2D _body;
    private Sprite2D _face;
    private Sprite2D _handLeft;
    private Sprite2D _handRight;
    private AnimationPlayer _animPlayer;
    private Control _itemBox;
    private TextureRect _itemIcon;
    private Label _itemLabel;

    private int _requestQuantity;

    public bool _waitingOrder = false;
    public bool beingServed = false;
    public int currentOrderStatus;
    public ItemData RequestItem { get; private set; }
    public Vector2 counterPos;

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

    public override void _Process(double delta)
    {
        _itemLabel.Text = currentOrderStatus.ToString();
    }

    public void SetSprites(CustomerData data)
    {
        _body.Texture = data.Body;
        _face.Texture = data.Face;
        _handLeft.Texture = data.Hand;
        _handRight.Texture = data.Hand;
    }

    public void ReceiveOrder()
    {
        currentOrderStatus -= 1;
        if (currentOrderStatus <= 0)
            OrderCompleted();
    }

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

    public void InitItem(ItemData item, int quantity)
    {
        RequestItem = item;
        _requestQuantity = quantity;
        currentOrderStatus = quantity;
    }

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

    public void PlayMoveAnim()
    {
        _animPlayer.Play("move");
    }

    public void ShowOrderUI()
    {
        _itemBox.Show();
        _itemIcon.Texture = RequestItem.sprite;
        _itemLabel.Text = _requestQuantity.ToString();
    }
}
