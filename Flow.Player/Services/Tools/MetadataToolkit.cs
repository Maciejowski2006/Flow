using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Media.Imaging;
using TagLib;
using File = TagLib.File;

namespace Flow.Player.Services.Tools;

public static class MetadataToolkit
{
	private static readonly string[] Id3SupportedTypes = [".aa", ".aax", ".aac", ".aiff", ".ape", ".dsf", ".flac", ".m4a", ".m4b", ".m4p", ".mp3", ".mpc", ".mpp", ".ogg", ".oga", ".wav", ".wma", ".wv", ".webm"];

	private static readonly string[] ImageExtensions = [ ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".tif", ".webp", ".heic", ".heif", ".avif",];


	public static Metadata GetMetadata(FileInfo file)
	{
		Metadata metadata;
		if (Id3SupportedTypes.Contains(file.Extension))
		{
			File? id3 = File.Create(file.FullName);
			metadata = new(
				string.IsNullOrEmpty(id3.Tag.Title) ? file.Name : id3.Tag.Title,
				id3.Tag.AlbumArtists?.FirstOrDefault() ?? "",
				id3.Tag.Album,
				id3.Tag.Genres?.FirstOrDefault() ?? "",
				id3.Tag.Track
			);
		}
		else
		{
			metadata = new(file.Name);
		}
		
		return metadata;
	}

	public static Bitmap? GetCoverArt(FileInfo file)
	{
		if (!Id3SupportedTypes.Contains(file.Extension))
			return null;
		
		File? id3 = File.Create(file.FullName);
		IPicture[] pictures = id3.Tag.Pictures;
		if (pictures.Length > 0)
		{
			byte[] imageBytes = pictures[0].Data.Data;
			
			using MemoryStream memoryStream = new(imageBytes);
			return new(memoryStream);
		}
		
		int extensionLength = file.Extension.Length;
		string fileWithoutExtension = file.Name.Substring(0, file.Name.Length - extensionLength);
		string trackDirectory = file.DirectoryName!;
		IEnumerable<string> covers = Directory.GetFiles(trackDirectory, $"{fileWithoutExtension}.*").Where(x => ImageExtensions.Any(x.EndsWith));
		if (covers.FirstOrDefault() is not { } cover)
			return null;

		string coverPath = Path.Combine(trackDirectory, cover);
		return new(coverPath);

	}
}

public record Metadata(
	string Title,
	string Artist = "",
	string Album = "",
	string Genre = "",
	uint TrackNumber = 0
);