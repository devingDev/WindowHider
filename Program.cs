using System;
using System.Runtime.InteropServices;
using System.Text;

class Program
{

	// Imports
	[DllImport("user32.dll")]
	private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

	[DllImport("user32.dll")]
	private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

	delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
	[DllImport("user32.dll")]
	private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	[DllImport("user32.dll")]
	private static extern bool SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);

	[DllImport("user32.dll")]
	private static extern long GetWindowLong(IntPtr hWnd, int nIndex);

	[DllImport("user32.dll")]
	private static extern bool IsWindowVisible(IntPtr hWnd);

	[StructLayout(LayoutKind.Sequential)]
	public struct WINDOWPLACEMENT
	{
		public int length;
		public int flags;
		public uint showCmd;
		public System.Drawing.Point ptMinPosition;
		public System.Drawing.Point ptMaxPosition;
		public System.Drawing.Rectangle rcNormalPosition;
	}

	// Constants
	const int SW_HIDE = 0;
	const int SW_SHOW = 5;
	const int GWL_EXSTYLE = -20;
	const long WS_EX_TOOLWINDOW = 0x00000080L; 
	const long WS_EX_APPWINDOW = 0x00040000L; 




	const string WINDOW_TITLE = "Ghidra:";

	static void Main(string[] args)
	{
		if(EnumWindows((hWnd, lParam) =>
		{
			StringBuilder windowText = new StringBuilder(256);
			GetWindowText(hWnd, windowText, windowText.Capacity);
			string title = windowText.ToString();
			if (title.StartsWith(WINDOW_TITLE))
			{
				Console.WriteLine($"Found window with title: {title}");
				ToggleVisibility(hWnd);
				return false;
			}

			return true;
		}, IntPtr.Zero))
		{
			Console.WriteLine($"Window starting with \"{WINDOW_TITLE}\" not found.");
		}

		


	}

	private static void ToggleVisibility(nint hWnd)
	{
		if (hWnd != IntPtr.Zero)
		{
			WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
			placement.length = Marshal.SizeOf(placement);
			bool isVisible = IsWindowVisible(hWnd);
			Console.WriteLine($"Hiding window: {!isVisible}");
			if (isVisible)
			{
				SetWindow(hWnd, SW_HIDE, WS_EX_TOOLWINDOW);
			}
			else
			{
				SetWindow(hWnd, SW_SHOW, WS_EX_APPWINDOW);
			}

		}
		else
		{
			Console.WriteLine("Window not found.");
		}
	}
	private static void SetWindow(nint hWnd, int SW_STATE = SW_HIDE, long WS_EX_STATE = WS_EX_TOOLWINDOW)
	{
		ShowWindow(hWnd, SW_STATE);
		long style = GetWindowLong(hWnd, GWL_EXSTYLE);
		SetWindowLong(hWnd, GWL_EXSTYLE, style | WS_EX_STATE);
	}

}
