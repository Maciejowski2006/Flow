using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Flow.Player.Models;

namespace Flow.Player.Services.PlaybackSubsystem;

public interface IPlaybackSubsystem
{
	public PlaybackMode PlaybackMode { get; set; }
	public Track? CurrentTrack { get; }
	public ObservableCollection<Track> Playlist { get; }
	public event EventHandler<TrackChangedEventArgs> TrackChanged;

	public Task LoadTrackAsync(string path);
	public Task PlayTrackFromPlaylistAsync(Track track);
	public Task PlayPreviousFromPlaylistAsync();
	public Task PlayNextFromPlaylistAsync();
}

public enum PlaybackMode
{
	Single,
	Playlist,
}

public class TrackChangedEventArgs(Track? track, float duration) : EventArgs
{
	public Track? NewTrack { get; } = track;
	public float Duration { get; } = duration;
}