using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibVLCSharp.Shared;

namespace Flow.Player.Services.MediaPlayerService;

public interface IMediaPlayerService
{
	public long Time { get; set; }
	public int Volume { get; set; }
	public List<AudioOutputDevice> AudioOutputDevices { get; }
	public string? CurrentOutputDeviceId { get; }
	public Task LoadFile(string filePath);
	public void Play();
	public void Pause();
	public void Stop();
	public void SetMute(bool mute);
	public long GetDuration();
	public void SetTimeCallback(EventHandler<MediaPlayerTimeChangedEventArgs> callback);
	public void SetOutputDevice(string id);
}

/// <summary>
/// Describes individual audio output device 
/// </summary>
public readonly struct AudioOutputDevice(string name, string id, string? group = null)
{
	public string Name { get; init; } = name;
	public string Id { get; init; } = id;
	public string? Group { get; init; } = group;
}