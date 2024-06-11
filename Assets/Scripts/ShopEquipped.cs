using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopEquipped : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _mItemName, _mCooldownText, _mAttackText, _mHealthText, _mPriceText;
    [SerializeField] TextMeshProUGUI _oItemName, _oCooldownText, _oAttackText, _oHealthText, _oPriceText;
    [SerializeField] TextMeshProUGUI _hItemName, _hCooldownText, _hAttackText, _hHealthText, _hPriceText;
    [SerializeField] Image _mImageRenderer, _mPriceImage;
    [SerializeField] Image _oImageRenderer, _oPriceImage;
    [SerializeField] Image _hImageRenderer, _hPriceImage;
    [SerializeField] GameObject _mUpgradeIndicator, _oUpgradeIndicator, _hUpgradeIndicator;

    Unit _unit;

    public void EquippedSetup(Unit selectedUnit)
    {
        _unit = selectedUnit;

        if(_unit.Main().Gear)
        {
            _mItemName.text = _unit.Main().UpgradeName;

            if(_unit.Main().Gear.Skill.Cooldown > 0)
            {
                _mCooldownText.text = _unit.Main().Gear.Skill.Cooldown.ToString();
            }
            else
            {
                _mCooldownText.text = "N/A";
            }
            _mAttackText.text = (_unit.Main().Gear.AttackIncrease * _unit.Main().UpgradeLevel).ToString();
            _mHealthText.text = (_unit.Main().Gear.HealthIncrease * _unit.Main().UpgradeLevel).ToString();
            _mPriceImage.enabled = true;
            _mPriceText.text = $"{_unit.Main().Gear.Price * _unit.Main().UpgradeLevel / 3}";
            _mImageRenderer.enabled = true;
            _mImageRenderer.sprite = _unit.Main().Gear.Sprite;
        }
        else
        {
            _mItemName.text = "Nothing";
            _mCooldownText.text = "N/A";
            _mAttackText.text = "0";
            _mHealthText.text = "0";
            _mPriceImage.enabled = false;
            _mPriceText.text = "0";
            _mImageRenderer.enabled = false;
        }

        if(_unit.Offhand().Gear)
        {
            _oItemName.text = _unit.Offhand().UpgradeName;

            if(_unit.Offhand().Gear.Skill.Cooldown > 0)
            {
                _oCooldownText.text = _unit.Offhand().Gear.Skill.Cooldown.ToString();
            }
            else
            {
                _oCooldownText.text = "N/A";
            }
            _oAttackText.text = (_unit.Offhand().Gear.AttackIncrease * _unit.Offhand().UpgradeLevel).ToString();
            _oHealthText.text = (_unit.Offhand().Gear.HealthIncrease * _unit.Offhand().UpgradeLevel).ToString();
            _oPriceImage.enabled = true;
            _oPriceText.text = $"{_unit.Offhand().Gear.Price * _unit.Offhand().UpgradeLevel / 3}";
            _oImageRenderer.enabled = true;
            _oImageRenderer.sprite = _unit.Offhand().Gear.Sprite;
        }
        else
        {
            _oItemName.text = "Nothing";
            _oCooldownText.text = "N/A";
            _oAttackText.text = "0";
            _oHealthText.text = "0";
            _oPriceImage.enabled = false;
            _oPriceText.text = "0";
            _oImageRenderer.enabled = false;
        }
        
        if(_unit.Headgear().Gear)
        {
            _hItemName.text = _unit.Headgear().UpgradeName;

            if(_unit.Headgear().Gear.Skill.Cooldown > 0)
            {
                _hCooldownText.text = _unit.Headgear().Gear.Skill.Cooldown.ToString();
            }
            else
            {
                _hCooldownText.text = "N/A";
            }
            _hAttackText.text = (_unit.Headgear().Gear.AttackIncrease * _unit.Headgear().UpgradeLevel).ToString();
            _hHealthText.text = (_unit.Headgear().Gear.HealthIncrease * _unit.Headgear().UpgradeLevel).ToString();
            _hPriceImage.enabled = true;
            _hPriceText.text = $"{_unit.Headgear().Gear.Price * _unit.Headgear().UpgradeLevel / 3}";
            _hImageRenderer.enabled = true;
            _hImageRenderer.sprite = _unit.Headgear().Gear.Sprite;
        }
        else
        {
            _hItemName.text = "Nothing";
            _hCooldownText.text = "N/A";
            _hAttackText.text = "0";
            _hHealthText.text = "0";
            _hPriceImage.enabled = false;
            _hPriceText.text = "0";
            _hImageRenderer.enabled = false;
        }
    }

    public void MainIndicateUpgrade(bool isUpgradeable)
    {
        _mUpgradeIndicator.SetActive(isUpgradeable);
        if(isUpgradeable)
        {
            _mPriceText.text = string.Empty;
            _mPriceImage.enabled = false;
        }
        else
        {
            if(_unit.Main().Gear)
            {
                _mPriceText.text = $"{_unit.Main().Gear.Price * _unit.Main().UpgradeLevel / 3}";
            }
            else
            {
                _mPriceText.text = "0";
            }
            _mPriceImage.enabled = true;
        }
    }

    public void OffIndicateUpgrade(bool isUpgradeable)
    {
        _oUpgradeIndicator.SetActive(isUpgradeable);
        if(isUpgradeable)
        {
            _oPriceText.text = string.Empty;
            _oPriceImage.enabled = false;
        }
        else
        {
            if(_unit.Offhand().Gear)
            {
                _oPriceText.text = $"{_unit.Offhand().Gear.Price * _unit.Offhand().UpgradeLevel / 3}";
            }
            else
            {
                _oPriceText.text = "0";
            }
            _oPriceImage.enabled = true;
        }
    }

    public void HeadIndicateUpgrade(bool isUpgradeable)
    {
        _hUpgradeIndicator.SetActive(isUpgradeable);
        if(isUpgradeable)
        {
            _hPriceText.text = string.Empty;
            _hPriceImage.enabled = false;
        }
        else
        {
            if(_unit.Headgear().Gear)
            {
                _hPriceText.text = $"{_unit.Headgear().Gear.Price * _unit.Headgear().UpgradeLevel / 3}";
            }
            else
            {
                _hPriceText.text = "0";
            }
            _hPriceImage.enabled = true;
        }
    }
}
