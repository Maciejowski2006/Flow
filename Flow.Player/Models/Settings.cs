using System;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace Flow.Player.Models;

public sealed class Settings
{
	[JsonIgnore] private static readonly string SettingsFilePath = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
		"Flow",
		"settings.json"
	);

	public bool EnableBlur { get; set; }

	[JsonConstructor]
	private Settings() { }

	private static readonly Lock Padlock = new();
	public static Settings Instance
	{
		get
		{
			lock (Padlock)
			{
				if (field is not null)
					return field;
	
				string json = File.ReadAllText(SettingsFilePath);
				Settings? settings =  JsonConvert.DeserializeObject<Settings>(json);
				if (settings is null)
				{
					settings = new();
					settings.WriteSettings();
				}
				field = settings;
				return field;
			}
		}
	} = null!;

	public void WriteSettings()
	{
		string json = JsonConvert.SerializeObject(this, Formatting.Indented);
		File.WriteAllText(SettingsFilePath, json);
	}
}