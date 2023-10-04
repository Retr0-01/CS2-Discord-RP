using System.Runtime.InteropServices;

namespace RichPresenceApp;

public static class Program
{
	[STAThread]
	static void Main()
	{
		Application.SetCompatibleTextRenderingDefault( false );
		Application.Run( new RichPresenceAppContext() );
	}

	[DllImport( "kernel32.dll", SetLastError = true )][return: MarshalAs( UnmanagedType.Bool )] public static extern bool AllocConsole();
	[DllImport( "kernel32.dll" )] public static extern bool FreeConsole();
	[DllImport( "kernel32.dll" )] static extern IntPtr GetConsoleWindow();
	[DllImport( "user32.dll" )] static extern bool ShowWindow( IntPtr hWnd, int nCmdShow );

	// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow#parameters
	const int SW_HIDE = 0;
	const int SW_SHOW = 5;

	public static void SetConsoleWindowVisibility( bool visible )
	{
		IntPtr hWnd = GetConsoleWindow();
		if ( hWnd != IntPtr.Zero )
		{
			if ( visible ) ShowWindow( hWnd, SW_SHOW );
			else ShowWindow( hWnd, SW_HIDE );
		}
	}
}

public class RichPresenceAppContext : ApplicationContext
{
	public RichPresenceAppContext()
	{
		Program.AllocConsole();
		Console.Title = "Counter-Strike 2 Discord Rich Presence";
		Console.WriteLine( "=============== CS2 DISCORD RICH PRESENCE ===============" );
		Console.WriteLine( "Create by Giannis \"Retr0\" Kepas with love :>" );
		Console.WriteLine( "Source: https://github.com/Retr0-01/CS2-Discord-RP" );
		Console.WriteLine();
		Program.SetConsoleWindowVisibility( false );

		SystemTray.Setup();
		ApplicationSetup.Configure();
		DiscordManager.Initialize();
		// Main application loop. Code after this point will be run only on shutdown.
		HttpServer.Start();
	}

	void Exit( object sender, EventArgs e )
	{
		SystemTray.NotifyIcon.Visible = false;
		Program.FreeConsole();
		DiscordManager.Client.Dispose();
		Application.Exit();
	}
}
