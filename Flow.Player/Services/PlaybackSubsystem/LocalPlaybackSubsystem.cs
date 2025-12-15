using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Flow.Player.Models;
using Flow.Player.Services.AudioEngineService;

namespace Flow.Player.Services.PlaybackSubsystem;

public sealed class LocalPlaybackSubsystem : IPlaybackSubsystem
{
	private readonly IAudioEngineService _audioEngine;
	public ObservableCollection<Track> Playlist { get; set; } = [];

	public PlaybackMode PlaybackMode { get; set; }
	public Track? CurrentTrack
	{
		get;
		private set
		{
			field = value;
			TrackChanged?.Invoke(this, new(CurrentTrack, _audioEngine.Duration));
		}
	}
	
	public event EventHandler<TrackChangedEventArgs>? TrackChanged;

	public LocalPlaybackSubsystem(IAudioEngineService audioEngine)
	{
		_audioEngine = audioEngine;
		_audioEngine.PlaybackEnded += async (_, _) =>
		{
			await PlayNextFromPlaylistAsync();
		};
	}
	
	public async Task LoadTrackAsync(string path)
	{
		await _audioEngine.LoadFile(path);
		CurrentTrack = new(path);
		PlaybackMode = PlaybackMode.Single;
	}

	public async Task PlayTrackFromPlaylistAsync(Track track)
	{
		await _audioEngine.LoadFile(track.FilePath);
		CurrentTrack = track;
		PlaybackMode = PlaybackMode.Playlist;
	}
	public async Task PlayPreviousFromPlaylistAsync()
	{
		if (CurrentTrack is null)
			return;
		int index = Playlist.IndexOf(CurrentTrack);
		if (index == 0)
			return;
		
		await PlayTrackFromPlaylistAsync(Playlist[index - 1]);
	}
	public async Task PlayNextFromPlaylistAsync()
	{
		if (CurrentTrack is null)
			return;
		int index = Playlist.IndexOf(CurrentTrack);
		if (index == Playlist.Count - 1)
			return;
		await PlayTrackFromPlaylistAsync(Playlist[index + 1]);
	}
}