
public class AdventurerSkill : BaseSkill
{
    protected override void Battle_OnBattleStarted()
    {
        UseSkill();
    }

    protected override void UseSkill()
    {
        _unit.GainDodge();
    }
}
