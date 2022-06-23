using System.Text.Json;

using Vector = System.Numerics.Vector2;

public static class Settings
{
	public static string path = Directory.GetCurrentDirectory();
	public static Container? content;

	public static JsonSerializerOptions options = new JsonSerializerOptions()
	{
		ReadCommentHandling = JsonCommentHandling.Skip,
		WriteIndented = true
	};

	public static void Load()
	{
		path = Path.Combine(path, "settings.json");
		if (!File.Exists(path)) // creates file if it doesn't exist
			File.WriteAllText(path, JsonSerializer.Serialize(content = new(), options));

		// load saves
		content = JsonSerializer.Deserialize<Container>(File.ReadAllText(path), options);
	}

	public static Container sets { get => content == null ? new() : content; }

	[Serializable]
	public class Container
	{
		public bool console { get; set; } = false;
		public int width { get; set; } = 800;
		public int heigth { get; set; } = 450;
		public int fps { get; set; } = 60;

		public float mat_x { get; set; } = 250f;
		public float mat_y { get; set; } = 338f;
		public float mat_w { get; set; } = 250f;
		public float mat_h { get; set; } = 110f;

		public PawPoint paw_right { get; set; } = new PawPoint
		{
			x = new int[] { 279, 291, 284, 319, 329 },
			y = new int[] { 162, 121, 127, 192, 182 }
		};
		public PawPoint paw_left { get; set; } = new PawPoint
		{
			x = new int[] { 500, 500, 507, 585, 577 },
			y = new int[] { 150, 233, 243, 218, 208 }
		};

		public KeyPoint[] keys { get; set; } = new KeyPoint[0];
	}

	[Serializable]
	public class PawPoint
	{
		public Vector mid;
		public int[] x { get; set; }
		public int[] y { get; set; }

		public PawPoint Init()
		{
			mid = new Vector(x[2] + x[3], y[2] + y[3]) / 2;
			return this;
		}
	}

	[Serializable]
	public class KeyPoint
	{
		public int key { get; set; }
		public int x { get; set; }
		public int y { get; set; }
	}
}
