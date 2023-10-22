using GameState;
using System.Net;

namespace RichPresenceApp;

public class HttpServer
{
	private static readonly HttpListener Listener = new();

	public static void StartServer()
	{
		string url = $"http://localhost:{ApplicationSetup.AppPort}/";

		try
		{
			Listener.Prefixes.Add( url );
			Listener.Start();
			Console.WriteLine( $"[SERVER] Listening for data on {url}" );

			// Handle requests.
			Task ListenTask = HandleRequests();
			ListenTask.GetAwaiter().GetResult();

			Listener.Close();
		}
		catch ( Exception ex )
		{
			ConsoleManager.SetConsoleWindowVisibility( true );
			Console.WriteLine( "[SERVER] Error starting listen server!" );
			Console.Write( ex.ToString() );
			Console.WriteLine( "\n\nA common issue for this error is that the port is already being listened by an application." );
			Console.WriteLine( "Make sure that the rich presence app is not open more than once and try again.\n\nPress any key to close this..." );
			Console.ReadLine();
			Utils.ExitApplication();
		}
	}

	private static async Task HandleRequests()
	{
		while ( true )
		{
			try
			{
				HttpListenerContext Context = await Listener.GetContextAsync();
				HttpListenerRequest Request = Context.Request;
				HttpListenerResponse Response = Context.Response;

				if ( Request.HttpMethod == "POST" )
				{
					Console.WriteLine( "[SERVER] Incoming data..." );

					// Read the data that were sent by the game.
					using var Reader = new StreamReader( Request.InputStream, Request.ContentEncoding );
					string rawData = Reader.ReadToEnd();
					TopLevel gameData = TopLevel.FromJson( rawData );

					// Set our new presence...
					DiscordManager.Client.SetPresence( DiscordManager.BuildPresenceFromData( gameData ) );
					// and apply the presence changes.
					DiscordManager.Client.Invoke();
				}

				// We must return a response once we are done.
				Response.StatusCode = (int)HttpStatusCode.NoContent;
				Response.Close();
			}
			catch ( Exception ex )
			{
				Console.WriteLine( "[SERVER] Error handling request!" );
				Console.Write( ex.ToString() );
			}
		}
	}
}
