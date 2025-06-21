using System.Linq;
using Godot;
using Godot.Collections;

public partial class SoundManager : Node
{
    public static SoundManager Instance { get; private set; }

    private static readonly AudioStream COINS = ResourceLoader.Load<AudioStream>("res://assets/sound/coins.wav");
    private static readonly AudioStream NORMAL_SFX = ResourceLoader.Load<AudioStream>("res://assets/sound/normal-sfx.mp3");

    [Export] private Array<AudioStreamPlayer> _streamPlayers;

    public override void _Ready()
    {
        if (Instance == null)
            Instance = this;
        else
            QueueFree();
    }

    private void PlayAudio(AudioStream clip, float volume)
    {
        if (GetFreeAudioPlayer() is { } player)
        {
            player.Stream = clip;
            player.VolumeDb = volume;
            player.Play();
        }
    }

    public void PlayCoins()
    {
        PlayAudio(COINS, 15);
    }

    public void PlayUI()
    {
        PlayAudio(NORMAL_SFX, 0.5f);
    }

    private AudioStreamPlayer GetFreeAudioPlayer()
    {
        return _streamPlayers.FirstOrDefault(audio => !audio.Playing);
    }
}