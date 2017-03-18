public class UnitSDS : CsvBase, IUnitSDS
{

	public double moveSpeed;
	public double radius;
	public int weight;
	public double queuePos;
	public int attackDamage;
	public double attackRange;
	public double attackStep;
	public int hp;
	public double visionRange;
	public bool isAirUnit;
	public bool canAttackAirUnit;
	public bool canAttackGroundUnit;
	public int attackType;
	public double attackTypeData;

	public double GetMoveSpeed()
	{

		return moveSpeed;
	}

	public double GetRadius()
	{

		return radius;
	}

	public int GetWeight()
	{

		return weight;
	}

	public double GetQueuePos()
	{

		return queuePos;
	}

	public int GetAttackDamage()
	{
		return attackDamage;
	}

	public double GetAttackRange()
	{
		return attackRange;
	}

	public double GetAttackStep()
	{
		return attackStep;
	}

	public int GetHp()
	{
		return hp;
	}

	public double GetVisionRange()
	{
		return visionRange;
	}

	public bool GetIsAirUnit()
	{
		return isAirUnit;
	}

	public bool GetCanAttackAirUnit()
	{
		return canAttackAirUnit;
	}

	public bool GetCanAttackGroundUnit()
	{
		return canAttackGroundUnit;
	}

	public UnitAttackType GetAttackType()
	{
		return (UnitAttackType)attackType;
	}

	public double GetAttackTypeData()
	{
		return attackTypeData;
	}
}
