using System;

public enum ActionInteruptType
{
    None,
    ActionEnded, //this is the end of the action
    SwitchAction, //the AI has triggered a new action to be used immediately.
    TookDamage, //AI has taken damage and shoudl end the action and a new action in response should be chosen
    Parried, //AI's attack was parried and a new action in response should be chosem
    Died, //AI health dropped below zero and died
    FightEnded //The fight has ended and we are canceling all actions and killing enemy.
}