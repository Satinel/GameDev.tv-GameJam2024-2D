
public class TopHatSkill : BaseSkill
{
    protected override void Battle_OnBattleStarted()
    {
        UseSkill();
    }

    protected override void UseSkill()
    {
        Wallet wallet = FindFirstObjectByType<Wallet>();

        wallet.SetBonusMoney(0.1f);
    }
}
