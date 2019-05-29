using System;

public enum ActionType
{
    //Declaration of every action the AI can take
    MeleeAttack,
    RangedAttack, //an instant melee or ranged attack
    MoveToRandomLocation, //the enemy will find a location and move to it stalking the player
    Strafe, //The enemy will strafe around the player
    BackUp, //The enemy will back up
    Defend, //The enemy will block an attack
    Dodge, //The enemy will perform a dodge action
    Buff, //The enemy will stand still and grow in power.
    SpellCast //The enemy will stand still and cast a spell
}