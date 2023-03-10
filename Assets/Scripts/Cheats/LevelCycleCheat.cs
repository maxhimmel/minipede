using Minipede.Gameplay.LevelPieces;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Cheats
{
	[System.Serializable]
	public class LevelCycleCheat : LevelCycleTimer.ISettings
	{
		float LevelCycleTimer.ISettings.CycleDuration => _settings.CycleDuration;

		private readonly Settings _settings;

		public LevelCycleCheat( Settings settings,
			LevelCycleTimer.ISettings baseSettings )
		{
			_settings = settings;
		}

		[System.Serializable]
		public class Settings
		{
			[MinValue( 0 ), MaxValue( 9999 )]
			public float CycleDuration;

			[ButtonGroup, EnableIf( "@UnityEngine.Application.isPlaying" )]
			private void Interrupt()
			{
				GetLevelCycleTimer().Stop();
			}

			[ButtonGroup, EnableIf( "@UnityEngine.Application.isPlaying" )]
			private void Resume()
			{
				GetLevelCycleTimer().Play();
			}

			private LevelCycleTimer GetLevelCycleTimer()
			{
				var context = GameObject.FindObjectOfType<SceneContext>();
				return context.Container.Resolve<LevelCycleTimer>();
			}
		}
	}
}