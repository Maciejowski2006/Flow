using Avalonia.Controls;
using Avalonia.Interactivity;
using Flow.Player.Messages;

namespace Flow.Player.Views;

public partial class MessageBoxWindow : Window
{
	public MessageBoxWindow(string message, MessageBoxButtons buttons)
	{
		InitializeComponent();

		Message.Text = message;
		Control[] controls = buttons switch
		{
			MessageBoxButtons.Ok => [CreateButton("OK", MessageBoxReturn.Ok)],
			MessageBoxButtons.OkCancel => [CreateButton("OK", MessageBoxReturn.Ok), CreateButton("Cancel", MessageBoxReturn.Cancel)],
			MessageBoxButtons.YesNo => [CreateButton("Yes", MessageBoxReturn.Yes), CreateButton("No", MessageBoxReturn.No)],
			MessageBoxButtons.YesNoCancel => [CreateButton("Yes", MessageBoxReturn.Yes), CreateButton("No", MessageBoxReturn.No), CreateButton("Cancel", MessageBoxReturn.Cancel)],
			_ => [CreateButton("OK", MessageBoxReturn.Ok)]
		};
		DialogButtons.Children.AddRange(controls);
	}
	
	private Control CreateButton(string text, MessageBoxReturn messageBoxReturn)
	{
		Button b = new()
		{
			Content = text,
			Margin = new(4, 0, 0, 0)
		};
		b.Click += (_, _) => Close(messageBoxReturn);

		return b;
	}

	protected override void OnLoaded(RoutedEventArgs e)
	{
		base.OnLoaded(e);
		MinWidth = Width;
		MinHeight = Height;
		SizeToContent = SizeToContent.Manual;
	}
}