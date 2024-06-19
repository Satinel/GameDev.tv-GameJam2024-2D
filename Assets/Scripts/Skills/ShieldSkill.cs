using UnityEngine;

public class ShieldSkill : BaseSkill
{
    [SerializeField] int _heatGenerated; // TODO? Move this to BaseSkill and have all skills generate (possibly negative) Heat on use

    protected override void UseSkill()
    {
        _unitAnimator.SetTrigger(SHIELD_HASH);        
    }

    public override void SkillEffect()
    {
        base.SkillEffect();
        if(VisualEffect)
        {
            Instantiate(VisualEffect, _unit.transform);
        }
        _unit.ChangeHeat(_heatGenerated);
        _unit.GainHealth(_unit.Offhand().Gear.HealthIncrease);
    }
}
