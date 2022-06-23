using System.Runtime.InteropServices;
using Raylib_cs;

using static Settings;

public static class Launcher
{
	[DllImport("kernel32.dll")]
	public static extern IntPtr GetConsoleWindow();

	[DllImport("user32.dll")]
	public static extern bool ShowWindow(IntPtr window, int show);

	public static void Main(string[] args)
	{
		Settings.Load();

		Raylib.SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT);

		ShowWindow(GetConsoleWindow(), sets.console ? 5 : 0);
		Raylib.InitWindow(sets.width, sets.heigth, "Bongo Cat");

		Raylib.SetTargetFPS(sets.fps);
		Raylib.SetWindowIcon(Raylib.LoadImage("sprites\\icon small.png"));
		Raylib.SetExitKey(KeyboardKey.KEY_NULL);

		Renderer.Load(); // textures can only be loaded after window initialization
		while (!Raylib.WindowShouldClose()) Renderer.Draw();

		Raylib.CloseWindow();
	}
}