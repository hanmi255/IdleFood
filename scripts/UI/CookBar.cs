using System;
using Godot;

public partial class CookBar : Control
{
    [Signal] public delegate void OnCookFinishedEventHandler();

    private TextureProgressBar _cookProgressBar;

    public override void _Ready()
    {
        _cookProgressBar = GetNode<TextureProgressBar>("CookProgressBar");
    }

    public void CookItem(float cookTime)
    {
        var tween = CreateTween();
        tween.TweenProperty(_cookProgressBar, "value", 1.0f, cookTime);
        tween.Finished += () => { EmitSignal("OnCookFinished"); };
    }

    public void ResetBar()
    {
        _cookProgressBar.Value = 0;
    }
}
