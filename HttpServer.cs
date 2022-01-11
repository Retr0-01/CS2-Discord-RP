using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace CSGODiscordRP;

public class HttpServer
{
	public static HttpListener Listener = new();
	public static readonly string Url = "http://localhost:3000/";

	public static void Start()
	{
		Listener.Prefixes.Add( Url );
		Listener.Start();
		Console.WriteLine( $"Listening for data on {Url}" );

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
			string RawData;
			string JSONData = "{ 'info': 'No Data' }";

			HttpListenerContext Context = await Listener.GetContextAsync();
			HttpListenerRequest Request = Context.Request;
			HttpListenerResponse Response = Context.Response;

			if ( Request.HttpMethod == "POST" )
			{
				Console.WriteLine( "Incoming data..." );

				using ( var Reader = new StreamReader( Request.InputStream, Request.ContentEncoding ) )
				{
					RawData = Reader.ReadToEnd();
				}

				JSONData = JsonConvert.SerializeObject( RawData );

				Console.WriteLine( JSONData );
			}
			// Enable manual shutdown through a browser.
			else if ( Request.HttpMethod == "GET" && Request.Url.AbsolutePath == "/shutdown" )
			{
				Console.WriteLine( "Shutdown requested." );
				IsRunning = false;
			}

			byte[] Data = Encoding.UTF8.GetBytes( JSONData );

			Response.ContentType = "application/json";
			Response.ContentEncoding = Encoding.UTF8;
			Response.ContentLength64 = Data.LongLength;

			// Write out the data.
			await Response.OutputStream.WriteAsync( Data );
			DiscordManager.UpdateDiscordActivity();
			Response.Close();
		}
	}
}
