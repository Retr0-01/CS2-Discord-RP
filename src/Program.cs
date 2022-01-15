namespace CSGODiscordRP;

public class Program
{
	public static void Main( string[] args )
	{
		Console.Title = "CS:GO Discord Rich Presence";

		DiscordManager.Initialize();
		HttpServer.Start();
	}

	static void CurrentDomain_ProcessExit( object sender, EventArgs e )
	{
		Console.WriteLine( "exit" );
		DiscordManager.Client.Dispose();
	}
}
