using System;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

namespace AH.Max.Gameplay.AI
{
	public interface IEnemy
	{
		Transform Target { get; }

		EnemyType EnemyType { get; }

		float MaxFieldOfViewAngle { get; }

		float MaxHeightDifference { get; }

		float AggroRange { get; }

		float AttackRange { get; }
	} 
}
