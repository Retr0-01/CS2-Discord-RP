using System.Runtime.InteropServices;

namespace RichPresenceApp;

public static class ConsoleManager
{
	public static bool IsConsoleVisible = true;

	// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-deletemenu
	private const int MF_BYCOMMAND = 0x00000000;
	private const int SC_CLOSE = 0xF060;
	private const int SC_MINIMIZE = 0xF020;
	private const int SC_MAXIMIZE = 0xF030;

	// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow#parameters
	private const int SW_HIDE = 0;
	private const int SW_SHOW = 5;

	[DllImport( "kernel32.dll", SetLastError = true )][return: MarshalAs( UnmanagedType.Bool )] private static extern bool AllocConsole();
	[DllImport( "kernel32.dll" )] public static extern bool FreeConsole();
	[DllImport( "kernel32.dll" )] private static extern IntPtr GetConsoleWindow();
	[DllImport( "user32.dll" )] private static extern bool ShowWindow( IntPtr hWnd, int nCmdShow );
	[DllImport( "user32.dll" )] private static extern int DeleteMenu( IntPtr hMenu, int nPosition, int wFlags );
	[DllImport( "user32.dll" )] private static extern IntPtr GetSystemMenu( IntPtr hWnd, bool bRevert );

	public static void CreateConsole()
	{
		AllocConsole();
		_ = DeleteMenu( GetSystemMenu( GetConsoleWindow(), false ), SC_CLOSE, MF_BYCOMMAND );
		_ = DeleteMenu( GetSystemMenu( GetConsoleWindow(), false ), SC_MINIMIZE, MF_BYCOMMAND );
		_ = DeleteMenu( GetSystemMenu( GetConsoleWindow(), false ), SC_MAXIMIZE, MF_BYCOMMAND );
	}

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
