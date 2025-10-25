using System;
using System.Threading.Tasks;
using LibVLCSharp.Shared;

namespace Flow.Player;

public interface IMediaPlayerService
{
	public long Time { get; set; }
	public int Volume { get; set; }
	public Task LoadFile(string filePath);
	public void Play();
	public void Pause();
	public void Stop();
	public void SetMute(bool mute);
	public long GetDuration();
	public void SetTimeCallback(EventHandler<MediaPlayerTimeChangedEventArgs> callback);
}