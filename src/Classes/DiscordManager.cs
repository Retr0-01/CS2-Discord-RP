using DiscordRPC;
using DiscordRPC.Logging;

namespace RichPresenceApp;

public class DiscordManager
{
	public static DiscordRpcClient Client;

	public static void Initialize()
	{
		// Create our Discord client. This is the ID of the application I have created which
		// contains all image assets and icons. There is no reason to replace it.
		Client = new DiscordRpcClient( "872181511334543370", autoEvents: false )
		{
			Logger = new ConsoleLogger() { Level = LogLevel.Warning }
		};

		// Subscribe to events.
		Client.OnReady += ( sender, args ) =>
		{
			Console.WriteLine( "[DISCORD] Received 'ready' from user {0}", args.User.Username );
		};

		Client.OnPresenceUpdate += ( sender, args ) =>
		{
			Console.WriteLine( "[DISCORD] Presence updated!" );
		};

		// Connect to the RPC.
		Client.Initialize();
	}
}
