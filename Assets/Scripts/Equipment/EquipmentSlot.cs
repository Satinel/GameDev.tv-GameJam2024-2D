using System;
using UnityEngine;

public class EquipmentSlot : MonoBehaviour
{
    public static event Action OnLegendaryUpgrade;
    public static event Action OnTophatEquipped;

    [field:SerializeField] public EquipmentScriptableObject Gear {get; private set;}
    public int UpgradeLevel {get; private set;} = 1;
    public string UpgradeName {get; private set;}
    public BaseSkill Skill {get; private set;}

    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] GameObject _cooldownUI, _cooldownFillImage;

    public Sprite ItemSprite() => _spriteRenderer.sprite;

    const string _tophatName = "Top Hat";

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
        UpgradeName = gear.Name;
        _spriteRenderer.sprite = gear.Sprite;
        if(gear.HasOffset)
        {
            _spriteRenderer.transform.localPosition = gear.SpriteOffset;
        }
        _spriteRenderer.transform.localScale = gear.SpriteScale;
        _spriteRenderer.flipX = gear.SpriteFlipped;
        
        if(GetComponentInChildren<BaseSkill>(true))
        {
            Destroy(GetComponentInChildren<BaseSkill>(true).gameObject);
        }
        Skill = Instantiate(gear.Skill, transform);
        Skill.SetUI(_cooldownUI, _cooldownFillImage);

        if(gear.Slot == EquipmentType.Headgear && gear.Name == _tophatName)
        {
            OnTophatEquipped?.Invoke();
        }
    }

    public void UpgradeItem(EquipmentScriptableObject gear)
    {
        UpgradeLevel++;

        switch(UpgradeLevel)
        {
            case 2:
                UpgradeName = $"{gear.Name} +1";
                break;
            case 3:
                UpgradeName = $"{gear.Name} +2";
                break;
            case 4:
                UpgradeName = $"<color=#00FF00>Rare</color>\n {gear.Name}";
                _spriteRenderer.sprite = gear.RareSprite;
                break;
            case 5:
                UpgradeName = $"<color=#00FF00>Rare</color>\n {gear.Name} +1";
                break;
            case 6:
                UpgradeName = $"<color=#00FF00>Rare</color>\n {gear.Name} +2";
                break;
            case 7:
                UpgradeName = $"<color=#FF00DB>Epic</color>\n {gear.Name}";
                _spriteRenderer.sprite = gear.EpicSprite;
                break;
            case 8:
                UpgradeName = $"<color=#FF00DB>Epic</color>\n {gear.Name} +1";
                _spriteRenderer.sprite = gear.EpicSprite;
                break;
            case 9:
                UpgradeName = $"<color=#FF00DB>Epic</color>\n {gear.Name} +2";
                _spriteRenderer.sprite = gear.EpicSprite;
                break;
            case 10:
                UpgradeName = $"<color=#FF6500>Legendary</color>\n {gear.Name}";
                _spriteRenderer.sprite = gear.LegendarySprite;
                if(gear.Slot != EquipmentType.Headgear)
                {
                    OnLegendaryUpgrade?.Invoke();
                }
                break;
            case >10:
                UpgradeName = $"<color=#FF6500>Legendary</color>\n {gear.Name} +{UpgradeLevel - 10}";
                _spriteRenderer.sprite = gear.LegendarySprite;
                break;    
            default:
            break;
        }
    }

    public void SetUnarmedSkill(BaseSkill skill)
    {
        Skill = skill;
        Skill.SetUI(_cooldownUI, _cooldownFillImage);
    }
}
