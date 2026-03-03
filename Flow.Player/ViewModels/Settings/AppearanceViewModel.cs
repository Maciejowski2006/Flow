using CommunityToolkit.Mvvm.Messaging;
using Flow.Player.Messages;

namespace Flow.Player.ViewModels.Settings;

public class AppearanceViewModel : ViewModelBase
{
	private static readonly Models.Settings Settings = Models.Settings.Instance;
	
	public bool EnableBlur
	{
		get;
		set
		{
			SetProperty(ref field, value);
			Settings.EnableBlur = value;
			Settings.WriteSettings();
			WeakReferenceMessenger.Default.Send<SettingsUpdatedMessage>();
		}
	} = Settings.EnableBlur;
}