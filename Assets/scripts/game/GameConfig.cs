class GameConfig : Config, IGameConfig
{
	private static GameConfig _Instance;

	public static GameConfig Instance
	{
		get
		{
			if (_Instance == null)
			{
				_Instance = new GameConfig();
			}

			return _Instance;
		}
	}

	public double timeStep;
	public double mapX;
	public double mapY;
	public double mapWidth;
	public double mapHeight;
	public double maxRadius;
	public double mapBoundFix;
	public int moveTimes;

	public double GetTimeStep()
	{
		return timeStep;
	}

	public double GetMapX()
	{
		return mapX;
	}

	public double GetMapY()
	{
		return mapY;
	}

	public double GetMapWidth()
	{
		return mapWidth;
	}

	public double GetMapHeight()
	{
		return mapHeight;
	}

	public double GetMaxRadius()
	{
		return maxRadius;
	}

	public double GetMapBoundFix()
	{
		return mapBoundFix;
	}

	public int GetMoveTimes()
	{
		return moveTimes;
	}
}
