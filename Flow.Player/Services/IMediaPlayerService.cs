using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibVLCSharp.Shared;

namespace Flow.Player;

public interface IMediaPlayerService
{
	public long Time { get; set; }
	public int Volume { get; set; }
	public List<AudioOutputGroup> AudioOutputGroups { get; }
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
/// Describes audio output groups (Like DirectSound, WASAPI, CoreAudio, PulseAudio)
/// </summary>
public readonly struct AudioOutputGroup(string id, string name, List<AudioOutputDevice> devices)
{
	public string Id { get; init; } = id;
	public string Name { get; init; } = name;
	public List<AudioOutputDevice> Devices { get; init; } = devices;
}
/// <summary>
/// Describes individual audio output device 
/// </summary>
public readonly struct AudioOutputDevice(string name, string id)
{
	public string Name { get; init; } = name;
	public string Id { get; init; } = id;
}