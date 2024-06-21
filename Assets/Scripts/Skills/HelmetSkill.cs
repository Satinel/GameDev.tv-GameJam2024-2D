
public class HelmetSkill : BaseSkill
{
    protected override void Battle_OnBattleStarted()
    {
        UseSkill();
    }

    protected override void UseSkill()
    {
        _unit.ChangeHeat(_heatGenerated);
    }
}
