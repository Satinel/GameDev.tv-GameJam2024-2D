
public class WitchHatSkill : BaseSkill
{
    protected override void Battle_OnBattleStarted()
    {
        UseSkill();
    }

    protected override void UseSkill()
    {
        Campaign campaign = FindFirstObjectByType<Campaign>();

        campaign.SetHasWitchHat();
    }
}
