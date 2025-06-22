using Godot;

/// <summary>
/// 顾客数据类，存储顾客外观相关的纹理资源
/// </summary>
[GlobalClass]
public partial class CustomerData : Resource
{
    /// <summary>
    /// 顾客身体纹理资源，用于显示身体外观
    /// </summary>
    [Export] public Texture2D Body { get; set; }

    /// <summary>
    /// 顾客面部纹理资源，用于显示表情和面部特征
    /// </summary>
    [Export] public Texture2D Face { get; set; }

    /// <summary>
    /// 顾客手部纹理资源，用于显示手势动画
    /// </summary>
    [Export] public Texture2D Hand { get; set; }
}