using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace AH.Max.System
{
	public class GamePause
	{
		private static bool isPaused = false;
		public static bool IsPaused
		{
			get { return isPaused; }
		}

		///<Summary>
		/// Subscribe to this event to respond to pausing
		///</Summary>
		public static PausedEvent pausedEvent = new PausedEvent();

		///<Summary>
		/// Subscribe to this event to respond to unpausing
		///</Summary>
		public static UnPausedEvent UnPausedEvent = new UnPausedEvent();

		///<Summary>
		/// Method used to pause the game
		///</Summary>
		public static void Pause()
		{
			isPaused = true;

			pausedEvent.Invoke();
		}

		///<Summary>
		///Method used to unpause the game.
		///</Summary>
		public static void UnPause()
		{
			isPaused = false;

			UnPausedEvent.Invoke();
		}
	}
}
