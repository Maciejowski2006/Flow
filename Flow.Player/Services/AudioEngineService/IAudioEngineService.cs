using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Flow.Player.Services.AudioEngineService;

public interface IAudioEngineService
{
	public float Time { get; }
	public float Volume { get; set; }
	public float Duration { get; }
	public List<AudioOutputDevice> AudioOutputDevices { get; }
	public AudioOutputDevice CurrentOutputDevice { get; }
	public Task LoadFile(string filePath);
	public void Play();
	public void Pause();
	public void Stop();
	public void SetMute(bool mute);
	public void Seek(TimeSpan time, SeekOrigin origin);
	public void SetOutputDevice(nint id);
	public event EventHandler? PlaybackEnded;
}

/// <summary>
/// Describes individual audio output device 
/// </summary>
public readonly struct AudioOutputDevice(string name, nint id, string? group = null)
{
	public string Name { get; init; } = name;
	public nint Id { get; init; } = id;
	public string? Group { get; init; } = group;
}