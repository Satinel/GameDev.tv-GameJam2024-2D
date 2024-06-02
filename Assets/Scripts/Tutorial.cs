using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class Tutorial : MonoBehaviour
{
    public static event Action OnTargetingTutorialOver;

    [SerializeField] Battle _battle;
    [SerializeField] Unit _unit;
    [SerializeField] Enemy _enemy, _enemy1;
    [SerializeField] GameObject _textBox, _pointer, _incomingEnemy;
    [SerializeField] Button _manualButton, _autoButton, _pauseButton, _playButton, _fastForwardButton, _doubleFastButton, _retreatButton, _button1, _button2, _button3, _button4, _button5, _button5b, _button6, _button7, _button8;
    [SerializeField] Button[] _enemyButtons;
    [SerializeField] TextMeshProUGUI _text, _button2Text;
    Button _unitButton;
    TeamManager _teamManager;

    bool _rejectedTutorial, _manualPressed, _enemyClicked, _enemyKilled, _unitClicked, _speedChanged;
    Enemy _currentEnemy;

    void OnEnable()
    {
        Battle.OnBattleStarted += Battle_OnBattleStarted;
        Unit.OnAnyUnitClicked += Unit_OnAnyUnitClicked;
        Enemy.OnAnyEnemyClicked += Enemy_OnAnyEnemyClicked;
        Enemy.OnAnyEnemyKilled += Enemy_OnAnyEnemyKilled;
        Timer.OnHalfTime += Timer_OnHalfTime;
        TeamManager.OnManualPressed += TeamManager_OnManualPressed;
    }

    void OnDisable()
    {
        Battle.OnBattleStarted -= Battle_OnBattleStarted;
        Unit.OnAnyUnitClicked -= Unit_OnAnyUnitClicked;
        Enemy.OnAnyEnemyClicked -= Enemy_OnAnyEnemyClicked;
        Enemy.OnAnyEnemyKilled -= Enemy_OnAnyEnemyKilled;
        Timer.OnHalfTime -= Timer_OnHalfTime;
        TeamManager.OnManualPressed -= TeamManager_OnManualPressed;
    }

    void Start()
    {
        _teamManager = FindFirstObjectByType<TeamManager>();
        _unit = _teamManager.Team[0];
        _manualButton = _teamManager.ManualButton.GetComponent<Button>();
        _autoButton = _teamManager.AutoButton.GetComponent<Button>();
        _unitButton = _unit.GetComponentInChildren<Button>();
        _unitButton.interactable = false;
        _manualButton.gameObject.SetActive(false);
        _autoButton.interactable = false;
        _autoButton.gameObject.SetActive(false);
        _pauseButton.gameObject.SetActive(false);
        _playButton.gameObject.SetActive(false);
        _fastForwardButton.gameObject.SetActive(false);
        _doubleFastButton.gameObject.SetActive(false);
        foreach (var button in _enemyButtons)
        {
            button.gameObject.SetActive(false);
        }
        _retreatButton.gameObject.SetActive(false);
    }

    void Battle_OnBattleStarted()
    {
        _pointer.SetActive(false);
        _battle.PauseBattle();
        _textBox.SetActive(true);
        _text.text = "Would you like to join me in a short tutorial?";
        _button1.gameObject.SetActive(true);
        _button2.gameObject.SetActive(true);
        _button2Text.text = "No Thanks!";
        _unit.SetTutorialTarget(_enemy);
    }
    
    public void Button1()
    {
        _button1.gameObject.SetActive(false);
        _button2.gameObject.SetActive(false);
        _pointer.SetActive(true);
        _pointer.transform.position = _unit.EnemyTarget.transform.position;
        _text.text = $"When battle starts {_unit.HeroName} will automatically pick an enemy and start fighting. ";
        _text.text += "When the enemy is defeated a new target will be picked at random.";
        
        _button3.gameObject.SetActive(true);
    }

    public void Button2()
    {
        if(!_rejectedTutorial)
        {
            _text.text = "Good luck and have fun!";
        
            _button2Text.text = "Okay, bye!";
            _button1.gameObject.SetActive(false);
            _rejectedTutorial = true;
        }
        else
        {
            OnTargetingTutorialOver?.Invoke();
            StartNormalBattle();
        }
    }

    void StartNormalBattle()
    {
        _autoButton.interactable = true;
        _autoButton.gameObject.SetActive(false);
        _unitButton.interactable = true;
        _manualButton.gameObject.SetActive(true);
        _pauseButton.gameObject.SetActive(true);
        _playButton.gameObject.SetActive(true);
        _fastForwardButton.gameObject.SetActive(true);
        _doubleFastButton.gameObject.SetActive(true);
        foreach (var button in _enemyButtons)
        {
            button.gameObject.SetActive(true);
        }
        _retreatButton.gameObject.SetActive(true);
        _battle.UnpauseBattle();
        Destroy(gameObject);
    }

    public void Button3()
    {
        _button3.gameObject.SetActive(false);
        _pointer.transform.position = _manualButton.transform.position;
        _text.text = "You can change to Manual targeting for more control.";
        _manualButton.gameObject.SetActive(true);
    }

    void TeamManager_OnManualPressed()
    {
        if(!_manualPressed)
        {
            _text.text = $"Click on {_unit.HeroName} to select them.";
            _pointer.transform.position = _unit.transform.position;
            _manualPressed = true;
            _unitButton.interactable = true;
        }
    }

    void Unit_OnAnyUnitClicked(object sender, Unit e)
    {
        if(!_unitClicked)
        {
            _unitButton.interactable = false;
            _unitClicked = true;
            _text.text = $"Click on a different enemy to tell {_unit.HeroName} to attack them.";
            _currentEnemy = _unit.EnemyTarget;
            foreach (var button in _enemyButtons)
            {
                button.gameObject.SetActive(true);
            }
            if(_currentEnemy != _enemy1)
            {
                _pointer.transform.position = _enemy1.transform.position;
            }
            else
            {
                _pointer.transform.position = _enemy.transform.position;
            }
        }
    }
    
    void Enemy_OnAnyEnemyClicked(object sender, Enemy e)
    {
        if(!_enemyClicked)
        {
            _pointer.SetActive(false);
            _enemyClicked = true;
            _text.text = "";
            if(e == _currentEnemy)
            {
                _text.text += $"Hey that's the enemy {_unit.HeroName} was already attacking! ...well, that's fine too. ";
            }
            _text.text += $"If this enemy is defeated, {_unit.HeroName} will wait for you to pick a new enemy.";
            _enemyClicked = true;
            _button4.gameObject.SetActive(true);
            _unitButton.interactable = true;
        }
    }

    public void Button4()
    {
        _button4.gameObject.SetActive(false);
        _button5.gameObject.SetActive(true);
        _text.text = "You can switch between Auto and Manual targeting at any time.\n";
        _text.text += "<color=#000000>But enough talk, it's time to battle!";
        _manualButton.gameObject.SetActive(false);
        _autoButton.gameObject.SetActive(true);
        _autoButton.interactable = true;
        _pointer.SetActive(true);
        _pointer.transform.position = _autoButton.transform.position;
        OnTargetingTutorialOver?.Invoke();
    }

    public void Button5()
    {
        _button5.gameObject.SetActive(false);
        _textBox.SetActive(false);
        _pointer.SetActive(false);
        _battle.UnpauseBattle();
    }

    void Enemy_OnAnyEnemyKilled(object sender, Enemy enemy)
    {
        if(!_enemyKilled)
        {
            _battle.PauseBattle();
            _enemyKilled = true;
            _battle.PauseBattle();
            _textBox.SetActive(true);
            _pointer.SetActive(true);
            _pointer.transform.position = _incomingEnemy.transform.position;
            _text.text = "When an enemy is defeated you earn gold. ";
            _text.text += "A new enemy will soon arrive to take its place.";
            _button5b.gameObject.SetActive(true);
        }
    }

    public void Buton5b()
    {
        _button5b.gameObject.SetActive(false);
        _text.text = "If your team is defeated (like in this scripted loss) ";
        _text.text += "you'll lose half the gold you earned this battle, and the castle you're protecting will take damage.";
        _button6.gameObject.SetActive(true);
    }

    public void Button6()
    {
        _button6.gameObject.SetActive(false);
        _fastForwardButton.gameObject.SetActive(true);
        _pointer.SetActive(true);
        _pointer.transform.position = _fastForwardButton.transform.position;
        _text.text = "Your goal is to survive four hours. ";
        _text.text += "For every hour which passes in battle, <i>one minute passes in the real world.</i> ";
        _text.text += "But you hold the power to manipulate time itself!";
    }

    public void FastForwardButton()
    {
        if(_speedChanged) { return; }

        _textBox.SetActive(false);
        _pointer.SetActive(false);
        Invoke(nameof(TimeManipulation), 10f);
    }

    void TimeManipulation()
    {
        _playButton.gameObject.SetActive(true);
        _pauseButton.gameObject.SetActive(true);
        _fastForwardButton.interactable = false;
        _playButton.interactable = false;
        _pauseButton.interactable = false;
        _battle.PauseBattle();
        _textBox.SetActive(true);
        _text.text = "Pretty useful, right? ";
        _text.text += "You can also pause time like I keep doing. ";
        _text.text += "Or set it to a ludicrous speed!";
        _doubleFastButton.gameObject.SetActive(true);
        _pauseButton.gameObject.SetActive(true);
        _pointer.SetActive(true);
        _pointer.transform.position = _doubleFastButton.transform.position;
    }

    public void DoubleFastButton()
    {
        if(_speedChanged) { return; }

        _textBox.SetActive(false);
        _pointer.SetActive(false);
        _fastForwardButton.interactable = true;
        _playButton.interactable = true;
        _pauseButton.interactable = true;
        _speedChanged = true;
    }

    void Timer_OnHalfTime()
    {
        _battle.PauseBattle();
        _textBox.SetActive(true);
        _text.text = "Halfway through the night, monsters will enter a Frenzied state. ";
        _text.text += "They'll be twice as strong and twice as tough!";
        _button7.gameObject.SetActive(true);
    }

    public void Button7()
    {
        _button7.gameObject.SetActive(false);
        _button8.gameObject.SetActive(true);
        _retreatButton.gameObject.SetActive(true);
        _retreatButton.interactable = false;
        _pointer.SetActive(true);
        _pointer.transform.position = _retreatButton.transform.position;
        _text.text = "If you feel like you can't win a battle: RETREAT! ";
        _text.text += "The castle will take damage but you'll keep ALL of your gold. ";
        _text.text += "That's everything you need to know, the rest is up to you!";
    }

    public void Button8()
    {
        _textBox.SetActive(false);
        _battle.UnpauseBattle();
        _button8.gameObject.SetActive(false);
        _retreatButton.interactable = true;
    }
}
