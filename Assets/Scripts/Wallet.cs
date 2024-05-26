using UnityEngine;

public class Wallet : MonoBehaviour
{
    [field:SerializeField] public int TotalMoney {get; set;}

    public void GainMoney(int gains)
    {
        TotalMoney += gains;
        // TODO make an event to update UI as soon as I look up how to do events who can remember these things
    }

    public bool AskToSpend(int cost)
    {
        if(cost > TotalMoney)
        {
            return false;
        }
        else
        {
            SpendMoney(cost);
            return true;
        }
    }

    void SpendMoney(int cost)
    {
        TotalMoney -= cost;
        // TODO Same even as in GainMoney probably!
    }
}
