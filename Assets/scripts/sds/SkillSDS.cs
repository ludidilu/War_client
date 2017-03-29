public class SkillSDS : CsvBase, ISkillSDS
{
    public double moveSpeed;
    public double radius;
    public double range;
    public int skillType;
    public int time;

    public double GetMoveSpeed()
    {
        return moveSpeed;
    }

    public double GetRadius()
    {
        return radius;
    }

    public double GetRange()
    {
        return range;
    }

    public SkillType GetSkillType()
    {
        return (SkillType)skillType;
    }

    public int GetTime()
    {
        return time;
    }
}
