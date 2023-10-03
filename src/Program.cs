namespace RichPresenceApp;

public class Program
{
	public static void Main( string[] args )
	{
		Console.Title = "CS2 Discord Rich Presence";
		Console.WriteLine( "=============== CS2 DISCORD RICH PRESENCE ===============" );
		Console.WriteLine( "Create by Giannis \"Retr0\" Kepas with :>" );
		Console.WriteLine( "Source: https://github.com/Retr0-01/CS2-Discord-RP" );
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
