using Minipede.Gameplay.LevelPieces;
using UnityEngine;
using Zenject;

namespace Minipede.Cheats
{
	public class LevelBalanceCheat : LevelBalanceController
	{
		private readonly Settings _settings;

		public LevelBalanceCheat( Settings settings, 
			SignalBus signalBus,
			LevelBalanceController baseController ) 
			: base( settings, signalBus )
		{
			_settings = settings;
		}

		public override void IncrementCycle()
		{
			if ( _settings.DisableIncrementing )
			{
				Debug.LogWarning( $"<b>{nameof( LevelBalanceCheat )}</b> is enabled.\n" +
					$"No cycles will increment." );
				return;
			}

			base.IncrementCycle();
		}

		[System.Serializable]
		public new class Settings : LevelBalanceController.Settings
		{
			public bool DisableIncrementing;
		}
	}
}