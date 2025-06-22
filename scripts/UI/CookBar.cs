using Godot;

/// <summary>
/// 烹饪进度条类，负责控制烹饪动画和进度显示
/// </summary>
public partial class CookBar : Control
{
    /// <summary>
    /// 烹饪完成事件，在进度条完成时触发
    /// </summary>
    [Signal] public delegate void OnCookFinishedEventHandler();

    private TextureProgressBar _cookProgressBar;  /// 进度条控件引用，用于显示烹饪进度

    /// <summary>
    /// 节点初始化，获取进度条子节点
    /// </summary>
    public override void _Ready()
    {
        _cookProgressBar = GetNode<TextureProgressBar>("CookProgressBar");
    }

    /// <summary>
    /// 开始烹饪动画
    /// </summary>
    /// <param name="cookTime">烹饪所需时间（秒）</param>
    public void CookItem(float cookTime)
    {
        var tween = CreateTween();
        tween.TweenProperty(_cookProgressBar, "value", 1.0f, cookTime);
        tween.Finished += () => { EmitSignal("OnCookFinished"); };
    }

    /// <summary>
    /// 重置进度条到初始状态
    /// </summary>
    public void ResetBar()
    {
        _cookProgressBar.Value = 0;
    }
}
