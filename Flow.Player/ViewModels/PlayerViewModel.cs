using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Flow.Player.Messages;
using Flow.Player.Models;
using Flow.Player.Services;
using Flow.Player.Services.AudioEngineService;
using Flow.Player.Services.PlaybackSubsystem;

namespace Flow.Player.ViewModels;

public partial class PlayerViewModel : ViewModelBase
{
	public bool IsSeeking { get; set; }
	[ObservableProperty] private Track? _playingTrack;
	[ObservableProperty] private TimeSpan _duration;
	[ObservableProperty] private TimeSpan _time;
	[ObservableProperty] private float _sliderDuration;
	public float SliderTime
	{
		get;
		set
		{
			if (IsSeeking)
				return;

			SetProperty(ref field, value);
		}
	}
	public float Volume
	{
		get;
		set
		{
			float v = Math.Clamp(value, 0, 1);
			_player.SetMute(false);
			_player.Volume = v;
			Muted = false;
			SetProperty(ref field, v);
		}
	} = 1;
	public bool IsPlaying
	{
		get;
		set
		{
			if (value)
				_player.Play();
			else
				_player.Pause();

			SetProperty(ref field, value);
		}
	}
	[ObservableProperty] private bool _showPlaylistView;

	[ObservableProperty] private bool _muted;
	private readonly IAudioEngineService _player;
	private readonly IPlaybackSubsystem _playbackSubsystem;

	public PlayerViewModel(IAudioEngineService playerService, IPlaybackSubsystem playbackSubsystem, CommandLineArgumentsService commandLineArgumentsService)
	{
		_player = playerService;
		_playbackSubsystem = playbackSubsystem;

		_playbackSubsystem.TrackChanged += (_, args) =>
		{
			PlayingTrack = args.NewTrack;
			Duration = TimeSpan.FromSeconds(args.Duration);
			SliderDuration = args.Duration;

			_player.Volume = Volume;

			DispatcherTimer timer = new()
			{
				Interval = TimeSpan.FromMilliseconds(100)
			};
			timer.Tick += (_, _) => UpdateTimers();
			timer.Start();

			IsPlaying = true;
		};

		string[] args = commandLineArgumentsService.Arguments;
		if (commandLineArgumentsService.Arguments.Length == 0)
			return;

		WeakReferenceMessenger.Default.Register<PlaylistViewChangedMessage>(this, (_, message) => ShowPlaylistView = message.Value);
		IEnumerable<string> formats = FilePickerService.AudioAll.Patterns!.Select(x => x.Substring(1, x.Length - 1));
		if (formats.Any(x => args[0].EndsWith(x)))
			_ = LoadTrack(args[0]);
	}

	private async Task LoadTrack(string filePath) { await _playbackSubsystem.LoadTrackAsync(filePath); }

	private void UpdateTimers()
	{
		Time = TimeSpan.FromSeconds(_player.Time);
		SliderTime = _player.Time;
	}

	[RelayCommand]
	public void ToggleMute() => _player.SetMute(Muted);

	public void Seek(TimeSpan value, SeekOrigin origin = SeekOrigin.Current)
	{
		_player.Seek(value, origin);
		UpdateTimers();
	}

	[RelayCommand]
	private void PlayPrevious()
	{
		if (_playbackSubsystem.PlaybackMode == PlaybackMode.Playlist && Time < TimeSpan.FromSeconds(2))
		{
			_playbackSubsystem.PlayPreviousFromPlaylistAsync();
			return;
		}

		// Seek to beginning if more than 2 seconds have passed
		Seek(TimeSpan.FromSeconds(0), SeekOrigin.Begin);
		OnPropertyChanged(nameof(SliderTime));
	}
	[RelayCommand]
	private void PlayNext()
	{
		if (_playbackSubsystem.PlaybackMode != PlaybackMode.Playlist)
			return;

		_playbackSubsystem.PlayNextFromPlaylistAsync();
	}

	[RelayCommand]
	private void TogglePlaylistView()
	{
		ShowPlaylistView = !ShowPlaylistView;
		WeakReferenceMessenger.Default.Send(new PlaylistViewChangedMessage(ShowPlaylistView));
	}
}