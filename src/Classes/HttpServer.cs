using GameState;
using System.Net;

namespace RichPresenceApp;

public class HttpServer
{
    private static readonly HttpListener Listener = new();
    private const string Url = "http://localhost:3000/";

    public static void StartServer()
    {
        Listener.Prefixes.Add( Url );
        Listener.Start();
        Console.WriteLine( $"[SERVER] Listening for data on {Url}" );

        // Handle requests.
        Task ListenTask = HandleRequests();
        ListenTask.GetAwaiter().GetResult();

        Listener.Close();
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
                Console.WriteLine( "[SERVER] Error handling request: " + ex.Message );
                Console.Write( ex.ToString() );
            }
        }
    }
}
