using DiscordRPC;
using DiscordRPC.Logging;
using GameState;

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

	public static RichPresence BuildPresenceFromData( TopLevel gameData )
	{
		// Default presence will be set to menu.
		RichPresence presence = new()
		{
			Details = "Main Menu",
			State = "In Menu",
			Assets = new Assets()
			{
				LargeImageKey = "menu",
				LargeImageText = "Menu",
			}
		};

		// If for whatever reason we have bad data OR one of our core JSON categories are null, return early.
		// For example, gameData.Player is null while loading into a game/map. We need to handle that properly.
		if ( gameData == null || gameData.Provider == null || gameData.Map == null || gameData.Player == null ) return presence;

		// Let's start building our rich presence.
		// First check if we are really playing.
		if ( gameData.Player.Activity == "playing" )
		{
			// Regardless of the gamemode we play, we will have the large image set to the map.
			presence = new RichPresence()
			{
				Assets = new Assets()
				{
					LargeImageKey = gameData.Map.Name,
					LargeImageText = $"Playing on {gameData.Map.Name}",
				},
			};

			string FixedModeName = null;

			// Let's create different presences for each gamemode.
			switch ( gameData.Map.Mode )
			{
				// Casual, competitive and wingman.
				case "casual":
				case "competitive":
				case "scrimcomp2v2":

					if ( gameData.Map.Mode == "scrimcomp2v2" ) FixedModeName = "Wingman";

					presence.Details = $"{Utils.FirstCharToUpper( gameData.Player.Activity )} {FixedModeName ?? Utils.FirstCharToUpper( gameData.Map.Mode )}";
					presence.State = $"{Utils.FirstCharToUpper( gameData.Map.Phase )} - CT: {gameData.Map.TeamCt.Score} | {gameData.Map.TeamT.Score} :T";

					// We are not spectating someone else, we have fully connected to the server and we are on a team.
					if ( gameData.Provider.Steamid == gameData.Player.Steamid && gameData.Player.Name != "unconnected" && gameData.Player.State.Health > 0 )
					{
						presence.Assets.SmallImageKey = $"{gameData.Player.Team.ToLower()}_logo";
						presence.Assets.SmallImageText = $"Playing as {gameData.Player.Team}";
					}
					// We are spectating/observing.
					else
					{
						presence.Details = $"Watching {FixedModeName ?? Utils.FirstCharToUpper( gameData.Map.Mode )}";
						presence.Assets.SmallImageKey = gameData.Map.Mode;
						presence.Assets.SmallImageText = "Spectating/Observing";
					}
					break;

				// Arms race, demolition and deathmatch.
				case "gungameprogressive":
				case "gungametrbomb":
				case "deathmatch":
					if ( gameData.Map.Mode == "gungameprogressive" ) FixedModeName = "Armsrace";
					if ( gameData.Map.Mode == "gungametrbomb" ) FixedModeName = "Demolition";

					presence.Details = $"{Utils.FirstCharToUpper( gameData.Player.Activity )} {FixedModeName ?? Utils.FirstCharToUpper( gameData.Map.Mode )}";
					presence.State = $"{Utils.FirstCharToUpper( gameData.Map.Phase )} - Kills: {gameData.Player.MatchStats.Kills}";

					presence.Assets.SmallImageKey = gameData.Map.Mode;
					presence.Assets.SmallImageText = FixedModeName ?? Utils.FirstCharToUpper( gameData.Map.Mode );
					break;

				// Training.
				case "training":
					presence.Details = "Training";
					break;

				// Guardian and Co-Op missions.
				case "cooperative":
				case "coopmission":
					if ( gameData.Map.Mode == "cooperative" ) FixedModeName = "Guardian";
					if ( gameData.Map.Mode == "coopmission" ) FixedModeName = "Co-Op Strike";

					presence.Details = $"{Utils.FirstCharToUpper( gameData.Player.Activity )} {FixedModeName ?? Utils.FirstCharToUpper( gameData.Map.Mode )}";
					presence.State = $"Defending the objective - Kills: {gameData.Player.MatchStats.Kills}";

					presence.Assets.SmallImageKey = gameData.Map.Mode;
					presence.Assets.SmallImageText = FixedModeName;
					break;

				// Danger zone.
				case "survival":
					if ( gameData.Map.Mode == "survival" ) FixedModeName = "Danger Zone";

					presence.Details = $"{Utils.FirstCharToUpper( gameData.Player.Activity )} {FixedModeName ?? Utils.FirstCharToUpper( gameData.Map.Mode )}";

					if ( gameData.Player.State.Health > 0 )
						presence.State = $"Status: Alive - Kills: {gameData.Player.MatchStats.Kills}";
					else presence.State = $"Status: Dead - Kills: {gameData.Player.MatchStats.Kills}";

					presence.Assets.SmallImageKey = gameData.Map.Mode;
					presence.Assets.SmallImageText = FixedModeName;
					break;

				// Any other mode that we cannot recognize will have some default images.
				default:
					presence.Details = "Custom Gamemode";
					presence.State = "No Data";
					presence.Assets.SmallImageKey = "default";
					break;
			}
		}
		return presence;
	}
}
