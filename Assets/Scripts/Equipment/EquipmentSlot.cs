using UnityEngine;

public class EquipmentSlot : MonoBehaviour
{
    [field:SerializeField] public EquipmentScriptableObject Gear {get; private set;}
    public int UpgradeLevel {get; private set;} = 1;
    public BaseSkill Skill {get; private set;}

    [SerializeField] SpriteRenderer _spriteRenderer;



    void Awake()
    {
        if(!_spriteRenderer)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void EquipItem(EquipmentScriptableObject gear)
    {
        UpgradeLevel = 1; // Resets for new items
        Gear = gear;
        _spriteRenderer.sprite = gear.Sprite;
        if(gear.HasOffset)
        {
            _spriteRenderer.transform.localPosition = gear.SpriteOffset;
        }
        _spriteRenderer.transform.localScale = gear.SpriteScale;
        _spriteRenderer.flipX = gear.SpriteFlipped;
        
        if(GetComponentInChildren<BaseSkill>())
        {
            Destroy(GetComponentInChildren<BaseSkill>().gameObject);
        }
        Skill = Instantiate(gear.Skill, transform);
    }

    public void UpgradeItem(EquipmentScriptableObject gear)
    {
        // TODO Figure out this whole upgrade gear system idea thing...
        UpgradeLevel++;

        switch(UpgradeLevel)
        {
            case 3:
                _spriteRenderer.sprite = gear.RareSprite;
                break;
            case 6:
                _spriteRenderer.sprite = gear.EpicSprite;
                break;
            case 9:
                _spriteRenderer.sprite = gear.LegendarySprite;
                break;
            default:
            break;
        }
        Debug.Log("Upgraded " + gear.Name);
    }

    public void SetSkill()
    {
        Skill = GetComponentInChildren<BaseSkill>();
    }
}
