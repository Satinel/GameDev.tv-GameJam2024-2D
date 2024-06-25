using UnityEngine;

public class MinionHeart : Minion
{
    public override void MinionAction()
    {
        throw new System.NotImplementedException(); // TODO Minion Heals Boss or Gives a shield based on this minion's current health
    }

    public override void TakeDamage(int damage)
    {
        throw new System.NotImplementedException(); // TODO Minion takes damage + deals damage to Boss(?)/Boss's Shield(??) Also this one can die?!
    }
}
