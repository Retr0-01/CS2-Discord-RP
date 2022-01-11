namespace CSGODiscordRP;

public class Program
{
	public static void Main( string[] args )
	{
		DiscordManager.Start();
		HttpServer.Start();
	}
}
