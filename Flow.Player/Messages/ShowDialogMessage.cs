using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Flow.Player.Messages;

public class ShowDialogMessage(string message, MessageBoxButtons buttons) : AsyncRequestMessage<MessageBoxReturn?>
{
	public readonly string Message = message;
	public readonly MessageBoxButtons Buttons = buttons;
}
public enum MessageBoxReturn
{
	Ok,
	Yes,
	No,
	Cancel
}

public enum MessageBoxButtons
{
	Ok,
	OkCancel,
	YesNo,
	YesNoCancel,
}