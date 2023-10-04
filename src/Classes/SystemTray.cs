namespace RichPresenceApp;

public class SystemTray
{
	public static NotifyIcon NotifyIcon;
	static ContextMenuStrip ContextMenu;

	public static void Setup()
	{
		Thread trayThread = new( delegate ()
		{
			Icon csIcon = Icon.ExtractAssociatedIcon( Application.ExecutablePath );
			NotifyIcon = new();
			NotifyIcon.Icon = csIcon;
			NotifyIcon.Text = "CS2 Discord Rich Presence";

			ContextMenu = new();
			ToolStripMenuItem statusMenuItem = new( "Status: Active", SystemIcons.Information.ToBitmap(), Click_Handler );
			statusMenuItem.Enabled = false;
			ToolStripMenuItem openConsoleMenuItem = new( "Open Console", csIcon.ToBitmap(), Click_Handler );
			ToolStripMenuItem exitMenuItem = new( "Quit App", SystemIcons.Hand.ToBitmap(), Click_Handler );
			ContextMenu.Items.AddRange( new ToolStripItem[] { statusMenuItem, openConsoleMenuItem, exitMenuItem } );
			NotifyIcon.ContextMenuStrip = ContextMenu;

			NotifyIcon.BalloonTipIcon = ToolTipIcon.Info;
			NotifyIcon.BalloonTipTitle = "No Distractions ;)";
			NotifyIcon.BalloonTipText = "The CS2 Discord Rich Presence application will run in the background.";

			NotifyIcon.Visible = true;
			NotifyIcon.ShowBalloonTip( 10000 );

			Application.Run();
		} );

		trayThread.Name = "System Tray Thread";
		trayThread.IsBackground = true;
		trayThread.SetApartmentState( ApartmentState.STA );
		trayThread.Start();
	}

	private static void Click_Handler( object sender, EventArgs e )
	{
		ToolStripItem item = (ToolStripItem)sender;

		if ( item.Text == "Open Console" )
			Program.SetConsoleWindowVisibility( true );
		else if ( item.Text == "Quit App" )
			Environment.Exit( Environment.ExitCode );
	}
}
