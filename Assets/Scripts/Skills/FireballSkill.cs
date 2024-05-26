using UnityEngine;

public class FireballSkill : BaseSkill
{
    protected override void UseSkill()
    {
        base.UseSkill();
        Debug.Log("FIREBALL!");
    }
}
