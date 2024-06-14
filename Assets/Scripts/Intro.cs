using UnityEngine;

public class Intro : MonoBehaviour
{
    [SerializeField] Canvas _titleCanvas;
    [SerializeField] Animator _animator;
    [SerializeField] Campaign _campaign;

    bool _tutorialStarted;

    static readonly int INTRO_Hash = Animator.StringToHash("Intro");

    void Update()
    {
        if(_tutorialStarted) { return; }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            _tutorialStarted = true;
            _campaign.GoToTutorial();
        }
    }

    public void BeginButton()
    {
        _titleCanvas.enabled = false;
        _animator.SetTrigger(INTRO_Hash);
    }

    public void TutorialButton()
    {
        if(_tutorialStarted) { return; }

        _tutorialStarted = true;
        _campaign.GoToTutorial();
    }
}
