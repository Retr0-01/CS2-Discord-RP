using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace RichPresenceApp;

public class ApplicationSetup
{
	private const string configFileName = "gamestate_integration_discord-rp.cfg";

	public static void Configure()
	{
		string gamePath = GetGameDir();
		if ( gamePath == null )
			Console.WriteLine( "[SETUP] CS2 installation path not found! Aborting..." );
		else
		{
			Console.WriteLine( $"[SETUP] Found CS2 installation path...\n|--- {gamePath}" );

			string configFile = Path.Combine( gamePath, "game", "csgo", "cfg", configFileName );
			// Delete the file if it already exists and make a clean one.
			if ( File.Exists( configFile ) )
			{
				File.Delete( configFile );
				Console.WriteLine( "[SETUP] Deleted existing config file." );
			}

			Console.WriteLine( $"[SETUP] Writing \"{configFileName}\" config file...\n|--- {configFile}" );
			using ( StreamWriter sw = File.CreateText( configFile ) )
			{
				sw.WriteLine( "\"CS2-Discord-RP\"" );
				sw.WriteLine( "{" );
				sw.WriteLine( "	\"uri\"				\"http://localhost:3000\"" );
				sw.WriteLine( "	\"timeout\"			\"5.0\"" );
				sw.WriteLine( "	\"buffer\"			\"1\"" );
				sw.WriteLine( "	\"throttle\"		\"5\"" );
				sw.WriteLine( "	\"heartbeat\"		\"15\"" );
				sw.WriteLine( "	\"data\"" );
				sw.WriteLine( "	{" );
				sw.WriteLine( "		\"provider\"				\"1\"" );
				sw.WriteLine( "		\"map\"						\"1\"" );
				sw.WriteLine( "		\"player_id\"				\"1\"" );
				sw.WriteLine( "		\"player_match_stats\"		\"1\"" );
				sw.WriteLine( "		\"player_state\"			\"1\"" );
				sw.WriteLine( "	}" );
				sw.WriteLine( "}" );
			}
			Console.WriteLine( "[SETUP] Done!" );
		}
	}

	/// <summary>
	/// Returns the location of the CS:GO installation, or null if it's unable to find it. 
	/// </summary>
	/// <returns></returns>
	private static string GetGameDir()
	{
		// Credit to moritzuehling for this code snippet
		// https://gist.github.com/moritzuehling/7f1c512871e193c0222f

		string steamPath = (string)Registry.GetValue( "HKEY_CURRENT_USER\\Software\\Valve\\Steam", "SteamPath", "" );
		string pathsFile = Path.Combine( steamPath, "steamapps", "libraryfolders.vdf" );

		if ( !File.Exists( pathsFile ) )
			return null;

		List<string> libraries = new()
		{
			Path.Combine( steamPath )
		};

		var pathVDF = File.ReadAllLines( pathsFile );

		Regex pathRegex = new( @"\""(([^\""]*):\\([^\""]*))\""" );
		foreach ( var line in pathVDF )
		{
			if ( pathRegex.IsMatch( line ) )
			{
				string match = pathRegex.Matches( line )[0].Groups[1].Value;

				// De-Escape vdf. 
				libraries.Add( match.Replace( "\\\\", "\\" ) );
			}
		}

		foreach ( var library in libraries )
		{
			string csGamePath = Path.Combine( library, "steamapps\\common\\Counter-Strike Global Offensive" );
			if ( Directory.Exists( csGamePath ) )
			{
				return csGamePath;
			}
		}

		return null;
	}
}
