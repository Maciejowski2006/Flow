namespace Flow.Player.Services;

public class CommandLineArgumentsService(string[] args)
{
	public string[] Arguments { get; init; } = args;
}