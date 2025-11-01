using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using LibVLCSharp.Shared.Structures;

namespace Flow.Player.Services;



public class MediaPlayerService : IMediaPlayerService, IDisposable
{
	private readonly LibVLC _libVlc = new();
	private Media? _media;
	private MediaPlayer? _mediaPlayer;

	private int _unmutedVolume;

	public MediaPlayerService() { Core.Initialize(); }

	public bool IsPlaying => _mediaPlayer?.IsPlaying ?? false;
	public long Time
	{
		get => _mediaPlayer?.Time ?? 0;
		set
		{
			if (_mediaPlayer != null) _mediaPlayer.Time = value;
		}
	}
	public int Volume {
		get => _mediaPlayer?.Volume ?? 0;
		set
		{
			if (_mediaPlayer is null)
				return;

			_mediaPlayer.Volume = value;
		}
	}
	public List<AudioOutputGroup> AudioOutputGroups
	{
		get
		{
			List<AudioOutputGroup> outputGroups = [];
			AudioOutputDescription[] audioGroups = _libVlc.AudioOutputs;

			foreach (AudioOutputDescription group in audioGroups)
			{
				LibVLCSharp.Shared.Structures.AudioOutputDevice[] devices = _libVlc.AudioOutputDevices(group.Name);
				List<AudioOutputDevice> deviceIter = devices.Select(dev => new AudioOutputDevice(dev.Description, dev.DeviceIdentifier)).ToList();
				if (deviceIter.Count == 0)
					continue;
				outputGroups.Add(new(group.Name, group.Description, deviceIter));
			}
			
			return outputGroups;
		}
	}
	public string? CurrentOutputDeviceId => _mediaPlayer?.OutputDevice;
	public async Task LoadFile(string filePath)
	{
		// Dispose if there was a track playing before
		_mediaPlayer?.Stop();
		_mediaPlayer?.Dispose();

		_media = new(_libVlc, filePath);
		_mediaPlayer = new(_media);
		await _media.Parse();
	}

	public void Play() { _mediaPlayer?.Play(); }
	public void Pause() { _mediaPlayer?.Pause(); }
	public void Stop() { _mediaPlayer?.Stop(); }
	public void SetMute(bool mute)
	{
		if (_mediaPlayer is null)
			return;

		if (mute)
		{
			_unmutedVolume = _mediaPlayer.Volume;
			_mediaPlayer.Volume = 0;
		}
		else
		{
			_mediaPlayer.Volume = _unmutedVolume;
		}
	}
	public long GetDuration()
	{
		if (_media is null)
			return -1;
		
		return _media.Duration;
	}
	public void SetTimeCallback(EventHandler<MediaPlayerTimeChangedEventArgs> callback)
	{
		if (_mediaPlayer is null)
			return;

		_mediaPlayer.TimeChanged += callback;
	}
	public void SetOutputDevice(string id)
	{
		if (_mediaPlayer is null)
			return;
		
		_mediaPlayer.SetOutputDevice(id);
	}

	public void Dispose()
	{
		_libVlc.Dispose();
		_media?.Dispose();
		_mediaPlayer?.Dispose();
	}
}