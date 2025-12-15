using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Flow.Player.Models;

namespace Flow.Player.Services.PlaylistSerializer;

public class JsonPlaylistSerializer : IPlaylistSerializer
{
	public async Task<IReadOnlyList<Track>?> DeserializeAsync(string path)
	{
		Stream stream = File.OpenRead(path);
		IReadOnlyList<string>? playlist = await JsonSerializer.DeserializeAsync<IReadOnlyList<string>>(stream);
		stream.Close();
		
		IReadOnlyList<Track>? restoredPlaylist = playlist?.Select(x => new Track(x)).ToList();
		return restoredPlaylist;
		
	}
	public async Task SerializeAsync(IReadOnlyList<Track> playlist, string path)
	{
		Stream stream = File.Create(path);
		IEnumerable<string> mappedPlaylist = playlist.Select(x => x.FilePath);
		await JsonSerializer.SerializeAsync(stream, mappedPlaylist, new JsonSerializerOptions
		{
			#if DEBUG
			WriteIndented = true,
			#endif
		});
		stream.Close();
	}
}