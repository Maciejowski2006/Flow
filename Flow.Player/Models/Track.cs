using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Media.Imaging;
using TagLib;
using File = TagLib.File;

namespace Flow.Player.Models;



public class Track
{
	
	
	public string Title { get; set; }
	public string Artist { get; set; }
	public string Album { get; set; }
	public string Genre { get; set; }
	public uint Year { get; set; }
	public uint TrackNumber { get; set; }
	public Bitmap? CoverArt { get; set; }
	public bool DoesHaveCover => CoverArt is not null;

	public Track(string path)
	{
		FileInfo trackFile = new(path);
		File? id3 = File.Create(trackFile.FullName);
		Title = id3.Tag.Title;
		Artist = id3.Tag.AlbumArtists?.FirstOrDefault() ?? "";
		Album = id3.Tag.Album;
		Genre = id3.Tag.Genres?.FirstOrDefault() ?? "";
		Year = id3.Tag.Year;
		TrackNumber = id3.Tag.Track;
		IPicture[] pictures = id3.Tag.Pictures;
		if (pictures.Length > 0)
		{
			byte[] imageBytes = pictures[0].Data.Data;
			
			using MemoryStream memoryStream = new(imageBytes);
			CoverArt = new(memoryStream);
			return;
		}
		
		int extensionLength = trackFile.Extension.Length;
		string fileWithoutExtension = trackFile.Name.Substring(0, trackFile.Name.Length - extensionLength);
		string trackDirectory = trackFile.DirectoryName!;
		IEnumerable<string> covers = Directory.GetFiles(trackDirectory, $"{fileWithoutExtension}.*").Where(x => ImageExtensions.Any(x.EndsWith));
		if (covers.FirstOrDefault() is { } cover)
		{
			string coverPath = Path.Combine(trackDirectory, cover);
			CoverArt = new(coverPath);
		}
	}
	
	private static readonly string[] ImageExtensions =
	[
		".jpg",
		".jpeg",
		".png",
		".bmp",
		".tiff",
		".tif",
		".webp",
		".heic",
		".heif",
		".avif",
	];
}
