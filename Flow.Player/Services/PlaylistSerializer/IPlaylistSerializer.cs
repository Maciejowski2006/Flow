using System.Collections.Generic;
using System.Threading.Tasks;
using Flow.Player.Models;

namespace Flow.Player.Services.PlaylistSerializer;

public interface IPlaylistSerializer
{
	public Task<IReadOnlyList<Track>?> DeserializeAsync(string path);
	public Task SerializeAsync(IReadOnlyList<Track> playlist, string path);
}