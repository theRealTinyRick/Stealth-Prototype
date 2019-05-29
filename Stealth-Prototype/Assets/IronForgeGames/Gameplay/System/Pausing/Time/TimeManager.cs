using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AH.Max.System
{
	namespace AH.Max.Gameplay
	{
		public class TimeManager : Entity 
		{
			protected override void Enable()
			{
				GamePause.pausedEvent.AddListener(PauseTime);
			}

			protected override void Disable()
			{
				GamePause.UnPausedEvent.AddListener(UnpauseTime);
			}

			// Sets time scale to zero
			private void PauseTime()
			{
				Time.timeScale = 0;
			}

			// Resets time scale
			private void UnpauseTime()
			{
				Time.timeScale = 1;
			}
		}
	}
}

