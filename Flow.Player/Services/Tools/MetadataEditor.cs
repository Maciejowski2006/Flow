using System;
using TagLib;

namespace Flow.Player.Services.Tools;

public static class MetadataEditor
{
	public static void EmbedCoverArt(string trackPath, string imagePath)
	{
		File? metadata = File.Create(trackPath);
		if (metadata is null)
			return;

		byte[] coverArt = System.IO.File.ReadAllBytes(imagePath);
		metadata.Tag.Pictures =
		[
			new Picture(coverArt)
			{
				Type = PictureType.FrontCover
			}
		];
		metadata.Save();
	}
}