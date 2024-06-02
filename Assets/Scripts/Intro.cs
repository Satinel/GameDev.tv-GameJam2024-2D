using UnityEngine;

public class Intro : MonoBehaviour
{
    [SerializeField] Canvas _titleCanvas;

    [SerializeField] Animator _animator;

    static readonly int INTRO_Hash = Animator.StringToHash("Intro");

    public void BeginButton()
    {
        _titleCanvas.enabled = false;
        _animator.SetTrigger(INTRO_Hash);
    }
}
