using System.Runtime.InteropServices;

namespace RichPresenceApp;

public static class ConsoleManager
{
	public static bool IsConsoleVisible = true;

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
			IsConsoleVisible = visible;
		}
	}
}
