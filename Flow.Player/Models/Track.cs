using System.IO;
using Avalonia.Media.Imaging;
using Flow.Player.Services.Tools;

namespace Flow.Player.Models;

public class Track
{
	public Metadata Metadata { get; set; }
	public Bitmap? CoverArt { get; set; }
	public bool DoesHaveCover => CoverArt is not null;
	public string FilePath { get; set; }
	
	public Track(string path)
	{
		FilePath = path;
		FileInfo trackFile = new(path);
		
		Metadata = MetadataToolkit.GetMetadata(trackFile);
		CoverArt = MetadataToolkit.GetCoverArt(trackFile);
	}
}
