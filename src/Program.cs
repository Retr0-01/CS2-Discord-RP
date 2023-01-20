namespace RichPresenceApp;

public class Program
{
	public static void Main( string[] args )
	{
		Console.Title = "CS:GO Discord Rich Presence";
		Console.WriteLine( "=============== CS:GO DISCORD RICH PRESENCE ===============" );
		Console.WriteLine( "Create by Retr0#1799 :)" );
		Console.WriteLine( "Source Code: https://github.com/Retr0-01/CSGO-Discord-RP" );
		Console.WriteLine();

		ApplicationSetup.Configure();
		DiscordManager.Initialize();
		HttpServer.Start();
	}

	static void CurrentDomain_ProcessExit( object sender, EventArgs e )
	{
		DiscordManager.Client.Dispose();
	}
}
