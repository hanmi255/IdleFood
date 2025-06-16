using Godot;

public partial class Customer : Node2D
{
    private GameManager _gameManager;
    private Sprite2D _body;
    private Sprite2D _face;
    private Sprite2D _handLeft;
    private Sprite2D _handRight;
    private AnimationPlayer _animPlayer;
    private Control _itemBox;
    private TextureRect _itemIcon;
    private Label _itemLabel;
    private ItemData _requestItem;

    private int _requestQuantity;
    private int _currentOrderStatus;
    private bool _waitingOrder = false;

    public bool beingServed = false;
    public Vector2 counterPos;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>("/root/GameManager");
        _body = GetNode<Sprite2D>("%Body");
        _face = GetNode<Sprite2D>("%Face");
        _handLeft = GetNode<Sprite2D>("%HandLeft");
        _handRight = GetNode<Sprite2D>("%HandRight");
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _itemBox = GetNode<Control>("ItemBox");
        _itemIcon = GetNode<TextureRect>("%ItemIcon");
        _itemLabel = GetNode<Label>("%ItemLabel");
    }

    public void SetSprites(CustomerData data)
    {
        _body.Texture = data.Body;
        _face.Texture = data.Face;
        _handLeft.Texture = data.Hand;
        _handRight.Texture = data.Hand;
    }

    public void InitItem(ItemData item, int quantity)
    {
        _requestItem = item;
        _requestQuantity = quantity;
        _currentOrderStatus = quantity;
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
            _gameManager.EmitSignal("OnCustomerRequested", this);
        };
    }

    public void PlayMoveAnim()
    {
        _animPlayer.Play("move");
    }

    public void ShowOrderUI()
    {
        _itemBox.Show();
        _itemIcon.Texture = _requestItem.sprite;
        _itemLabel.Text = _requestQuantity.ToString();
    }
}
