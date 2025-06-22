using System.Linq;
using Godot;
using Godot.Collections;

/// <summary>
/// 音效管理类，负责全局音效播放和音频资源管理
/// </summary>
public partial class SoundManager : Node
{
    /// <summary>
    /// 音频播放器数组，用于管理多个音频播放通道
    /// </summary>
    [Export] private Array<AudioStreamPlayer> _streamPlayers;

    /// <summary>
    /// 音效管理器单例实例
    /// </summary>
    public static SoundManager Instance { get; private set; }

    /// <summary>
    /// 金币收集音效资源
    /// </summary>
    private static readonly AudioStream COINS = ResourceLoader.Load<AudioStream>("res://assets/sound/coins.wav");
    /// <summary>
    /// 普通音效资源（用于UI等）
    /// </summary>
    private static readonly AudioStream NORMAL_SFX = ResourceLoader.Load<AudioStream>("res://assets/sound/normal-sfx.mp3");

    /// <summary>
    /// 节点初始化，创建单例并初始化音频资源
    /// </summary>
    public override void _Ready()
    {
        if (Instance == null)
            Instance = this;
        else
            QueueFree();
    }

    /// <summary>
    /// 播放指定音频
    /// </summary>
    /// <param name="clip">要播放的音频资源</param>
    /// <param name="volume">播放音量（0-100）</param>
    private void PlayAudio(AudioStream clip, float volume)
    {
        if (GetFreeAudioPlayer() is { } player)
        {
            player.Stream = clip;
            player.VolumeDb = volume;
            player.Play();
        }
    }

    /// <summary>
    /// 播放金币收集音效
    /// </summary>
    public void PlayCoins()
    {
        PlayAudio(COINS, 15);
    }

    /// <summary>
    /// 播放UI操作音效
    /// </summary>
    public void PlayUI()
    {
        PlayAudio(NORMAL_SFX, 0.5f);
    }

    /// <summary>
    /// 获取空闲的音频播放器
    /// </summary>
    /// <returns>空闲的音频播放器实例，若无可返回null</returns>
    private AudioStreamPlayer GetFreeAudioPlayer()
    {
        return _streamPlayers.FirstOrDefault(audio => !audio.Playing);
    }
}