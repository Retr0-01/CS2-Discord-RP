namespace RichPresenceApp;

public static class Program
{
	[STAThread]
	static void Main()
	{
		Application.SetCompatibleTextRenderingDefault( false );
		Application.Run( new RichPresenceAppContext() );
	}
}

public class RichPresenceAppContext : ApplicationContext
{
	public RichPresenceAppContext()
	{
		ConsoleManager.AllocConsole();
		ConsoleManager.SetConsoleWindowVisibility( false );

		Console.Title = "Counter-Strike 2 Discord Rich Presence";
		Console.WriteLine( "=============== CS2 DISCORD RICH PRESENCE ===============" );
		Console.WriteLine( "Create by Giannis \"Retr0\" Kepas with love :>" );
		Console.WriteLine( "Source: https://github.com/Retr0-01/CS2-Discord-RP" );
		Console.WriteLine();

		SystemTray.Setup();
		ApplicationSetup.Configure();
		DiscordManager.Initialize();
		// Main application loop. Code after this point will be run only on shutdown.
		HttpServer.Start();

		Exit();
	}

	private static void Exit()
	{
		SystemTray.NotifyIcon.Visible = false;
		ConsoleManager.FreeConsole();
		DiscordManager.Client.Dispose();
		Application.Exit();
	}
}
