using System.IO;
using System.Linq;
using File = TagLib.File;

namespace Flow.Player.Services.Tools;

public static class ID3Toolkit
{
	private static readonly string[] Id3SupportedTypes = [".aa", ".aax", ".aac", ".aiff", ".ape", ".dsf", ".flac", ".m4a", ".m4b", ".m4p", ".mp3", ".mpc", ".mpp", ".ogg", ".oga", ".wav", ".wma", ".wv", ".webm"];

	public static Id3Metadata GetMetadata(FileInfo file)
	{
		File? id3 = File.Create(file.FullName);
		Id3Metadata metadata = new(
			string.IsNullOrEmpty(id3.Tag.Title) ? file.Name : id3.Tag.Title,
			id3.Tag.AlbumArtists?.FirstOrDefault() ?? "",
			id3.Tag.Album,
			id3.Tag.Genres?.FirstOrDefault() ?? "",
			id3.Tag.Year,
			id3.Tag.Track
		);
		
	}
}

public record Id3Metadata(
	string Title,
	string Artist,
	string Album,
	string Genre,
	uint Year,
	uint TrackNumber
);