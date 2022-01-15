using System.Net;
using System.Text;
using DiscordRPC;
using GameState;

namespace CSGODiscordRP;

public class HttpServer
{
	public static HttpListener Listener = new();
	public static readonly string Url = "http://localhost:3000/";

	public static void Start()
	{
		Listener.Prefixes.Add( Url );
		Listener.Start();
		Console.WriteLine( $"[SERVER] - Listening for data on {Url}" );

		// Handle requests.
		Task ListenTask = HandleRequests();
		ListenTask.GetAwaiter().GetResult();

		Listener.Close();
	}

	public static async Task HandleRequests()
	{
		bool IsRunning = true;

		while ( IsRunning )
		{
			string RawData = "";
			TopLevel topLevel = null;

			// Default presence will be set to menu.
			RichPresence Presence = new()
			{
				Details = "Main Menu",
				State = "In Menu",
				Assets = new Assets()
				{
					LargeImageKey = "menu",
					LargeImageText = "Menu",
				}
			}; ;

			HttpListenerContext Context = await Listener.GetContextAsync();
			HttpListenerRequest Request = Context.Request;
			HttpListenerResponse Response = Context.Response;

			if ( Request.HttpMethod == "POST" )
			{
				Console.WriteLine( "[SERVER] - Incoming data..." );
				using ( var Reader = new StreamReader( Request.InputStream, Request.ContentEncoding ) )
				{
					RawData = Reader.ReadToEnd();
				}

				topLevel = TopLevel.FromJson( RawData );
			}

			byte[] Data = Encoding.UTF8.GetBytes( RawData );

			Response.ContentType = "application/json";
			Response.ContentEncoding = Encoding.UTF8;
			Response.ContentLength64 = Data.LongLength;

			// Write out the data. We need to do this regardless if it's used or not.
			await Response.OutputStream.WriteAsync( Data );

			// Let's start building our rich presence.
			// First check if we are really playing.
			if ( topLevel.Player.Activity == "playing" )
			{
				// Regardless of the gamemode we play, we will have the large image set to the map.
				Presence = new RichPresence()
				{
					Assets = new Assets()
					{
						LargeImageKey = topLevel.Map.Name,
						LargeImageText = $"Playing on {topLevel.Map.Name}",
					},
				};

				string FixedModeName = null;

				// Let's create different presences for each gamemode.
				switch ( topLevel.Map.Mode )
				{
					// Casual, competitive and wingman.
					case "casual":
					case "competitive":
					case "scrimcomp2v2":

						if ( topLevel.Map.Mode == "scrimcomp2v2" ) FixedModeName = "Wingman";

						Presence.Details = $"{FirstCharToUpper( topLevel.Player.Activity )} {FixedModeName ?? FirstCharToUpper( topLevel.Map.Mode )}";
						Presence.State = $"{FirstCharToUpper( topLevel.Map.Phase )} - CT: {topLevel.Map.TeamCt.Score} | {topLevel.Map.TeamT.Score} :T";

						// We are not spectating someone else, we have fully connected to the server and we are on a team.
						if ( topLevel.Provider.Steamid == topLevel.Player.Steamid && topLevel.Player.Name != "unconnected" && topLevel.Player.State.Health > 0 )
						{
							Presence.Assets.SmallImageKey = $"{topLevel.Player.Team.ToLower()}_logo";
							Presence.Assets.SmallImageText = $"Playing as {topLevel.Player.Team}";
						}
						// We are spectating/observing.
						else
						{
							Presence.Details = $"Watching {FixedModeName ?? FirstCharToUpper( topLevel.Map.Mode )}";
							Presence.Assets.SmallImageKey = topLevel.Map.Mode;
							Presence.Assets.SmallImageText = "Spectating/Observing";
						}
						break;

					// Arms race, demolition and deathmatch.
					case "gungameprogressive":
					case "gungametrbomb":
					case "deathmatch":
						if ( topLevel.Map.Mode == "gungameprogressive" ) FixedModeName = "Armsrace";
						if ( topLevel.Map.Mode == "gungametrbomb" ) FixedModeName = "Demolition";

						Presence.Details = $"{FirstCharToUpper( topLevel.Player.Activity )} {FixedModeName ?? FirstCharToUpper( topLevel.Map.Mode )}";
						Presence.State = $"{FirstCharToUpper( topLevel.Map.Phase )} - Kills: {topLevel.Player.MatchStats.Kills}";

						Presence.Assets.SmallImageKey = topLevel.Map.Mode;
						Presence.Assets.SmallImageText = FixedModeName ?? FirstCharToUpper( topLevel.Map.Mode );
						break;

					// Training.
					case "training":
						Presence.Details = "Training";
						break;

					// Guardian and Co-Op missions.
					case "cooperative":
					case "coopmission":
						if ( topLevel.Map.Mode == "cooperative" ) FixedModeName = "Guardian";
						if ( topLevel.Map.Mode == "coopmission" ) FixedModeName = "Co-Op Strike";

						Presence.Details = $"{FirstCharToUpper( topLevel.Player.Activity )} {FixedModeName ?? FirstCharToUpper( topLevel.Map.Mode )}";
						Presence.State = $"Defending the objective - Kills: {topLevel.Player.MatchStats.Kills}";

						Presence.Assets.SmallImageKey = topLevel.Map.Mode;
						Presence.Assets.SmallImageText = FixedModeName;
						break;

					// Danger zone.
					case "survival":
						if ( topLevel.Map.Mode == "survival" ) FixedModeName = "Danger Zone";

						Presence.Details = $"{FirstCharToUpper( topLevel.Player.Activity )} {FixedModeName ?? FirstCharToUpper( topLevel.Map.Mode )}";

						if ( topLevel.Player.State.Health > 0 )
							Presence.State = $"Status: Alive - Kills: {topLevel.Player.MatchStats.Kills}";
						else Presence.State = $"Status: Dead - Kills: {topLevel.Player.MatchStats.Kills}";

						Presence.Assets.SmallImageKey = topLevel.Map.Mode;
						Presence.Assets.SmallImageText = FixedModeName;
						break;

					// Any other mode that we cannot recognize will have some default images.
					default:
						Presence.Details = "Custom Gamemode";
						Presence.State = "No Data";
						Presence.Assets.SmallImageKey = "default";
						break;
				}
			}

			// Set our new presence...
			DiscordManager.Client.SetPresence( Presence );
			// and apply the presence changes.
			DiscordManager.Client.Invoke();

			Response.Close();
		}
	}

	public static string FirstCharToUpper( string input ) =>
		input switch
		{
			null => throw new ArgumentNullException( nameof( input ) ),
			"" => throw new ArgumentException( $"{nameof( input )} cannot be empty", nameof( input ) ),
			_ => string.Concat( input[0].ToString().ToUpper(), input.AsSpan( 1 ) )
		};
}
