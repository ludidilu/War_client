public class SkillSDS : CsvBase, ISkillSDS
{
	public int skillType;
	public int time;
	public double moveSpeed;
	public double range;
	public double obstacleRadius;
	public int effectTargetType;
	public int effectTarget;
	public double effectRadius;
	public int effect;
	public int[] effectData;

	public SkillType GetSkillType()
	{
		return (SkillType)skillType;
	}

	public int GetTime()
	{
		return time;
	}

	public double GetMoveSpeed()
	{
		return moveSpeed;
	}

	public double GetRange()
	{
		return range;
	}

	public double GetObstacleRadius()
	{
		return obstacleRadius;
	}

	public UnitTargetType GetEffectTargetType()
	{
		return (UnitTargetType)effectTargetType;
	}

	public SkillEffectTarget GetEffectTarget()
	{
		return (SkillEffectTarget)effectTarget;
	}

	public double GetEffectRadius()
	{
		return effectRadius;
	}

	public SkillEffect GetEffect()
	{
		return (SkillEffect)effect;
	}

	public int[] GetEffectData()
	{
		return effectData;
	}
}
