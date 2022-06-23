using System.Runtime.InteropServices;
using Raylib_cs;

using static Settings;

using Point = System.Drawing.Point;
using Vector = System.Numerics.Vector2;

public static class Renderer
{
	public static float width;
	public static float height;

	public static Drawable background;
	public static Drawable mouse;

	public static Paw right;
	public static Paw left;

	public static float mat_sin;
	public static float mat_cos;

	[DllImport("user32.dll")]
	public static extern bool GetCursorPos(out Point point);

	[DllImport("User32.dll")]
	private static extern short GetAsyncKeyState(int key);

	public static void Load()
	{
		width = Raylib.GetMonitorWidth(Raylib.GetCurrentMonitor());
		height = Raylib.GetMonitorHeight(Raylib.GetCurrentMonitor());

		background = new Drawable("background");
		mouse = new Drawable("mouse");

		right = new Paw("paw right", sets.paw_right);
		left = new Paw("paw left", sets.paw_left);

		(mat_sin, mat_cos) = MathF.SinCos(17f * MathF.PI / 180f);
	}

	public static void Draw()
	{
		GetCursorPos(out Point pos); // get current position
		(float x, float y) = (pos.X / width, pos.Y / height);
		Spline.fix = (1f - y + x) / 2f + .6f; // in the lower left corner of the mat, the hand is broken
		(x, y) = (x * mat_cos - y * mat_sin, x * mat_sin + y * mat_cos);

		mouse.x = (int)(sets.mat_x - x * sets.mat_w);
		mouse.y = (int)(sets.mat_y - y * sets.mat_h);
		right.setTarget(mouse.x + 50, mouse.y + 10);
		Spline.fix = 1f; // return default value

		left.raised = true;
		foreach (KeyPoint key in sets.keys)
			if ((GetAsyncKeyState(key.key) >> 15) == -1)
			{
				left.setTarget(key.x, key.y);
				left.raised = false;
			}

		Raylib.BeginDrawing();

		background.Draw();
		mouse.Draw();

		right.Draw();
		left.Draw();

		Raylib.EndDrawing();
	}

	public class Drawable
	{
		public int x;
		public int y;

		public Texture2D texture;

		public Drawable(String name)
		{
			texture = Raylib.LoadTexture("sprites\\" + name + ".png");
		}

		public virtual void Draw()
		{
			Raylib.DrawTexture(texture, x, y, Color.WHITE);
		}
	}

	public class Paw : Drawable
	{
		public const int divs = 40;
		public Vector[] spline = new Vector[divs];

		public PawPoint pos;
		public bool raised;

		public Paw(String name, PawPoint pos) : base(name)
		{
			(x, y) = (pos.x[0], pos.y[0]);
			this.pos = pos.Init();
		}

		public override void Draw()
		{
			if (raised) base.Draw();
			else
			{
				Vector[] part = new Vector[10];
				for (int i = 0; i < 20; i++)
				{
					Array.Copy(spline, i, part, 0, 2);
					Array.Copy(spline, divs - 2 - i, part, 2, 2);
					Raylib.DrawTriangleFan(part, 4, Color.WHITE);
				}

				for (int i = 1; i < divs; i++) Raylib.DrawLineEx(spline[i - 1], spline[i], 6f, Color.BLACK);

				Raylib.DrawCircle(pos.x[1], pos.y[1], 3f, Color.BLACK);
				Raylib.DrawCircle(pos.x[4], pos.y[4], 3f, Color.BLACK);
			}
		}

		public void setTarget(int tx, int ty)
		{
			Vector dif = Vector.Normalize(pos.mid - new Vector(tx, ty));
			Vector p1 = new Vector(-dif.Y, dif.X) * 18;
			Vector p2 = new Vector(-dif.X, -dif.Y) * 5;

			(double[] xs, double[] ys) = Spline.InterpolateXY(new double[] { pos.x[1], pos.x[2], tx - p1.X, tx + p2.X, tx + p1.X, pos.x[3], pos.x[4] },
															  new double[] { pos.y[1], pos.y[2], ty - p1.Y, ty + p2.Y, ty + p1.Y, pos.y[3], pos.y[4] }, divs);

			for (int i = 0; i < divs; i++) spline[i] = new Vector((float)xs[i], (float)ys[i]);
		}
	}
}