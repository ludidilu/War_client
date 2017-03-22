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
	public double mapWidth;
	public double mapHeight;
	public double spawnX;
	public double spawnY;
	public double maxRadius;
	public double mapBoundFix;
	public int moveTimes;
	public int commandDelay;
	public int spawnStep;
	public int defaultMoney;
	public int moneyPerStep;

	public double GetTimeStep()
	{
		return timeStep;
	}

	public double GetMapWidth()
	{
		return mapWidth;
	}

	public double GetMapHeight()
	{
		return mapHeight;
	}

	public double GetSpawnX()
	{
		return spawnX;
	}

	public double GetSpawnY()
	{
		return spawnY;
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

	public int GetCommandDelay()
	{
		return commandDelay;
	}

	public int GetSpawnStep()
	{
		return spawnStep;
	}

	public int GetDefaultMoney()
	{
		return defaultMoney;
	}

	public int GetMoneyPerStep()
	{
		return moneyPerStep;
	}
}
