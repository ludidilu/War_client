public class GameConfig : Config {

	private static GameConfig _Instance;

	public static GameConfig Instance{

		get{

			if (_Instance == null) {

				_Instance = new GameConfig ();
			}

			return _Instance;
		}
	}

	public double mapX;
	public double mapY;
	public double mapWidth;
	public double mapHeight;
	public double timeStep;
	public double maxRadius;
	public double mapBoundFix;
}
