using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using FFMpegCore;
using FFMpegCore.Enums;
using Flow.Player.Extensions;
using SoundFlow.Abstracts.Devices;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Interfaces;
using SoundFlow.Providers;
using SoundFlow.Structs;
using Log = Serilog.Log;

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

	private static readonly string[] NativeCodecs = [".aac", ".aiff", ".flac", ".m4a", ".mp3", ".ogg", ".wav"];

	public SoundFlowAudioEngineService()
	{
		Log.Information("Initializing audio engine");
		_engine = new();
		AudioOutputDevices = _engine.PlaybackDevices.Select(x => new AudioOutputDevice(x.Name, x.Id)).ToList();
		Log.Information("Audio output devices found: {Devices}", AudioOutputDevices.ToPrintableString(x => x.Name));
		_selectedDevice = _engine.PlaybackDevices.FirstOrDefault(x => x.IsDefault);
		CurrentOutputDevice = new(_selectedDevice.Name, _selectedDevice.Id);
	}

	public async Task LoadFile(string filePath)
	{
		// Dispose if there was a track playing before
		_soundPlayer?.Stop();
		_soundPlayer?.Dispose();

		FileInfo fileInfo = new(filePath);
		if (!NativeCodecs.Contains(fileInfo.Extension))
		{
			Log.Information("Reencoding file: {filePath}", filePath);
			filePath = ReencodeAudio(fileInfo);
		}

		IMediaAnalysis mediaInfo = await FFProbe.AnalyseAsync(filePath);
		if (mediaInfo.PrimaryAudioStream is not { } audioStream)
			return;
		
		AudioFormat format = new()
		{
			Channels = audioStream.Channels,
			SampleRate = audioStream.SampleRateHz,
			Format = SampleFormat.S32
		};

		LoadNativeCodec(filePath, format);
		
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
		Log.Information("Settings new output device: {Device}", newDevice.Name);
		_playbackDevice = _engine.SwitchDevice(_playbackDevice, newDevice);
		if (_playbackDevice.Info.HasValue)
			CurrentOutputDevice = new(_playbackDevice.Info.Value.Name, _playbackDevice.Info.Value.Id);
	}
	public event EventHandler? PlaybackEnded;

	private void LoadNativeCodec(string filePath, AudioFormat format)
	{
		_playbackDevice = _engine.InitializePlaybackDevice(_selectedDevice, format);
		_dataProvider = new StreamDataProvider(_engine, format, File.OpenRead(filePath));
		_soundPlayer = new(_engine, format, _dataProvider);

		_playbackDevice.MasterMixer.AddComponent(_soundPlayer);
		_playbackDevice.Start();
		_soundPlayer.Play();

		_soundPlayer.PlaybackEnded += (_, _) => PlaybackEnded?.Invoke(this, EventArgs.Empty);
	}

	private string ReencodeAudio(FileInfo fileInfo)
	{
		DirectoryInfo tempDir = Directory.CreateTempSubdirectory();
		string oldFile = Path.GetFileNameWithoutExtension(fileInfo.Name);
		string newFile = Path.Combine(tempDir.FullName, $"{oldFile}.wav");
		Log.Information(newFile);
		FFMpegArguments.FromFileInput(fileInfo)
			.OutputToFile(newFile, true, options => options
				.WithAudioCodec("pcm_s16le")
			)
			.ProcessSynchronously();
		return newFile;
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