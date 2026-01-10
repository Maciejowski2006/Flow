using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using SoundFlow.Abstracts.Devices;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Codecs.FFMpeg;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Interfaces;
using SoundFlow.Providers;
using SoundFlow.Structs;

namespace Flow.Player.Services.AudioEngineService;

public sealed class SoundFlowAudioEngineService : IAudioEngineService, IDisposable
{
	private readonly MiniAudioEngine _engine;
	private AudioPlaybackDevice _playbackDevice;
	private readonly DeviceInfo _selectedDevice;
	private SoundPlayer? _soundPlayer;
	private ISoundDataProvider? _dataProvider;

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

	public SoundFlowAudioEngineService()
	{

		_engine = new();
		_engine.RegisterCodecFactory(new FFmpegCodecFactory());
		AudioOutputDevices = _engine.PlaybackDevices.Select(x => new AudioOutputDevice(x.Name, x.Id)).ToList();
		_selectedDevice = _engine.PlaybackDevices.FirstOrDefault(x => x.IsDefault);
		CurrentOutputDevice = new(_selectedDevice.Name, _selectedDevice.Id);
	}

	public Task LoadFile(string filePath)
	{
		// Dispose if there was a track playing before
		_soundPlayer?.Stop();
		_soundPlayer?.Dispose();

		FileInfo fileInfo = new(filePath);

		if (NativeCodecs.Contains(fileInfo.Extension))
			LoadNative(filePath);
		else
			LoadReencoder(filePath);

		_playbackDevice.MasterMixer.AddComponent(_soundPlayer);
		_playbackDevice.Start();
		_soundPlayer.Play();
		_soundPlayer.PlaybackEnded += (_, _) => PlaybackEnded?.Invoke(this, EventArgs.Empty);
		return Task.CompletedTask;
	}
	public void Play() { _soundPlayer?.Play(); }
	public void Pause() { _soundPlayer?.Pause(); }
	public void Stop() { _soundPlayer?.Stop(); }
	public void SetMute(bool mute)
	{
		if (_soundPlayer != null)
			_soundPlayer.Mute = mute;
	}
	public void Seek(TimeSpan time, SeekOrigin origin) { _soundPlayer?.Seek(time, origin); }
	public void SetOutputDevice(nint id)
	{
		DeviceInfo newDevice = _engine.PlaybackDevices.FirstOrDefault(x => x.Id == id);
		_playbackDevice = _engine.SwitchDevice(_playbackDevice, newDevice);
		if (_playbackDevice.Info.HasValue)
			CurrentOutputDevice = new(_playbackDevice.Info.Value.Name, _playbackDevice.Info.Value.Id);
	}
	public event EventHandler? PlaybackEnded;
	public void Dispose()
	{
		_engine.Dispose();
		_playbackDevice.Dispose();
		_soundPlayer?.Dispose();
		_dataProvider?.Dispose();
		GC.SuppressFinalize(this);
	}
	
	private void LoadNative(string filePath)
	{
		AudioFormat format = AudioFormat.DvdHq;
		
		_playbackDevice = _engine.InitializePlaybackDevice(_selectedDevice, format);
		_dataProvider = new StreamDataProvider(_engine, format, File.OpenRead(filePath));
		_soundPlayer = new(_engine, format, _dataProvider);
	}
	private void LoadReencoder(string filePath)
	{
		// Decode
		try
		{
			using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
			using ISoundDecoder decoder = _engine.CreateDecoder(fs, out AudioFormat format);

			float[] rawAudio = new float[decoder.Length];
			decoder.Decode(rawAudio);

			using MemoryStream ms = new(rawAudio.Length * sizeof(float));
			using ISoundEncoder encoder = _engine.CreateEncoder(ms, "wav", format);
			encoder.Encode(rawAudio);
			
			_playbackDevice = _engine.InitializePlaybackDevice(_selectedDevice, format);
			_dataProvider = new AssetDataProvider(_engine, ms);
			_soundPlayer = new(_engine, format, _dataProvider);
		}
		catch (Exception e)
		{
			Log.Error("{Message}", e.Message);
		}
	}
	
	
	
	private static readonly string[] NativeCodecs = [".flac", ".mp3", ".wav"];
}