using UnityEngine;
using TMPro;

public class Intro : MonoBehaviour
{
    [SerializeField] Canvas _titleCanvas, _nameHeroCanvas;
    [SerializeField] Animator _animator;
    [SerializeField] Campaign _campaign;
    [SerializeField] GameObject _nameConfirm;
    [SerializeField] string _nameInputText = string.Empty;
    [SerializeField] TextMeshProUGUI _confirmNamePromptText;

    bool _introStarted;

    static readonly int INTRO_Hash = Animator.StringToHash("Intro");
    static readonly int SKIPINTRO_Hash = Animator.StringToHash("SkipIntro");

    void Start()
    {
        Time.timeScale = 1;
    }

    void Update()
    {
        if(!_introStarted) { return; }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            SkipIntro();
        }
    }

    public void SkipIntro()
    {
        _animator.SetTrigger(SKIPINTRO_Hash);
    }

    public void BeginButton()
    {
        _titleCanvas.enabled = false;
        _animator.SetTrigger(INTRO_Hash);
        _introStarted = true;

    }

    public void TutorialButton()
    {
        _campaign.GoToTutorial();
    }

    public void NameHeroButton()
    {
        _nameHeroCanvas.enabled = true;
    }

    public void CancelName()
    {
        _nameConfirm.SetActive(false);
        _nameHeroCanvas.enabled = false;
    }

    public void ConfirmNamePrompt(string name)
    {
        _nameInputText = name;
        _nameConfirm.SetActive(true);
        _confirmNamePromptText.text = $"Name the Hero {name}?";
    }

    public void AcceptName(Unit unit)
    {
        if(_nameInputText == string.Empty)
        {
            _nameInputText = "Peanut";
        }
        unit.SetHeroName(_nameInputText);
        _nameConfirm.SetActive(false);
        _nameHeroCanvas.enabled = false;
    }

    public void RejctName()
    {
        _nameConfirm.SetActive(false);
    }

    public void TextInputButton()
    {
        ConfirmNamePrompt(_nameInputText);
    }
    
    public void OnValueChanged(string value)
    {
        _nameInputText = value;
    }
}
