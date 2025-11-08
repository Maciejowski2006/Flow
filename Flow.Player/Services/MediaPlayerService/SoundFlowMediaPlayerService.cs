using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using SoundFlow.Abstracts.Devices;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Interfaces;
using SoundFlow.Providers;
using SoundFlow.Structs;

namespace Flow.Player.Services.MediaPlayerService;

public class SoundFlowMediaPlayerService : IMediaPlayerService, IDisposable
{
	private readonly MiniAudioEngine _engine;
	private AudioPlaybackDevice _playbackDevice;
	private SoundPlayer? _soundPlayer;
	private ISoundDataProvider? _dataProvider;

	private readonly AudioFormat _format = new()
	{
		SampleRate = 48000,
		Channels = 2,
		Format = SampleFormat.F32
	};
	public float Time => _soundPlayer?.Time ?? 0;
	public float Volume
	{
		get => _soundPlayer?.Volume ?? 0;
		set
		{
			if (_soundPlayer != null)
				_soundPlayer.Volume = value;
		}
	}
	public float Duration => _soundPlayer?.Duration ?? -1;
	public List<AudioOutputDevice> AudioOutputDevices { get; }
	public AudioOutputDevice CurrentOutputDevice { get; private set; }

	public SoundFlowMediaPlayerService()
	{
		_engine = new();
		AudioOutputDevices = _engine.PlaybackDevices.Select(x => new AudioOutputDevice(x.Name, x.Id)).ToList();
		DeviceInfo selectedDevice = _engine.PlaybackDevices.FirstOrDefault(x => x.IsDefault);
		CurrentOutputDevice = new (selectedDevice.Name, selectedDevice.Id);
		_playbackDevice = _engine.InitializePlaybackDevice(selectedDevice, _format);
	}

	public Task LoadFile(string filePath)
	{ 
		// Dispose if there was a track playing before
		_soundPlayer?.Stop();
		_soundPlayer?.Dispose();

		_dataProvider = new StreamDataProvider(_engine, _format, File.OpenRead(filePath));
		_soundPlayer = new(_engine, _format, _dataProvider);
		_playbackDevice.MasterMixer.AddComponent(_soundPlayer);

		_playbackDevice.Start();
		_soundPlayer.Play();
		return Task.CompletedTask;
	}
	public void Play()
	{
		_soundPlayer?.Play();
	}
	public void Pause()
	{
		_soundPlayer?.Pause();
	}
	public void Stop()
	{
		_soundPlayer?.Stop();
	}
	public void SetMute(bool mute)
	{
		if (_soundPlayer != null)
			_soundPlayer.Mute = mute;
	}
	public void Seek(TimeSpan time, SeekOrigin origin)
	{
		_soundPlayer?.Seek(time, origin);
	}
	public void SetOutputDevice(nint id)
	{
		DeviceInfo newDevice = _engine.PlaybackDevices.FirstOrDefault(x => x.Id == id);
		_playbackDevice = _engine.SwitchDevice(_playbackDevice, newDevice);
		if (_playbackDevice.Info.HasValue)
			CurrentOutputDevice = new(_playbackDevice.Info.Value.Name, _playbackDevice.Info.Value.Id);
	}
	public void Dispose()
	{
		_engine.Dispose();
		_playbackDevice.Dispose();
		_soundPlayer?.Dispose();
		_dataProvider?.Dispose();
		GC.SuppressFinalize(this);
	}
}