using Discord;

namespace CSGODiscordRP;

public class DiscordManager
{
	public static Discord.Discord discord = new( 872181511334543370, (ulong)CreateFlags.NoRequireDiscord );

	public static void Start()
	{
		Action action = () => discord.RunCallbacks();
		Task task = Task.Run( action );
	}

	public static void UpdateDiscordActivity()
	{
		var activity = new Activity
		{
			State = "Test",
			Details = "Hello world!",
		};

		var activityManager = discord.GetActivityManager();

		activityManager.UpdateActivity( activity, ( result ) =>
		{
			if ( result == Result.Ok )
			{
				Console.WriteLine( "Success!" );
			}
			else
			{
				Console.WriteLine( "Failed" );
			}
		} );
	}
}
