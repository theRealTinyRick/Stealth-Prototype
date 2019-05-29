using System;

[System.Serializable]
public enum PlayerState : int
{
	Normal, /*This is the normal state of the player where any basic action can be acrries out */
	Traversing, /*This is the state where the player is climbing or another simlar action. Only those actions can be acrries out */
	Attacking, /*When in this state player is in the freemove state the player may freely move to this one while attacking
	Other actions cannot be activated until the player has left this state which is done once an attack is finished */
	Evading /* Use this to calculate player state where the player is rolling or evading in general*/
}