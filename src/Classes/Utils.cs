namespace RichPresenceApp;

internal class Utils
{
	public static void ExitApplication()
	{
		SystemTray.NotifyIcon.Visible = false;
		ConsoleManager.FreeConsole();
		DiscordManager.Client.Dispose();
		Environment.Exit( Environment.ExitCode );
		Application.Exit();
	}

	public static string FirstCharToUpper( string input ) =>
	input switch
	{
		null => throw new ArgumentNullException( nameof( input ) ),
		"" => throw new ArgumentException( $"{nameof( input )} cannot be empty", nameof( input ) ),
		_ => string.Concat( input[0].ToString().ToUpper(), input.AsSpan( 1 ) )
	};
}
